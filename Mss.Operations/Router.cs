using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Devices.Tags;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public class Router : LineOperationBase
    {
        private RouterParameterSetWrapper _parameters;

        public override OperationCode OperationCode
        {
            get
            {
                switch (CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return Level == Levels.Lower
                            ? OperationCode.LR1
                            : OperationCode.UR1;
                    case CraneNumber.Crane2:
                        return Level == Levels.Lower
                            ? OperationCode.LR2
                            : OperationCode.UR2;
                    case CraneNumber.Crane3:
                        return Level == Levels.Lower
                            ? OperationCode.LR3
                            : OperationCode.UR3;
                    case CraneNumber.Crane4:
                        return Level == Levels.Lower
                            ? OperationCode.LR4
                            : OperationCode.UR4;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        //         protected override void ResetOperationVariables()
        //         {
        //             base.ResetOperationVariables();
        //         }

        protected string SizingTestResultName = "Sizing Test Result";
        protected SizingTestResult SizingTestResult
        {
            get => GetVariable<SizingTestResult>(SizingTestResultName);
            set => SetVariable(SizingTestResultName, value);
        }


        public CraneNumber CraneNumber => _parameters.CraneNumber;

        public Levels Level => _parameters.Level;

        protected Pit Pit => Level == Levels.Lower
            ? (Pit)LowerPit
            : (Pit)UpperPit;

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (RouterParameterSetWrapper)parameters;
        }

        protected override void RegisterCustomStates()
        {
            base.RegisterCustomStates();
            RegisterState(AwaitingDestinationState, "Awaiting Destination", AwaitingDestinationStateHandler, true);
            RegisterState(AwaitingSizingTestResultState, "Awaiting Sizing Test Result", AwaitingSizingTestResultStateHandler, true);
        }

        protected override void DoStart()
        {
            base.DoStart();

            SystemSettings.DataItemChanged += _SystemSettings_DataItemChanged;
            Pit.DataItemChanged += _Pit_DataItemChanged;

            if (CraneNumber == CraneNumber.Crane1)
            {
                StartPlcTagCapture(
                    Constant.SizingTestResultRoleName,
                    _SizingTestResult_TagValueChanged,
                    XTagDataCaptureUpdateMode.OnChange);
            }
        }

        protected override void DoStop()
        {
            SystemSettings.DataItemChanged -= _SystemSettings_DataItemChanged;
            Pit.DataItemChanged -= _Pit_DataItemChanged;

            base.DoStop();
        }

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(out int moveCommand, out string extendedState)
        {
            string fault;
            PalletItem palletItem;

            if (CraneNumber == CraneNumber.Crane1)
            {
                switch (SizingTestResult)
                {
                    case SizingTestResult.NoResult:
                        moveCommand = Constant.NoMoveCommand;
                        extendedState = string.Empty;
                        SetCurrentState(AwaitingSizingTestResultState);
                        return false;
                    case SizingTestResult.Pass:
                        // Nothing to do; Just fall through
                        break;
                    case SizingTestResult.Fail:
                    default:
                        if (!QueryMesPallet(
                            PalletID,
                            out palletItem,
                            out fault))
                        {
                            moveCommand = Constant.NoMoveCommand;
                            extendedState = string.Empty;
                            SetOperationFaulted(fault);
                            return false;
                        }
                        CurrentPallet = palletItem;
                        CurrentPallet.Status = PalletStatus.Purge;
                        CurrentPallet.Comment = "Failed Sizing Test";
                        DataLayer.SetPitPallet(
                            Level,
                            CurrentPallet,
                            PitCode.Purge);
                        moveCommand = Constant.RouterMoveCommandForward;
                        extendedState = $"(FAILED SIZING) Forwarding Pallet {CurrentPallet.PalletID} to Purge.";
                        return false;
                }
            }

            if (!DataLayer.ProcessPalletAtRouter(
                OperationCode,
                CraneNumber,
                Level,
                PalletID,
                out palletItem,
                out moveCommand,
                out extendedState,
                out fault))
            {
                if (IsFaulted(fault))
                {
                    SetOperationFaulted(fault);
                    return false;
                }
                _inhibitProcessPalletState = true;
                SetCurrentState(AwaitingDestinationState, extendedState);
                return false;
            }
            CurrentPallet = palletItem;
            return true;
        }

        #endregion

        //==================================================================================

        #region AwaitingDestination State

        protected readonly string AwaitingDestinationState = "AwaitingDestination";
        private bool _inhibitProcessPalletState = false;

        protected virtual void AwaitingDestinationStateHandler()
        {
            if (!_inhibitProcessPalletState)
            {
                SetCurrentState(ProcessPalletState);
            }
            _inhibitProcessPalletState = false;
        }
        #endregion

        //==================================================================================

        #region AwaitingSizingTestResult State

        protected readonly string AwaitingSizingTestResultState = "AwaitingSizingTestResult";

        protected virtual void AwaitingSizingTestResultStateHandler()
        {
        }
        #endregion

        //==================================================================================

        #region AwaitingMoveCompletedState

//         protected override bool DoMoveCompleted()
//         {
//             return true;
//         }

        #endregion

        //==================================================================================

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingDestinationState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _Pit_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingDestinationState)
            {
                RunCurrentStateHandler();
            }
        }

        //==================================================================================

        private void _SizingTestResult_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out int result))
            {
                SizingTestResult = (SizingTestResult)result;
                if (SizingTestResult != SizingTestResult.NoResult
                    && CurrentState.Name == AwaitingSizingTestResultState)
                {
                    SetCurrentState(ProcessPalletState);
                }
            }
        }
    }
}
