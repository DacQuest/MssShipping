using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Devices;
using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Operations;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public class TrailerLoad : XSimpleOperation
    {
        private TrailerLoadParameterSetWrapper _parameters;

        protected string TrailerNumberName = "Trailer Number";
        protected string TrailerNumber
        {
            get => GetVariable<string>(TrailerNumberName);
            set => SetVariable(TrailerNumberName, value);
        }

        protected string UpperLevelCompletedName = "Upper Level Completed";
        protected bool UpperLevelCompleted
        {
            get => GetVariable<bool>(UpperLevelCompletedName);
            set => SetVariable(UpperLevelCompletedName, value);
        }

        protected string LowerLevelCompletedName = "Lower Level Completed";
        protected bool LowerLevelCompleted
        {
            get => GetVariable<bool>(LowerLevelCompletedName);
            set => SetVariable(LowerLevelCompletedName, value);
        }

        protected string LoadAcceptedName = "Load Accepted";
        protected bool LoadAccepted
        {
            get => GetVariable<bool>(LoadAcceptedName);
            set => SetVariable(LoadAcceptedName, value);
        }

        protected string LoadTrailerCommandName = "Load Trailer Command";
        protected int LoadTrailerPermissive
        {
            get => GetVariable<int>(LoadTrailerCommandName);
            set => SetVariable(LoadTrailerCommandName, value);
        }

        protected void WritePlc(string tagRoleName, object value)
            => WriteTag(Constant.PlcRoleName, tagRoleName, value);

        protected XTagData ReadPlc(string tagRoleName)
            => ReadTag(Constant.PlcRoleName, tagRoleName, true);

        protected void StartPlcTagCapture(
            string tagRoleName,
            XTagDataEventHandler handler,
            XTagDataCaptureUpdateMode updateMode)
                => StartTagDataCapture(Constant.PlcRoleName, tagRoleName, handler, updateMode);

        protected void ClearSoftwareFaultInPlc()
            => WritePlc(Constant.SoftwareFaultRoleName, 0);

        protected void SetSoftwareFaultInPlc()
            => WritePlc(Constant.SoftwareFaultRoleName, 1);

        protected void SetOperationFaulted(string extendedState)
        {
            SetSoftwareFaultInPlc();
            SetFaulted(extendedState);
        }

        //         protected bool TrailerLoaded
        //         {
        //             get => GetVariable<bool>(TrailerLoadedName);
        //             set => SetVariable(TrailerLoadedName, value);
        //         }

        bool LoadCompleted => LowerLevelCompleted && UpperLevelCompleted;

        protected SlugLetter SlugLetter => _parameters.SlugLetter;

        private Storage _storage;
        private AssignmentPit _assignmentPit;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private HoldCodes _holdCodes;
        private LowerRecircBuffer _lowerRecirc;
        private UpperRecircBuffer _upperRecirc;
        private SlugA _slugA;
        private SlugB _slugB;

        protected DataLayer DataLayer
        {
            get; private set;
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (TrailerLoadParameterSetWrapper)parameters;

        protected override void RegisterCustomStates()
        {
            RegisterState(AwaitingLoadCompletedState, "Awaiting Load Completed", AwaitingLoadCompletedStateHandler);
            RegisterState(AwaitingLoadAcceptedState, "Awaiting Load Accepted", AwaitingLoadAcceptedStateHandler);
            RegisterState(AwaitingTrailerNumberState, "Awaiting Trailer Number", AwaitingTrailerNumberStateHandler);
            RegisterState(AwaitingTrailerLoadedState, "Awaiting Trailer Loaded", AwaitingTrailerLoadedStateHandler);
            base.RegisterCustomStates();
        }

        protected override void ResetOperationVariables()
        {
            TrailerNumber = Constant.NoTrailerNumber;
            UpperLevelCompleted = false;
            LowerLevelCompleted = false;
            LoadAccepted = false;
//             TrailerLoaded = false;
        }

        protected override void AutoSubscribe()
        {
            Subscribe(
                AcceptLoadMessageData.AcceptLoadMessageTopic,
                _AcceptLoad_OnMessage,
                XMessageScopes.All);

            Subscribe(
                PrintLabelMessageData.ReprintLoadLabelRequest,
                _ReprintLoadLabel_OnMessage,
                XMessageScopes.All);

            base.AutoSubscribe();
        }

        private void _ReprintLoadLabel_OnMessage(
            object sender,
            XMessageEventArgs e)
        {
            PrintLabelMessageData md = (PrintLabelMessageData)e.MessageData;

            if (md.SlugLetter != SlugLetter)
            {
                return;
            }

            // This will print directly when the TrailerLoad ops own printers
            // For now it forwards the request to the Lower Load Director

            if (!_PrintLoadLabel(
                md.SlugLetter,
                TrailerNumber,
                md.SmallestRotation,
                md.LargestRotation,
                md.PalletCount,
                out string fault))
            {
                SetFaulted(fault);
            }
        }

        private bool _PrintLoadLabel(
            SlugLetter slugLetter,
            string trailerNumber,
            string firstRotation,
            string lastRotation,
            int palletCount,
            out string fault)
        {
            if (TrailerNumber.ValidTrailerNumber())
            {
                XMessaging.Publish(
                    PrintLabelMessageData.PrintLoadLabelRequest,
                    new PrintLabelMessageData(
                        slugLetter,
                        trailerNumber,
                        firstRotation,
                        lastRotation,
                        palletCount),
                    XMessageScopes.All,
                    this);
            }
            fault = string.Empty;
            return true;
        }

        private void _AcceptLoad_OnMessage(
            object sender,
            XMessageEventArgs e)
        {
            AcceptLoadMessageData md = (AcceptLoadMessageData)e.MessageData;
            if (md.SlugLetter != SlugLetter)
            {
                return;
            }
            if (!TrailerNumber.ValidTrailerNumber())
            {
                return;
            }
            SetExtendedState($"Accepting Load on {md.SlugLetter.SlugDisplayName()}...", true);
            string error;
            if (CurrentState.Name == AwaitingLoadAcceptedState)
            {
                if (DataLayer.TryAcceptLoad(
                    md.SlugLetter,
                    TrailerNumber,
                    out string smallestRotation,
                    out string largestRotation,
                    out int palletCount,
                    out error))
                {
                    md.SystemEvent?.Publish();
                    _SendLoadTrailerCommand();
                    if (!_PrintLoadLabel(
                        SlugLetter,
                        TrailerNumber,
                        smallestRotation,
                        largestRotation,
                        palletCount,
                        out string fault))
                    {
                        SetFaulted(fault);
                        return;
                    }
                    SetCurrentState(AwaitingTrailerLoadedState);
                }
            }
            else
            {
                error = "Cannot Accept Load. Trailer Load operation is in the wrong state.";
            }
                md.PublishResponse(new AcceptLoadMessageData(md.SlugLetter, error));
            ClearExtendedState(true);
        }

        protected override void DoStart()
        {
            base.DoStart();

            TrailerNumber = Constant.NoTrailerNumber;
            UpperLevelCompleted = false;
            LowerLevelCompleted = false;
            LoadAccepted = false;
//             TrailerLoaded = false;

            DataLayer = DataLayer.Create(
                out _storage,
                out _assignmentPit,
                out _lowerPit,
                out _upperPit,
                out _systemSettings,
                out _broadcast,
                out _holdCodes,
                out _lowerRecirc,
                out _upperRecirc,
                out _slugA,
                out _slugB);

            if (SlugLetter == SlugLetter.A)
            {
                _slugA.Touched += _Slug_Touched;
            }
            else
            {
                _slugB.Touched += _Slug_Touched;
            }

            LoadAccepted = DataLayer.IsLoadLoadable(SlugLetter);

            ClearSoftwareFaultInPlc();

            StartPlcTagCapture(
                    Constant.LowerLevelCompletedRoleName,
                    _LowerLevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);

            StartPlcTagCapture(
                Constant.UpperLevelCompletedRoleName,
                _UpperLevelCompleted_TagChanged,
                XTagDataCaptureUpdateMode.OnChange);

            StartPlcTagCapture(
                Constant.TrailerNumberRoleName,
                _TrailerNumber_TagChanged,
                XTagDataCaptureUpdateMode.OnChange);

            StartPlcTagCapture(
                Constant.TrailerLoadTrailerPermissiveName,
                _TrailerLoadTrailerPermissive_TagChanged,
                XTagDataCaptureUpdateMode.OnChange);

        }

        private void _Slug_Touched(object sender, XMessageEventArgs eventArgs)
        {
//             if (DataLayer.IsLoadLoadable(SlugLetter))
//             {
//                 SetCurrentState(AwaitingTrailerLoadedState);
//             }
        }

        protected override void DoStop()
        {
            StopAllTagDataCapture(Constant.PlcRoleName);
            base.DoStop();
        }

        protected override void DoRewind()
        {
            ClearExtendedState(true);
            LoadAccepted = DataLayer.IsLoadLoadable(SlugLetter);
            if (LoadAccepted)
            {
                _SendLoadTrailerCommand();
                SetCurrentState(AwaitingTrailerLoadedState);
            }
            else
            {
                SetCurrentState(AwaitingLoadCompletedState);
            }
            base.DoRewind();
        }

        //==================================================================================

        private void _LowerLevelCompleted_TagChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool levelCompleted))
            {
                LowerLevelCompleted = levelCompleted;
                if (LoadCompleted
                    && CurrentState.Name == AwaitingLoadCompletedState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _UpperLevelCompleted_TagChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool levelCompleted))
            {
                UpperLevelCompleted = levelCompleted;
                string currentStateName = CurrentState.Name;
                if (LoadCompleted
                    && CurrentState.Name == AwaitingLoadCompletedState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _TrailerNumber_TagChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out string trailerNumber))
            {
                TrailerNumber = trailerNumber.IsNullOrWhiteSpace()
                    ? Constant.NoTrailerNumber
                    : trailerNumber;

                XMessaging.Publish(
                    TrailerNumberMessageData.TrailerNumberMessageTopicName,
                    new TrailerNumberMessageData(SlugLetter, TrailerNumber),
                    XMessageScopes.All,
                    null);

                string currentStateName = CurrentState.Name;
                if (TrailerNumber.ValidTrailerNumber()
                    && currentStateName == AwaitingTrailerNumberState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _TrailerLoadTrailerPermissive_TagChanged(object sender, XTagDataEventArgs e)
        {
            int previousState = LoadTrailerPermissive;
            if (e.TagData.TryGetTagValue(out int loadTrailerPermissive))
            {
                LoadTrailerPermissive = loadTrailerPermissive;
                if (previousState != Constant.NoMoveCommand
                    && LoadTrailerPermissive == Constant.NoMoveCommand
                    && CurrentState.Name == AwaitingTrailerLoadedState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        //==================================================================================

        #region AwaitingLoadCompletedState

        protected readonly string AwaitingLoadCompletedState = "AwaitingLoadCompleted";

        protected virtual void AwaitingLoadCompletedStateHandler()
        {
            if (LoadCompleted)
            {
                if (!TrailerNumber.ValidTrailerNumber())
                {
                    SetCurrentState(AwaitingTrailerNumberState);
                    return;
                }
                if (DataLayer.TryAutoAcceptLoad(
                    SlugLetter,
                    TrailerNumber,
                    out string smallestRotation,
                    out string largestRotation,
                    out int palletCount))
                {
                    _SendLoadTrailerCommand();
                    if (!_PrintLoadLabel(
                        SlugLetter,
                        TrailerNumber,
                        smallestRotation,
                        largestRotation,
                        palletCount,
                        out string fault))
                    {
                        SetFaulted(fault);
                        return;
                    }
                    SetCurrentState(AwaitingTrailerLoadedState);
                }
                else
                {
                    SetCurrentState(AwaitingLoadAcceptedState);
                }
            }
        }

        private void _SendLoadTrailerCommand()
        {
            LoadTrailerPermissive = Constant.LoadTrailerCommand;
            WritePlc(
                Constant.TrailerLoadTrailerPermissiveName,
                LoadTrailerPermissive);
        }

        #endregion

        //==================================================================================

        #region AwaitingTrailerNumberState

        protected readonly string AwaitingTrailerNumberState = "AwaitingTrailerNumber";

        protected virtual void AwaitingTrailerNumberStateHandler()
        {
            if (TrailerNumber.ValidTrailerNumber())
            {
                if (!LoadCompleted)
                {
                    SetCurrentState(AwaitingLoadCompletedState);
                    return;
                }
                if (DataLayer.TryAutoAcceptLoad(
                    SlugLetter,
                    TrailerNumber,
                    out string smallestRotation,
                    out string largestRotation,
                    out int palletCount))
                {
                    _SendLoadTrailerCommand();
                    if (!_PrintLoadLabel(
                        SlugLetter,
                        TrailerNumber,
                        smallestRotation,
                        largestRotation,
                        palletCount,
                        out string fault))
                    {
                        SetFaulted(fault);
                        return;
                    }
                    SetCurrentState(AwaitingTrailerLoadedState);
                }
                else
                {
                    SetCurrentState(AwaitingLoadAcceptedState);
                }
            }
        }

        #endregion

        //==================================================================================

        #region AwaitingLoadAcceptedState

        protected readonly string AwaitingLoadAcceptedState = "AwaitingLoadAccepted";

        protected virtual void AwaitingLoadAcceptedStateHandler()
        {
            LoadAccepted = DataLayer.IsLoadLoadable(SlugLetter);

            if (LoadAccepted)
            {
                SetCurrentState(AwaitingTrailerLoadedState);
            }
        }

        #endregion

        //==================================================================================

        #region AwaitingTrailerLoadedState

        protected readonly string AwaitingTrailerLoadedState = "AwaitingTrailerLoaded";

        protected virtual void AwaitingTrailerLoadedStateHandler()
        {
            if (LoadTrailerPermissive == Constant.NoMoveCommand
                && TrailerNumber.ValidTrailerNumber()
                && DataLayer.IsLoadLoadable(SlugLetter))
            {
                if (!DataLayer.TryFinalizeLoad(
                    SlugLetter,
                    out string fault))
                {
                    SetFaulted(fault);
                    return;
                }

//                 if (!DataLayer.FinalizeLoad(
//                     SlugLetter,
//                     out string error))
//                 {
//                     if (!error.IsNullOrEmpty())
//                     {
//                         XSystemEvent.Publish(
//                             "FinalizeLoad",
//                             XSystemEventLevel.Error,
//                             $" {SlugLetter.SlugDisplayName()} Finalized Load Error: {error}.");
//                     }
//                     return;
//                 }

                SetCurrentState(DfxRewind);
            }

        }

        #endregion

        //==================================================================================

        protected override void DoDispose()
        {
            if (SlugLetter == SlugLetter.A)
            {
                _slugA.Touched -= _Slug_Touched;
            }
            else
            {
                _slugB.Touched -= _Slug_Touched;
            }

            if (DataLayer != null)
            {
                DataLayer.Dispose();
                DataLayer = null;
            }

            base.DoDispose();
        }

        //==================================================================================

    }
}
