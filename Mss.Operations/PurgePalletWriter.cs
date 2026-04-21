using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public static class PurgePalletWriter
    {
        public static  void WritePurgePallet(PalletItem palletItem)
        {
            try
            {
                string sql = string.Format(
                    //                     @"INSERT INTO PurgePallets (PurgedOn,PalletID,JobID,Comment,) VALUES ('{0}','{1}','{2}','{3}'); SELECT Convert(Int, SCOPE_IDENTITY());",
                    @"INSERT INTO {0} (PurgedOn,PalletID,Sku,  JobID, HoldCode,BuiltOn, Comment,)
                      VALUES          ('{1}',   '{2}',   '{3}','{4}', {5},     {6},     '{7}');",
                    Constant.PurgePalletsTableName,
                    DateTime.Now.ToString(Constant.LongDateTimeFormat24),
                    palletItem.PalletID,
                    palletItem.Sku,
                    palletItem.JobID,
                    palletItem.HoldCode,
                    palletItem.BuiltOnText,
                    palletItem.Comment);

                string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    _ = command.ExecuteNonQuery();
//                     int PickID = (int)command.ExecuteScalar();
//                     sql = $"INSERT INTO Pallet_Purge_Queue (Pick_ID) VALUES ({PickID});";
//                     command.CommandText = sql;
//                     command.ExecuteNonQuery();
                }
                XSystemEvent.Publish(
                    nameof(WritePurgePallet),
                    XSystemEventLevel.Notification,
                    $"Purge Pallet written to {Constant.PurgePalletsTableName} table: PalletID={palletItem.PalletID}, JobID={palletItem.JobID}");
            }
            catch (Exception x)
            {
                XSystemEvent.Publish(
                    "PurgePalletDataWriter",
                    XSystemEventLevel.Error,
                    $"Exception thrown while sending Purge Pallet Data to MES:  PalletID={palletItem.PalletID}; JobID={palletItem.JobID}; Reason={palletItem.Comment}");
                x.PublishSystemEvent("PurgePalletDataWriter");
            }
        }
    }
}
