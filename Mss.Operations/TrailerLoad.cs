using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Operations;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Operations
{
    public class TrailerLoad : XSimpleOperation
    {
        private TrailerLoadParameterSetWrapper _parameters;

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

        protected string TrailerIDName = "Trailer ID";
        protected string TrailerID
        {
            get => GetVariable<string>(TrailerIDName);
            set => SetVariable(TrailerIDName, value);
        }

        protected string TrailerLoadedName = "Trailer Loaded";
        protected bool TrailerLoaded
        {
            get => GetVariable<bool>(TrailerLoadedName);
            set => SetVariable(TrailerLoadedName, value);
        }

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
            RegisterState(AwaitingTrailerLoadedState, "Awaiting Trailer Loaded", AwaitingTrailerLoadedStateHandler);
            RegisterState(AwaitingTrailerIDState, "Awaiting Trailer ID", AwaitingTrailerIDStateHandler);
            base.RegisterCustomStates();
        }

        protected override void DoStart()
        {
            base.DoStart();

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

            StartTagDataCapture(
                    Constant.PlcRoleName,
                    Constant.LowerLevelCompletedRoleName,
                    _LowerLevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);

            StartTagDataCapture(
                Constant.PlcRoleName,
                Constant.UpperLevelCompletedRoleName,
                _UpperLevelCompleted_TagChanged,
                XTagDataCaptureUpdateMode.OnChange);

            StartTagDataCapture(
                Constant.PlcRoleName,
                Constant.TrailerIDRoleName,
                _TrailerID_TagChanged,
                XTagDataCaptureUpdateMode.OnChange);

            StartTagDataCapture(
                Constant.PlcRoleName,
                Constant.TrailerLoadedRoleName,
                _TrailerLoaded_TagChanged,
                XTagDataCaptureUpdateMode.OnChange);

        }

        private void _Slug_Touched(object sender, XMessageEventArgs eventArgs)
        {
            if (DataLayer.IsLoadLoadable(SlugLetter))
            {
                SetCurrentState(AwaitingTrailerLoadedState);
            }
        }

        protected override void DoStop()
        {
            StopAllTagDataCapture(Constant.PlcRoleName);
            base.DoStop();
        }

        protected override void DoRewind()
        {
            ClearExtendedState(true);
            if (DataLayer.IsLoadLoadable(SlugLetter))
            {
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
                if (CurrentState.Name == AwaitingLoadCompletedState
                    && LoadCompleted)
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
                if (CurrentState.Name == AwaitingLoadCompletedState
                    && LoadCompleted)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _TrailerID_TagChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out string trailerID))
            {
                TrailerID = trailerID;
                string currentStateName = CurrentState.Name;
                if ((currentStateName == AwaitingTrailerIDState
                        || currentStateName == AwaitingTrailerLoadedState)
                    && !TrailerID.IsNullOrWhiteSpace())
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _TrailerLoaded_TagChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool trailerLoaded))
            {
                TrailerLoaded = trailerLoaded;
                string currentStateName = CurrentState.Name;
                if ((currentStateName == AwaitingTrailerIDState
                        || currentStateName == AwaitingTrailerLoadedState)
                    && TrailerLoaded)
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
                _ = DataLayer.TryAutoAcceptLoad(SlugLetter);
            }
        }

        #endregion

        //==================================================================================

        #region AwaitingTrailerLoadedState

        protected readonly string AwaitingTrailerLoadedState = "AwaitingTrailerLoaded";

        protected virtual void AwaitingTrailerLoadedStateHandler()
        {
            if (TrailerLoaded)
            {
                if (TrailerID.IsNullOrWhiteSpace())
                {
                    SetCurrentState(AwaitingTrailerIDState);
                    return;
                }
                if (!DataLayer.IsLoadLoadable(SlugLetter))
                {
                    SetCurrentState(AwaitingLoadCompletedState);
                    return;
                }
                if (!DataLayer.FinalizeLoad(SlugLetter, TrailerID, out string error))
                {
                    SetFaulted(error);
                    return;
                }

                WriteTag(
                    Constant.PlcRoleName,
                    Constant.TrailerLoadedRoleName,
                    false);

                if (!DataLayer.TryAutoReleaseBroadcast(out error))
                {
                    if (!error.IsNullOrWhiteSpace())
                    {
                        XSystemEvent.Publish(
                            ConfigurationItem.FriendlyName,
                            XSystemEventLevel.Error,
                            error);
                    }
                }

                SetCurrentState(DfxRewind);
            }

        }

        #endregion

        //==================================================================================

        #region AwaitingTrailerIDState

        protected readonly string AwaitingTrailerIDState = "AwaitingTrailerID";

        protected virtual void AwaitingTrailerIDStateHandler()
        {
            if (!TrailerID.IsNullOrWhiteSpace())
            {
                SetCurrentState(AwaitingTrailerLoadedState);
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
