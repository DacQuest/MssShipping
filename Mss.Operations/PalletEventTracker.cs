using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core;
using Mss.Collections;
using Mss.Common;
using System.Security.Cryptography;
using Mss.Operations;

namespace Mss.Operations
{
    public static class PalletEventTracker
    {

        public static void Capture(
            string palletID,
            PalletItem palletItem,
            OperationCode operationCode,
            PalletEvent palletEvent)
        {
            if (palletItem == null)
            {
                Capture(
                    palletID,
                    operationCode,
                    palletEvent);
            }
            else
            {
                Capture(
                    palletItem,
                    operationCode,
                    palletEvent);
            }
        }

        public static void Capture(
            string palletID,
            PalletItem palletItem,
            OperationCode operationCode,
            PalletEvent palletEvent,
            int moveCommand)
        {
            if (palletItem == null)
            {
                Capture(
                    palletID,
                    operationCode,
                    palletEvent,
                    moveCommand);
            }
            else
            {
                Capture(
                    palletItem,
                    operationCode,
                    palletEvent,
                    moveCommand);
            }
        }

        public static void Capture(
            string palletID,
            OperationCode operationCode,
            PalletEvent palletEvent)
        {
            Capture(
                palletID,
                PalletStatus.Invalid,
                string.Empty,
                Constant.NoJobID,
                string.Empty,
                operationCode,
                palletEvent,
                0);
        }

        public static void Capture(
            string palletID,
            OperationCode operationCode,
            PalletEvent palletEvent,
            int moveCommand)
        {
            Capture(
                palletID,
                PalletStatus.Invalid,
                string.Empty,
                Constant.NoJobID,
                string.Empty,
                operationCode,
                palletEvent,
                moveCommand);
        }

        public static void Capture(
            PalletItem palletItem,
            OperationCode operationCode,
            PalletEvent palletEvent)
        {
            Capture(
                palletItem,
                operationCode,
                palletEvent,
                0);
        }

        public static void Capture(
            PalletItem palletItem,
            OperationCode operationCode,
            PalletEvent palletEvent,
            int moveCommand)
        {
            if (palletItem == null)
            {
                XSystemEvent.Publish(
                    "PalletEventTracker",
                    XSystemEventLevel.Warning,
                    $"Received Pallet Event '{palletEvent.ToText()}' at {operationCode.ToText()}, but the Pallet Item was null.");
                return;
            }
            Capture(
                palletItem.PalletID,
                palletItem.Status,
                palletItem.Sku,
                palletItem.JobID,
                palletItem.Comment,
                operationCode,
                palletEvent,
                moveCommand);

        }
        public static void Capture(
            string palletID,
            PalletStatus palletStatus,
            string sku,
            string jobID,
            string comment,
            OperationCode operationCode,
            PalletEvent palletEvent,
            int moveCommand)
        {
            if (!palletID.ValidPalletID())
            {
                palletID = "????";
            }
            string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = string.Format(
                        @"INSERT INTO [PalletEventTracker] ([OccurredOn],[PalletID],[Sku],[JobID],[PalletStatus],[PalletStatusText],[OperationCode],[OperationCodeText],[PalletEvent],[PalletEventText],[MoveCommand],[Comment])
                                                    VALUES ('{0}',       '{1}',     '{2}','{3}'   {4},           '{5}',             {6},            '{7}',              {8},          '{9}',            {10},         '{11}');",
                        DateTime.Now.ToString(Constant.DateTimeFormat),
                        palletID,
                        sku,
                        jobID,
                        palletStatus.ToString(),
                        (int)palletStatus,
                        operationCode.ToText(),
                        (int)operationCode,
                        palletEvent.ToText(),
                        (int)palletEvent,
                        moveCommand,
                        comment);

                    SqlCommand command = new SqlCommand(sql, connection);
                    _ = command.ExecuteNonQuery();

                }
            }
            catch (Exception x)
            {
                x.PublishSystemEvent("PalletEventTracker");
            }
        }
    }
}
