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
        public override OperationCode OperationCode
        {
            get
            {
                switch (CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return Level == Levels.Lower
                            ? OperationCode.LIR1
                            : OperationCode.UIR1;
                    case CraneNumber.Crane2:
                        return Level == Levels.Lower
                            ? OperationCode.LIR2
                            : OperationCode.UIR2;
                    case CraneNumber.Crane3:
                        return Level == Levels.Lower
                            ? OperationCode.LIR3
                            : OperationCode.UIR3;
                    case CraneNumber.Crane4:
                        return Level == Levels.Lower
                            ? OperationCode.LIR4
                            : OperationCode.UIR4;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

//         protected override void ResetOperationVariables()
//         {
//             base.ResetOperationVariables();
//         }

        private RouterParameterSetWrapper _parameters;

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
        }

        protected override void DoStart()
        {
            base.DoStart();

            SystemSettings.DataItemChanged += _SystemSettings_DataItemChanged;
            Pit.DataItemChanged += _Pit_DataItemChanged;
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
            if (!DataLayer.ProcessPalletAtInboundRouter(
                CraneNumber,
                Level,
                PalletID,
                out PalletItem palletItem,
                out moveCommand,
                out extendedState,
                out string fault))
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

    }
}
