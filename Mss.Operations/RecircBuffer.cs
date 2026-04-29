using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Mss.Operations
{
    public class RecircBuffer : LineOperationBase
    {

        private RecircBufferParameterSetWrapper _parameters;
        private string _previousArrivalPalletID = string.Empty;

        protected Levels Level => _parameters.Level;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Level)
                {
                    case Levels.Upper:
                        return OperationCode.URB;
                    case Levels.Lower:
                        return OperationCode.LRB;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        //==================================================================================

        protected override void RegisterCustomStates()
        {
            RegisterState(
                MonitoringBufferState,
                "Monitoring Recirc Buffer",
                MonitoringBufferStateHandler,
                true);
            base.RegisterCustomStates();
        }

        protected override void DoStart()
        {
            base.DoStart();

            SlugA.DataItemChanged += _Slug_DataItemChanged;
            SlugB.DataItemChanged += _Slug_DataItemChanged;
        }

        protected override void DoStop()
        {
            SlugA.DataItemChanged -= _Slug_DataItemChanged;
            SlugB.DataItemChanged -= _Slug_DataItemChanged;

            base.DoStop();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (RecircBufferParameterSetWrapper)parameters;

        //==================================================================================

        #region MonitoringBufferState State

        protected readonly string MonitoringBufferState = "MonitoringBuffer";

        protected virtual void MonitoringBufferStateHandler()
        {
            // Nothing to do. State changes when Slug/Load changes
        }

        private void _Slug_DataItemChanged(
            object sender,
            XDataItemChangedEventArgs eventArgs)
        {
            if (CurrentState.Name == MonitoringBufferState)
            {
                SetCurrentState(ProcessPalletState);
            }
        }

        #endregion

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(
            out int moveCommand,
            out string extendedState)
        {
            if (DataLayer.ProcessPalletAtRecircBuffer(
                OperationCode,
                Level,
                PalletID,
                out PalletItem palletItem,
                out moveCommand,
                out extendedState,
                out string fault))
            {
                CurrentPallet = palletItem;
                return true;
            }
            if (!fault.IsNullOrWhiteSpace())
            {
                SetOperationFaulted(fault);
                return false;
            }
            SetCurrentState(MonitoringBufferState);
            return false;
        }

        #endregion

        //==================================================================================

        #region WaitMoveCompleted State

        protected override bool DoMoveCompleted()
        {
            DataLayer.RemoveRecircBufferPallet(CurrentPallet.PalletID);
            return true;
        }

        #endregion

        //==================================================================================
    }
}
