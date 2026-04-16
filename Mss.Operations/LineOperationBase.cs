using System;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Devices;
using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Operations;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Operations
{
    public abstract class LineOperationBase : XSimpleOperation
    {

        public abstract OperationCode OperationCode { get; }

        protected readonly string CurrentPalletName = "Current Pallet";
        protected PalletItem CurrentPallet { get; set; }

        protected string PalletIDName = "Pallet ID";
        protected string PalletID
        {
            get => GetVariable<string>(PalletIDName);
            set => SetVariable(PalletIDName, value);
        }

        protected string MoveCommandName = "Move Command";
        protected int MoveCommand
        {
            get => GetVariable<int>(MoveCommandName);
            set => SetVariable(MoveCommandName, value);
        }

        protected string CachedMoveCommandName = "Cached Move Command";
        protected int CachedMoveCommand
        {
            get => GetVariable<int>(CachedMoveCommandName);
            set => SetVariable(CachedMoveCommandName, value);
        }

        private Storage _storage;
        protected Storage Storage => _storage;

        private AssignmentPit _assignmentPit;
        protected AssignmentPit AssignmentPit => _assignmentPit;

        private LowerPit _lowerPit;
        protected LowerPit LowerPit => _lowerPit;

        private UpperPit _upperPit;
        protected UpperPit UpperPit => _upperPit;

        private SystemSettings _systemSettings;
        protected SystemSettings SystemSettings => _systemSettings;

        private Broadcast _broadcast;
        protected Broadcast Broadcast => _broadcast;

        private HoldCodes _holdCodes;
        protected HoldCodes HoldCodes => _holdCodes;

        private LowerRecircBuffer _lowerRecirc;
        protected LowerRecircBuffer LowerRecirc => _lowerRecirc;

        private UpperRecircBuffer _upperRecirc;
        protected UpperRecircBuffer UpperRecirc => _upperRecirc;

        private SlugA _slugA;
        protected SlugA SlugA => _slugA;

        private SlugB _slugB;
        protected SlugB SlugB => _slugB;

        protected DataLayer DataLayer { get; private set; }

        protected void WritePlc(string tagRoleName, object value)
            => WriteTag(Constant.PlcRoleName, tagRoleName, value);

        protected XTagData ReadPlc(string tagRoleName)
            => ReadTag(Constant.PlcRoleName, tagRoleName, true);

        protected void StartPlcTagCapture(
            string tagRoleName,
            XTagDataEventHandler handler,
            XTagDataCaptureUpdateMode updateMode)
                => StartTagDataCapture(Constant.PlcRoleName, tagRoleName, handler, updateMode);

        protected bool QueryMesPallet(
            string palletID,
            out PalletItem palletItem,
            out string fault)
        {
            return MesInterface.TryFetchPalletItem(
                OperationCode,
                palletID,
                out palletItem,
                out fault);
        }

        protected bool QueryMesPallet(
            string palletID,
            bool requestDestination,
            out PalletDestination destination,
            out PalletItem palletItem,
            out string fault)
        {
            return MesInterface.TryFetchPalletItem(
                OperationCode,
                palletID,
                requestDestination,
                out destination,
                out palletItem,
                out fault);
        }

        //==================================================================================

        #region General Overrides

        protected override void RegisterCustomStates()
        {
            RegisterState(AwaitingPalletState, "Awaiting Pallet", AwaitingPalletStateHandler, true);
            RegisterState(ProcessPalletState, "Processing Pallet", ProcessPalletStateHandler);
//             RegisterState(AwaitingMoveCommandConfirmationState, "Awaiting Move Command Confirmation", AwaitingMoveCommandConfirmationStateHandler);
//             RegisterState(AwaitingMoveCompletedConfirmationState, "Awaiting Move Completed Confirmation", AwaitingMoveCompletedConfirmationStateHandler);
            RegisterState(AwaitingMoveCompletedState, "Awaiting Move Completed", AwaitingMoveCompletedStateHandler);
            base.RegisterCustomStates();
        }

        protected override void BuildStateDetails()
        {
            base.BuildStateDetails();
            if (CurrentPallet != null)
            {
                SetStateDetail(
                    CurrentPalletName,
                    CurrentPallet.GetStateDetails(Constant.OperationDetailsLeadingSpaceCount),
                    false);
            }
            else
            {
                RemoveStateDetail(CurrentPalletName, false);
            }
        }

        protected override void ResetOperationVariables()
        {
            CachedMoveCommand = Constant.NoMoveCommand;
            CurrentPallet = null;

            // Always call the base class when overriding
            base.ResetOperationVariables();
        }

//        protected override void ProcessParameters(XConfigurationParameterSet parameters)
//        {
//        }

        protected override void DoStart()
        {
            PalletID = Constant.NoPalletID;
            MoveCommand = Constant.NoMoveCommand;
            CachedMoveCommand = Constant.NoMoveCommand;
            CurrentPallet = null;

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

            ClearSoftwareFaultInPlc();

            XTagData tagData = ReadPlc(Constant.PalletIDRoleName);
            if (tagData.TryGetTagValue(out string palletID)
                && palletID.ValidPalletID())
            {
                PalletID = palletID;
            }
            StartPlcTagCapture(
                Constant.PalletIDRoleName,
                _PalletID_TagValueChanged,
                XTagDataCaptureUpdateMode.OnRefresh);
            StartPlcTagCapture(
                Constant.MoveCommandRoleName,
                _MoveCommand_TagValueChanged,
                XTagDataCaptureUpdateMode.OnChange);
        }

        protected override void DoStop()
        {
//             StopMoveCommandConfirmationTimer();
//             StopMoveCompletedConfirmationTimer();
            StopAllTagDataCapture(Constant.PlcRoleName);

            if (DataLayer != null)
            {
                DataLayer.Dispose();
                DataLayer = null;
            }

            base.DoStop();
        }

        protected override void DoRewind()
        {
            ClearExtendedState(true);
            SetCurrentState(AwaitingPalletState);
            base.DoRewind();
        }

        #endregion

        protected bool IsFaulted(string fault)
        {
            if (fault.IsNullOrWhiteSpace())
            {
                return false;
            }
            SetOperationFaulted(fault);
            return true;
        }

        //==================================================================================

        #region AwaitingPalletState

        protected readonly string AwaitingPalletState = "AwaitingPallet";

        protected virtual void AwaitingPalletStateHandler()
        {
            if (PalletID.ValidPalletID())
            {
                PalletEventTracker.Capture(
                    PalletID,
                    OperationCode,
                    PalletEvent.Arrival);
                SetCurrentState(ProcessPalletState);
            }
        }

        #endregion

        //==================================================================================

        #region ProcessPalletState

        protected readonly string ProcessPalletState = "ProcessPallet";

        protected virtual void ProcessPalletStateHandler()
        {
            SetExtendedState(
                $"Processing Pallet {PalletID}",
                true);
            if (DoProcessPallet(out int moveCommand, out string extendedState))
            {
                SendMoveCommand(moveCommand);
                SetCurrentState(
                    AwaitingMoveCompletedState,
                    extendedState);
//                 SetCurrentState(
//                     AwaitingMoveCommandConfirmationState,
//                     extendedState);
            }
        }

        protected abstract bool DoProcessPallet(
            out int moveCommand,
            out string extendedState);

        protected void SendMoveCommand(int moveCommand)
        {
            PalletEventTracker.Capture(
                PalletID,
                CurrentPallet,
                OperationCode,
                PalletEvent.MoveCommandSent,
                moveCommand);

            MoveCommand = moveCommand;
            CachedMoveCommand = moveCommand;
            WritePlc(
                Constant.MoveCommandRoleName,
                moveCommand);
        }

        #endregion

        //==================================================================================

        #region AwaitingMoveCommandConfirmationState

//         protected readonly string AwaitingMoveCommandConfirmationState = "AwaitingMoveCommandConfirmation";
// 
//         public const long MoveCommandConfirmationTimerPeriodMilliseconds = 3000; // 3 seconds
//         private long _moveCommandConfirmationTimerID = 0;
// 
//         protected virtual void AwaitingMoveCommandConfirmationStateHandler()
//         {
//             if (MoveCommand == CachedMoveCommand)
//             {
//                 StopMoveCommandConfirmationTimer();
//                 SetCurrentState(AwaitingMoveCompletedConfirmationState);
//             }
//             else
//             {
//                 StartMoveCommandConfirmationTimer();
//             }
//         }
// 
//         protected void StartMoveCommandConfirmationTimer()
//         {
//             StartMoveCommandConfirmationTimer(MoveCommandConfirmationTimerPeriodMilliseconds);
//         }
// 
//         protected void StartMoveCommandConfirmationTimer(long timeout)
//         {
//             StopMoveCommandConfirmationTimer();
//             _moveCommandConfirmationTimerID = StartTimer(_MoveCommandConfirmationTimer_Expired, timeout, null);
//         }
// 
//         protected void StopMoveCommandConfirmationTimer()
//         {
//             if (_moveCommandConfirmationTimerID != 0)
//             {
//                 StopTimer(_moveCommandConfirmationTimerID);
//                 _moveCommandConfirmationTimerID = 0;
//             }
//         }
// 
//         private void _MoveCommandConfirmationTimer_Expired(XTimerEventArgs eventArgs)
//         {
//             _moveCommandConfirmationTimerID = 0;
//             if (CurrentState.Name == AwaitingMoveCommandConfirmationState
//                 && MoveCommand != CachedMoveCommand)
//             {
//                 try
//                 {
//                     XTagData tagData = ReadTag(
//                         Constant.MoveCommandRoleName,
//                         true);
//                     if (tagData.TryGetInt32(out int moveCommand))
//                     {
//                         MoveCommand = moveCommand;
//                     }
//                     if (MoveCommand != CachedMoveCommand)
//                     {
// //                         XSystemEvent.Publish(
// //                             ConfigurationItem.Name,
// //                             XSystemEventLevel.Notification,
// //                             $"Resent Move Command {CachedMoveCommand} for Pallet {PalletID} at {ConfigurationItem.Name}");
// 
//                         SendMoveCommand(CachedMoveCommand);
//                     }
//                 }
//                 catch (Exception x)
//                 {
//                     x.PublishSystemEvent(ConfigurationItem.Name + "_MoveCommandConfirmationTimer_Expired");
//                 }
// 
//                 // No need to set current state.
//                 // We are already in the right one;
//                 RunCurrentStateHandler();
//             }
//         }

        #endregion

        //==================================================================================

        #region AwaitingMoveCompletedConfirmation State

//         public const int MoveCompletedConfirmationTimerPeriodMilliseconds = 30000;  // 30 seconds
// 
//         protected readonly string AwaitingMoveCompletedConfirmationState = "AwaitingMoveCompletedConfirmation";
// 
//         private long _moveCompletedConfirmationTimerID = 0;
// 
//         protected virtual void AwaitingMoveCompletedConfirmationStateHandler()
//         {
//             if (MoveCompleted)
//             {
//                 StopMoveCompletedConfirmationTimer();
//                 SetCurrentState(AwaitingMoveCompletedState);
//             }
//             else
//             {
//                 StartMoveCompletedConfirmationTimer();
//             }
//         }
// 
//         protected void StartMoveCompletedConfirmationTimer()
//         {
//             StartMoveCompletedConfirmationTimer(MoveCompletedConfirmationTimerPeriodMilliseconds);
//         }
// 
//         protected void StartMoveCompletedConfirmationTimer(long timeout)
//         {
//             StopMoveCompletedConfirmationTimer();
//             _moveCompletedConfirmationTimerID = StartTimer(_MoveCompletedConfirmationTimer_Expired, timeout, null);
//         }
// 
//         protected void StopMoveCompletedConfirmationTimer()
//         {
//             if (_moveCompletedConfirmationTimerID != 0)
//             {
//                 StopTimer(_moveCompletedConfirmationTimerID);
//                 _moveCompletedConfirmationTimerID = 0;
//             }
//         }
// 
//         private void _MoveCompletedConfirmationTimer_Expired(XTimerEventArgs eventArgs)
//         {
//             _moveCompletedConfirmationTimerID = 0;
//             if (CurrentState.Name == AwaitingMoveCompletedConfirmationState
//                 && !MoveCompleted)
//             {
//                 try
//                 {
//                     XTagData tagData = ReadTag(
//                         Constant.MoveCommandRoleName,
//                         true);
//                     if (tagData.TryGetInt32(out int moveCommand))
//                     {
//                         MoveCommand = moveCommand;
//                     }
//                 }
//                 catch (Exception x)
//                 {
//                     x.PublishSystemEvent(ConfigurationItem.Name + "_MoveCompletedConfirmationTimer_Expired");
//                 }
// 
//                 //                 XSystemEvent.Publish(
//                 //                     ConfigurationItem.Name,
//                 //                     XSystemEventLevel.Notification,
//                 //                     $"Forced read of Move Command at {ConfigurationItem.Name}");
// 
//                 RunCurrentStateHandler();
//             }
//         }

        #endregion

        //==================================================================================

        #region AwaitingMoveCompletedState

        protected readonly string AwaitingMoveCompletedState = "AwaitingMoveCompleted";
        protected virtual void AwaitingMoveCompletedStateHandler()
        {
            if (MoveCompleted)
            {
                if (DoMoveCompleted())
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.DepartureCompleted,
                        CachedMoveCommand);

                    ClearExtendedState(true);
                    SetRewind();
                }
            }
        }

        protected bool MoveCompleted => MoveCommand == Constant.NoMoveCommand;

        protected virtual bool DoMoveCompleted()
        {
            return true;
        }

        #endregion

        //==================================================================================

        private void _PalletID_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out string palletID))
            {
                PalletID = palletID.ToUpper();
                if (PalletID.ValidPalletID()
                    && CurrentState.Name == AwaitingPalletState)
                {
                    CurrentState.StateHandler();
                }
                else if (!PalletID.ValidPalletID())
                {
                    ProcessPalletIDCleared();
                }
            }
        }

        protected virtual void ProcessPalletIDCleared()
        {
        }

        private void _MoveCommand_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out int moveCommand))
            {
                MoveCommand = moveCommand;
//                 if (CurrentState.Name == AwaitingMoveCommandConfirmationState
//                     || CurrentState.Name == AwaitingMoveCompletedConfirmationState
//                     || CurrentState.Name == AwaitingMoveCompletedState)
                if (CurrentState.Name == AwaitingMoveCompletedState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        //==================================================================================

        //         protected override void DoDispose()
        //         {
        //             base.DoDispose();
        //         }

        protected void ClearSoftwareFaultInPlc()
            => WritePlc(Constant.SoftwareFaultRoleName, 0);

        protected void SetSoftwareFaultInPlc()
            => WritePlc(Constant.SoftwareFaultRoleName, 1);

        protected void SetOperationFaulted(string extendedState)
        {
            SetSoftwareFaultInPlc();
            SetFaulted(extendedState);
        }

//         public override void FaultForUnresponsiveDependencyService(string unresponsiveDependencyServiceName)
//         {
//             SetOperationFaulted($"Dependency {unresponsiveDependencyServiceName} not responsive.");
//         }

//         protected override void FaultOnSharedCollectionOpenFailure(string sharedCollectionName)
//         {
//             SetOperationFaulted($"Failed to open {sharedCollectionName}.");
//         }

    }
}
