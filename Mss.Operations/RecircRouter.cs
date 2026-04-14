using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
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
    public class RecircRouter : LineOperationBase
    {

        private RecircRouterParameterSetWrapper _parameters;
        private bool _isLoadPallet = false;

        protected Levels Level => _parameters.Level;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Level)
                {
                    case Levels.Upper:
                        return OperationCode.URR;
                    case Levels.Lower:
                        return OperationCode.LRR;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (RecircRouterParameterSetWrapper)parameters;

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(
            out int moveCommand,
            out string extendedState)
        {
            if (DataLayer.ProcessPalletAtRecircRouter(
                OperationCode,
                Level,
                PalletID,
                out _isLoadPallet,
                out PalletItem palletItem,
                out moveCommand,
                out extendedState,
                out string fault))
            {
                CurrentPallet = palletItem;
                return true;
            }
            if (!string.IsNullOrWhiteSpace(fault))
            {
                SetOperationFaulted(fault);
            }
            return false;
        }

        #endregion

        //==================================================================================

        #region WaitMoveCompleted State

        protected override bool DoMoveCompleted()
        {
            if (CachedMoveCommand == Constant.RecircRouterMoveCommandForward)
            {
                if (_isLoadPallet)
                {
                    DataLayer.RemovePitPallet(CurrentPallet.PalletID);
                }
                DataLayer.RemoveRecircBufferPallet(CurrentPallet.PalletID);
            }
            else if (CachedMoveCommand == Constant.RecircRouterMoveCommandToRecircBuffer)
            {
                DataLayer.RemovePitPallet(CurrentPallet.PalletID);
                DataLayer.SetRecircBufferPallet(Level, CurrentPallet);
            }
            return true;
        }

        #endregion

        //==================================================================================
    }
}
