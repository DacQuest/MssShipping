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
    public class Purge : LineOperationBase
    {

        private PurgeParameterSetWrapper _parameters;

        protected Levels Level => _parameters.Level;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Level)
                {
                    case Levels.Upper:
                        return OperationCode.UP;
                    case Levels.Lower:
                        return OperationCode.LP;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (PurgeParameterSetWrapper)parameters;

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(
            out int moveCommand,
            out string extendedState)
        {
            if (!DataLayer.ProcessPalletAtPurge(
                OperationCode,
                Level,
                PalletID,
                out PalletItem palletItem,
                out moveCommand,
                out extendedState,
                out string fault))
            {
                if (!string.IsNullOrWhiteSpace(fault))
                {
                    SetOperationFaulted(fault);
                }
                return false;
            }
            CurrentPallet = palletItem;
            return true;
        }

        #endregion

        //==================================================================================

        #region WaitMoveCompleted State

        protected override bool DoMoveCompleted()
        {
            if (CachedMoveCommand == Constant.PurgeMoveCommandToPurgeLane)
            {
                PurgePalletWriter.WritePurgePallet(CurrentPallet);
                DataLayer.RemovePitPallet(CurrentPallet.PalletID);
            }
            return true;
        }

        #endregion

        //==================================================================================
    }
}
