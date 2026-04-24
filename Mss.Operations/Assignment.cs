using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Strings;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public class Assignment : LineOperationBase
    {

        private AssignmentParameterSetWrapper _parameters;

        protected int Number => _parameters.Number;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Number)
                {
                    case 1:
                        return OperationCode.AS1;
                    case 2:
                        return OperationCode.AS2;
                    case 3:
                        return OperationCode.AS3;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        //==================================================================================

        protected override void RegisterCustomStates()
        {
            base.RegisterCustomStates();
            RegisterState(AwaitingDestinationState, "Awaiting Destination", AwaitingDestinationStateHandler, true);
        }
        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (AssignmentParameterSetWrapper)parameters;
        }
        protected override void DoStart()
        {
            base.DoStart();

            AssignmentPit.DataItemChanged += _AssignmentPit_DataItemChanged;
        }

        protected override void DoStop()
        {
            if (AssignmentPit != null)
            {
                AssignmentPit.DataItemChanged -= _AssignmentPit_DataItemChanged;
            }
        }

        //==================================================================================

        protected override bool DoProcessPallet(out int moveCommand, out string extendedState)
        {
            if (!DataLayer.ProcessPalletAtAssignment(
                OperationCode,
                PalletID,
                out PalletItem palletItem,
                out moveCommand,
                out extendedState,
                out string fault))
            {
                if (!fault.IsNullOrWhiteSpace())
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

        private void _AssignmentPit_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingDestinationState)
            {
                RunCurrentStateHandler();
            }
        }

    }
}
