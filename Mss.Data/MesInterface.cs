using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Collections;
using Mss.Common;
using Mss.Data.Pocos;
using Mss.Data.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mss.Data
{
    public static class MesInterface
    {
        public static string FetchPalletStoredProcedureName = "SHIPSP_Pallet_Info";

        public static bool TryFetchPalletItem(
            OperationCode operationCode,
            string palletID,
            out PalletItem palletItem,
            out string fault)
        {
            return TryFetchPalletItem(
                operationCode,
                palletID,
                false,
                out _,
                out palletItem,
                out fault);
        }
        public static bool TryFetchPalletItem(
            OperationCode operationCode,
            string palletID,
            bool requestDestination,
            out PalletDestination destination,
            out PalletItem palletItem,
            out string fault)
        {
            destination = PalletDestination.None;
            palletItem = null;
            fault = string.Empty;

            if (!palletID.ValidPalletID())
            {
                fault = $"Pallet ID {palletID} is not a valid.";
                XSystemEvent.Publish(
                    nameof(TryFetchPalletItem),
                    XSystemEventLevel.Error,
                    fault + " Operation faulted!");
                return false;
            }

            using (SqlConnection connection = _Connection)
            {
                using (SqlCommand command = new SqlCommand(FetchPalletStoredProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter opCodeParam = command.Parameters.Add("@Operation", SqlDbType.Int);
                    opCodeParam.Direction = ParameterDirection.Input;
                    opCodeParam.Value = (int)operationCode;

                    SqlParameter palletIDParam = command.Parameters.Add("@PalletID", SqlDbType.VarChar, 10);
                    palletIDParam.Direction = ParameterDirection.Input;
                    palletIDParam.Value = palletID;

                    SqlParameter destinationRequestParam = command.Parameters.Add("@DestinationRequest", SqlDbType.Bit);
                    destinationRequestParam.Direction = ParameterDirection.Input;
                    destinationRequestParam.Value = requestDestination;

                    SqlParameter jobIDParam = command.Parameters.Add("@JobID", SqlDbType.VarChar, 50);
                    jobIDParam.Direction = ParameterDirection.Output;

                    SqlParameter skuParam = command.Parameters.Add("@SKU", SqlDbType.VarChar, 50);
                    skuParam.Direction = ParameterDirection.Output;

                    SqlParameter builtOnParam = command.Parameters.Add("@BuildDTTM", SqlDbType.DateTime);
                    builtOnParam.Direction = ParameterDirection.Output;

                    SqlParameter holdCodeParam = command.Parameters.Add("@HoldCode", SqlDbType.Int);
                    holdCodeParam.Direction = ParameterDirection.Output;

                    SqlParameter statusParam = command.Parameters.Add("@PalletStatus", SqlDbType.Int);
                    statusParam.Direction = ParameterDirection.Output;

                    SqlParameter commentParam = command.Parameters.Add("@Comment", SqlDbType.VarChar, 50);
                    commentParam.Direction = ParameterDirection.Output;

                    SqlParameter destinationParam = command.Parameters.Add("@DestinationStatus", SqlDbType.Int);
                    destinationParam.Direction = ParameterDirection.Output;

                    try
                    {
                        connection.Open();
                        _ = command.ExecuteNonQuery();
                        PalletStatus status = (int)statusParam.Value == -1
                            ? PalletStatus.Invalid
                            : (PalletStatus)statusParam.Value;
                        palletItem = new PalletItem
                        {
                            PalletID = palletID,
                            Status = status,
                            JobID = (string)jobIDParam.Value,
                            Sku = (string)skuParam.Value,
                            HoldCode = (int)holdCodeParam.Value,
                            BuiltOn = (DateTime)builtOnParam.Value,
                            Comment = (string)commentParam.Value
                        };
                        destination = (PalletDestination)destinationParam.Value;
                        return true;
                    }
                    catch (Exception x)
                    {
                        fault = "Exception thrown by Stored Procedure!";
                        XSystemEvent.Publish(
                            FetchPalletStoredProcedureName,
                            XSystemEventLevel.Error,
                            fault);
                        x.PublishSystemEvent(FetchPalletStoredProcedureName);
                        return false;
                    }
                }
            }
        }

        // Destructive! Cannot be called from anywhere but MesDataService._ProcessBroadcast()
        public static bool TryFetchBroadcast(
            int maxBroadcastNumbersToFetch,
            string lastCsnReleased,
            int largestRotationReceived,
            out List<BroadcastItem> broadcastItems)
        {
            broadcastItems = new List<BroadcastItem>();

            BroadcastRepository broadcastRepository = new BroadcastRepository(_Connection);
            if (!broadcastRepository.TryFetchRawBroadcasts(out IEnumerable<BroadcastQueue> rawBroadcasts))
            {
                broadcastItems = null;
                return false;
            }

            foreach (BroadcastQueue rawBroadcast in rawBroadcasts.Take(maxBroadcastNumbersToFetch))
            {
                BroadcastHeader broadcastHeader = rawBroadcast.BroadcastHeader;
                if (broadcastHeader == null)
                {
                    XSystemEvent.Publish(
                        nameof(MesInterface),
                        XSystemEventLevel.Error,
                        $"MES Broadcast Header ID {rawBroadcast.HeaderID} has no header data.");
//                     _ = broadcastRepository.Delete(rawBroadcast);
                    continue;
                }
                List<BroadcastDetail> broadcastDetails = broadcastHeader.BroadcastDetails;
                if (broadcastDetails == null
                    || broadcastDetails.Count == 0
                    || broadcastDetails.Count != broadcastHeader.RowCount)
                {
                    XSystemEvent.Publish(
                        nameof(MesInterface),
                        XSystemEventLevel.Error,
                        $"MES BroadcastHeader {broadcastHeader.HeaderID} has an invalid detail count.");
//                     _ = broadcastRepository.Delete(rawBroadcast);
                    continue;
                }

                bool outOfOrder = false;
                int lastRotationReleased = BroadcastItem.RotationFromCsn(lastCsnReleased); ;
                int rotationNumber = broadcastHeader.Rotation;
                if (rotationNumber != largestRotationReceived + 1)
                {
                    if (rotationNumber <= lastRotationReleased)
                    {
                        XSystemEvent.Publish(
                            nameof(MesInterface),
                            XSystemEventLevel.Warning,
                            $"Received a Rotation Number {rotationNumber} which is smaller than or equal to the rotation of the LastCsnReleased {lastCsnReleased}. Broadcast record discarded.");
                        continue;
                    }
                    outOfOrder = true;
//                     int modRotationNumber = rotationNumber % 10000;
//                     if (modRotationNumber != 0 && modRotationNumber <= Constant.MaxRotation)
                    if (!BroadcastItem.AutoSkipFromRotation(rotationNumber))
                    {
                        XSystemEvent.Publish(
                            nameof(MesInterface),
                            XSystemEventLevel.Warning,
                            $"Received Rotation Number {rotationNumber} out of order.");
                    }
//                     else
//                     {
//                         XSystemEvent.Publish(
//                             nameof(MesInterface),
//                             XSystemEventLevel.Notification,
//                             $"Millionth Broadcast Number Boundary Crossed ({broadcastNumber}).");
//                     }

                    if (rotationNumber > largestRotationReceived)
                    {
                        if (Math.Abs(largestRotationReceived - rotationNumber) > Constant.MaxBroadcastSkip)
                        {
//                             broadcastRepository.MarkAsProcessed(rawBroadcast);
                            XSystemEvent.Publish(
                                nameof(MesInterface),
                                XSystemEventLevel.Error,
                                $"Received Rotation Number {rotationNumber} which is more than {Constant.MaxBroadcastSkip} larger than Largest Rotation Received. Broadcast record not processed.");
                            return broadcastItems.Count > 0;
                        }
                    }
                }

                if (outOfOrder)
                {
                    int startRotation = largestRotationReceived + 1;
                    int endRotation = rotationNumber;
                    for (int missingRotationNumber = startRotation;
                        missingRotationNumber < endRotation;
                        missingRotationNumber++)
                    {
                        BroadcastStatus status = BroadcastStatus.Missing;
//                         int mod = missingRotationNumber % 10000;
//                         if (mod == 0 || mod > Constant.MaxRotation)
                        if (BroadcastItem.AutoSkipFromRotation(missingRotationNumber))
                        {
                            status = BroadcastStatus.Skip;
                        }

                        BroadcastItem missingBroadcastItem = new BroadcastItem
                        {
                            Status = status,
                            Csn = _FormatCsn(missingRotationNumber, VehicleRow.Row1),
                            VehicleSku = string.Empty,
                            Sku = string.Empty,
                            Vin = string.Empty,
                            PickMode = PickMode.BySku,
                            PickModeKey = string.Empty,
                            ReceivedOn = DateTime.Now,
                            VehicleRowCount = 0
                        };
                        broadcastItems.Add(missingBroadcastItem);
                    }
                }

                BroadcastHeader header = rawBroadcast.BroadcastHeader;
                BroadcastDetail detail = header.BroadcastDetails.Single(d => d.VehicleRow == VehicleRow.Row1);
                
                BroadcastItem row1BroadcastItem = new BroadcastItem
                {
                    Status = BroadcastStatus.OK,
                    Csn = _FormatCsn(header.Rotation, VehicleRow.Row1),
                    VehicleSku = header.VehicleSku,
                    Sku = detail.Sku,
                    Vin = header.Vin,
                    PickMode = detail.PickMode,
                    PickModeKey = detail.PickModeKey,
                    ReceivedOn = DateTime.Now,
                    VehicleRowCount = header.RowCount
                };
                broadcastItems.Add(row1BroadcastItem);

                if (rawBroadcast.BroadcastHeader.RowCount == 2)
                {
                    detail = header.BroadcastDetails.Single(d => d.VehicleRow == VehicleRow.Row2);
                    BroadcastItem row2BroadcastItem = new BroadcastItem
                    {
                        Status = BroadcastStatus.OK,
                        Csn = _FormatCsn(header.Rotation, VehicleRow.Row2),
                        VehicleSku = header.VehicleSku,
                        Sku = detail.Sku,
                        Vin = header.Vin,
                        PickMode = detail.PickMode,
                        PickModeKey = detail.PickModeKey,
                        ReceivedOn = DateTime.Now,
                        VehicleRowCount = header.RowCount
                    };
                    broadcastItems.Add(row2BroadcastItem);
                }
                if (row1BroadcastItem.Rotation > largestRotationReceived)
                {
                    largestRotationReceived = row1BroadcastItem.Rotation;
                }
                _ = broadcastRepository.TryMarkAsProcessed(rawBroadcast);
            }
            return broadcastItems.Count > 0;

        }

        private static string _FormatCsn(int rotationNumber, VehicleRow vehicleRow)
        {
            string suffix = vehicleRow == VehicleRow.Row1
                ? Constant.VehicleRow1CsnSuffix
                : Constant.VehicleRow2CsnSuffix;

            return BroadcastItem.MakeCsn(rotationNumber, suffix);
        }

        public static bool TryFetchHoldCodes(out List<HoldCodeItem> holdCodes)
        {
            try
            {
                HoldCodesRepository holdCodesRepository = new HoldCodesRepository(_Connection);
                if (!holdCodesRepository.TryFetchRawHoldCodes(out IEnumerable<HoldCode> rawHoldCodes))
                {
                    holdCodes = null;
                    return false;
                }

                holdCodes = rawHoldCodes
                    .Select(r =>
                        new HoldCodeItem
                        {
                            HoldCode = r.Code,
                            Description = r.Description
                        })
                    .ToList();
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(TryFetchHoldCodes));
                holdCodes = null;
                return false;
            }
        }

//         public bool TryFetchPalletStatusChangeRequests(
//             out IEnumerable<PalletStatusChangeRequest> requests,
//             out string fault)
//         {
//             try
//             {
//                 HoldCodesRepository holdCodesRepository = new HoldCodesRepository(_Connection);
//                 if (!holdCodesRepository.TryFetchRawHoldCodes(out IEnumerable<HoldCode> rawHoldCodes))
//                 {
//                     holdCodes = null;
//                     return false;
//                 }
// 
//                 holdCodes = rawHoldCodes
//                     .Select(r =>
//                         new HoldCodeItem
//                         {
//                             HoldCode = r.Code,
//                             Description = r.Description
//                         })
//                     .ToList();
//                 return true;
//             }
//             catch (Exception x)
//             {
//                 x.PublishSystemEvent(nameof(TryFetchHoldCodes));
//                 holdCodes = null;
//                 return false;
//             }
//         }

        private static SqlConnection _Connection => new SqlConnection(
            XConfiguration.GetConnectionString(Constant.MesConnectionStringName));

    }
}
