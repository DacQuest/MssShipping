using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DacQuest.DFX.Core.Strings;

namespace Mss.Data
{
    public static class MesInterface
    {
        public static bool TryFetchPalletItem(
            string palletID,
            out PalletItem palletItem,
            out string fault)
        {
//             if (!palletIDText.ValidPalletID())
//             {
//                 palletItem = null;
//                 fault = $"Pallet ID {palletIDText} is not a valid.";
//                 XSystemEvent.Publish(
//                     nameof(TryFetchPalletItem),
//                     XSystemEventLevel.Error,
//                     fault + " Operation faulted!");
//                 return false;
//             }
//             int palletID = int.Parse(palletIDText);
//             string connectionString = XConfiguration.GetConnectionString(Constant.SystemConnectionStringName);
//             try
//             {
//                 string sql = $"";
//                 Pallet pallet = null;
//                 using (IDbConnection connection = new SqlConnection(connectionString))
//                 {
//                     PalletRepository palletRepository = new PalletRepository(connection);
//                     pallet = palletRepository
//                         .Find(p => p.PalletID == palletID);
//                     if (pallet == null)
//                     {
//                         palletItem = null;
//                         fault = $"Pallet ID {palletIDText} was not found in {Constant.SystemPalletsTableName}.";
//                         XSystemEvent.Publish(
//                             nameof(TryFetchPalletItem),
//                             XSystemEventLevel.Error,
//                             fault + " Operation faulted!");
//                         return false;
//                     }
//                     palletItem = new PalletItem
//                     {
//                         //                         Identity = pallet.ID,
//                         //                         Location = pallet.Location,
//                         Status = pallet.Damaged ? PalletStatus.QAPick : PalletStatus.OK,
//                         PalletID = pallet.PalletID.ToString(Constant.PalletIDFormat),
//                         Sku = pallet.Sku ?? Constant.EmptyPalletSku,
//                         JobID = pallet.JobID ?? Constant.NoJobID,
//                         BuiltOn = (DateTime)(pallet.BuiltOn == null ? Constant.BeginningOfTime : pallet.BuiltOn),
//                         //                         GroupID = pallet.GroupID,
//                         Damaged = pallet.Damaged,
//                         Comment = pallet.Damaged ? Constant.DamagedPalletComment : string.Empty
//                     };
// 
//                     fault = null;
//                     return true;
//                 }
//             }
//             catch (Exception x)
//             {
//                 fault = $"An exception was thrown while fetching data for Pallet ID {palletIDText} from {Constant.SystemPalletsTableName}.";
//                 XSystemEvent.Publish(
//                     nameof(TryFetchPalletItem),
//                     XSystemEventLevel.Error,
//                     fault + " Operation faulted!");
//                 x.PublishSystemEvent(nameof(TryFetchPalletItem));
//                 fault += " See System Events.";
//                 palletItem = null;
//                 return false;
//             }

            palletItem = null;
            fault = null;
            return false;
        }

    }
}
