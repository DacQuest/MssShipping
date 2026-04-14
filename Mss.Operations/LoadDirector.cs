using DacQuest.DFX.Core.Configuration;
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
    public class LoadDirector : LineOperationBase
    {
        private LoadDirectorParameterSetWrapper _parameters;

        protected Levels Level => _parameters.Level;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Level)
                {
                    case Levels.Upper:
                        return OperationCode.ULD;
                    case Levels.Lower:
                        return OperationCode.LLD;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (LoadDirectorParameterSetWrapper)parameters;

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(
            out int moveCommand,
            out string extendedState)
        {
            throw new NotImplementedException("LoadDirector.DoProcessPallet() is not implemented!");
//             if (DataLayer.ProcessPalletAtPurge(
//                 OperationCode,
//                 Level,
//                 PalletID,
//                 out PalletItem palletItem,
//                 out moveCommand,
//                 out extendedState,
//                 out string fault))
//             {
//                 CurrentPallet = palletItem;
//                 return true;
//             }
//             if (!string.IsNullOrWhiteSpace(fault))
//             {
//                 SetOperationFaulted(fault);
//             }
//             return false;
        }

        #endregion

        //==================================================================================

        #region WaitMoveCompleted State

        protected override bool DoMoveCompleted()
        {
            throw new NotImplementedException("LoadDirector.DoMoveCompleted() is not implemented!");
//             if (CachedMoveCommand == Constant.PurgeMoveCommandToPurgeLane)
//             {
//                 PurgePalletWriter.WritePurgePallet(CurrentPallet);
//                 DataLayer.RemovePitPallet(CurrentPallet.PalletID);
//             }
//             return true;
        }

        #endregion

        //==================================================================================
    }
}
