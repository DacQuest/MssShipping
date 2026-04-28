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

namespace Mss.Data
{
    public static class MesInterface
    {
        public static string PalletInfoStoredProcedureName = "SHIPSP_Pallet_Info";

        public static bool TryFetchPalletItem(
            OperationCode operationCode,
            string palletID,
            out PalletItem palletItem,
            out string fault)
        {
            return _TryFetchPalletItem(
                operationCode,
                palletID,
                out _,
                out palletItem,
                out fault);
        }

        public static bool TryFetchPalletItemAtAS1andAS2(
            OperationCode operationCode,
            string palletID,
            out bool sendToConsoleArea,
            out PalletItem palletItem,
            out string fault)
        {
            return _TryFetchPalletItem(
                operationCode,
                palletID,
                out sendToConsoleArea,
                out palletItem,
                out fault);
        }

        private static bool _TryFetchPalletItem(
            OperationCode operationCode,
            string palletID,
            out bool sendToConsoleArea,
            out PalletItem palletItem,
            out string fault)
        {
            sendToConsoleArea = false;
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
                using (SqlCommand command = new SqlCommand(PalletInfoStoredProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter opCodeParam = command.Parameters.Add("@Operation", SqlDbType.Int);
                    opCodeParam.Direction = ParameterDirection.Input;
                    opCodeParam.Value = (int)operationCode;

                    SqlParameter palletIDParam = command.Parameters.Add("@PalletID", SqlDbType.VarChar, 10);
                    palletIDParam.Direction = ParameterDirection.Input;
                    palletIDParam.Value = palletID;

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

                    SqlParameter sendToConsoleAreaParam = command.Parameters.Add("@SendToConsoleArea", SqlDbType.Bit);
                    sendToConsoleAreaParam.Direction = ParameterDirection.Output;

                    try
                    {
                        connection.Open();
                        _ = command.ExecuteNonQuery();
                        string sku = (string)skuParam.Value;
                        PalletStatus status = (PalletStatus)statusParam.Value;
                        status = (int)status == -1
                            ? PalletStatus.Invalid
                            : (sku.IsStackSku() && status == PalletStatus.Unknown)
                                ? PalletStatus.OK
                                : status;
                        palletItem = new PalletItem
                        {
                            PalletID = palletID,
                            Status = status,
                            JobID = (string)jobIDParam.Value,
                            Sku = sku,
                            HoldCode = (int)holdCodeParam.Value,
                            BuiltOn = (DateTime)builtOnParam.Value,
                            Comment = (string)commentParam.Value
                        };
                        sendToConsoleArea = (bool)sendToConsoleAreaParam.Value;
                        return true;
                    }
                    catch (Exception x)
                    {
                        fault = "Exception thrown by Stored Procedure!";
                        XSystemEvent.Publish(
                            PalletInfoStoredProcedureName,
                            XSystemEventLevel.Error,
                            fault);
                        x.PublishSystemEvent(PalletInfoStoredProcedureName);
                        return false;
                    }
                }
            }
        }

        public static bool TryFetchBroadcast(
            int maxBroadcastNumbersToFetch,
            string lastCsnReleased,
            int largestRotationReceived,
            out List<BroadcastItem> broadcastItems)
        {
            broadcastItems = new List<BroadcastItem>();

            try
            {
                IEnumerable<BroadcastQueue> pendingItems;
                IEnumerable<BroadcastHeader> headersWithDetails;
                using (IDbConnection connection = _Connection)
                {
                    connection.Open();
                    BroadcastQueueRepository queueRepository = new BroadcastQueueRepository(connection);
                    BroadcastHeaderRepository headerRepository = new BroadcastHeaderRepository(connection);

                    pendingItems = queueRepository
                        .FindAll<BroadcastHeader>(
                            q => !q.Processed,
                            q => q.BroadcastHeader)
                        .Take(maxBroadcastNumbersToFetch);

                    // Get all unique HeaderIDs from the pending items
                    List<int> headerIDs = pendingItems
                        .Where(q => q.BroadcastHeader != null)
                        .Select(q => q.BroadcastHeader.HeaderID)
                        .Distinct()
                        .ToList();

                    // Fetch all relevant headers with their details in one call
                    headersWithDetails = headerRepository
                        .FindAll<BroadcastDetail>(
                            h => headerIDs.Contains(h.HeaderID),
                            h => h.BroadcastDetails);

                    // Build a lookup for fast matching
                    Dictionary<int, BroadcastHeader> headerLookup = headersWithDetails
                        .ToDictionary(h => h.HeaderID);

                    // Assign the populated headers back to each queue item
                    foreach (BroadcastQueue pendingItem in pendingItems)
                    {
                        if (pendingItem.BroadcastHeader != null &&
                            headerLookup.TryGetValue(pendingItem.BroadcastHeader.HeaderID, out BroadcastHeader populated))
                        {
                            pendingItem.BroadcastHeader.BroadcastDetails = populated.BroadcastDetails
                                ?? new List<BroadcastDetail>();
                        }
                    }

                    foreach (BroadcastQueue pendingItem in pendingItems)
                    {
                        BroadcastHeader header = pendingItem.BroadcastHeader;
                        if (header == null)
                        {
                            XSystemEvent.Publish(
                                nameof(MesInterface),
                                XSystemEventLevel.Error,
                                $"MES Broadcast Header ID {pendingItem.HeaderID} has no header data.");
                            continue;
                        }
                        List<BroadcastDetail> details = header.BroadcastDetails;
                        if (details == null
                            || details.Count == 0
                            || details.Count != header.DetailCount)
                        {
                            XSystemEvent.Publish(
                                nameof(MesInterface),
                                XSystemEventLevel.Error,
                                $"MES BroadcastHeader {header.HeaderID} has an invalid detail count.");
                            continue;
                        }

                        bool outOfOrder = false;
                        int lastRotationReleased = BroadcastItem.RotationFromCsn(lastCsnReleased); ;
                        int rotationNumber = header.Rotation;
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
                            if (!BroadcastItem.AutoSkipFromRotation(rotationNumber))
                            {
                                XSystemEvent.Publish(
                                    nameof(MesInterface),
                                    XSystemEventLevel.Warning,
                                    $"Received Rotation Number {rotationNumber} out of order.");
                            }

                            if (rotationNumber > largestRotationReceived)
                            {
                                if (Math.Abs(largestRotationReceived - rotationNumber) > Constant.MaxBroadcastSkip)
                                {
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

                        BroadcastItem row1BroadcastItem = null;
                        BroadcastItem row2BroadcastItem = null;
                        BroadcastDetail detail = header.BroadcastDetails.SingleOrDefault(d => d.VehicleRow == VehicleRow.Row1);
                        if (detail != null)
                        {
                            row1BroadcastItem = new BroadcastItem
                            {
                                Status = BroadcastStatus.OK,
                                Csn = _FormatCsn(header.Rotation, VehicleRow.Row1),
                                VehicleSku = header.VehicleSku,
                                Sku = detail.Sku,
                                Vin = header.Vin,
                                PickMode = detail.PickMode,
                                PickModeKey = detail.PickModeKey,
                                ReceivedOn = DateTime.Now,
                                VehicleRowCount = header.DetailCount
                            };

                            if (header.DetailCount == 2)
                            {
                                detail = header.BroadcastDetails.Single(d => d.VehicleRow == VehicleRow.Row2);
                                if (detail != null)
                                {
                                    row2BroadcastItem = new BroadcastItem
                                    {
                                        Status = BroadcastStatus.OK,
                                        Csn = _FormatCsn(header.Rotation, VehicleRow.Row2),
                                        VehicleSku = header.VehicleSku,
                                        Sku = detail.Sku,
                                        Vin = header.Vin,
                                        PickMode = detail.PickMode,
                                        PickModeKey = detail.PickModeKey,
                                        ReceivedOn = DateTime.Now,
                                        VehicleRowCount = header.DetailCount
                                    };
                                    broadcastItems.Add(row2BroadcastItem);
                                }
                                else
                                {
                                    XSystemEvent.Publish(
                                        nameof(TryFetchBroadcast),
                                        XSystemEventLevel.Error,
                                        $"HeaderID {header.HeaderID} does not have Row 2 Details in the SHIPBroadcastDetails table.");
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            XSystemEvent.Publish(
                                nameof(TryFetchBroadcast),
                                XSystemEventLevel.Error,
                                $"HeaderID {header.HeaderID} does not have Row 1 Details in the SHIPBroadcastDetails table.");
                            return true;
                        }
                        broadcastItems.Add(row1BroadcastItem);
                        if (row2BroadcastItem != null)
                        {
                            broadcastItems.Add(row2BroadcastItem);
                        }
                        if (row1BroadcastItem.Rotation > largestRotationReceived)
                        {
                            largestRotationReceived = row1BroadcastItem.Rotation;
                        }
                        pendingItem.Processed = true;
                        pendingItem.ProcessedOn = DateTime.Now;
                        _ = queueRepository.Update(pendingItem);
                    }
                }
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(TryFetchBroadcast));
                return false;
            }
        }

        //         public static bool TryFetchBroadcast(
        //             int maxBroadcastNumbersToFetch,
        //             string lastCsnReleased,
        //             int largestRotationReceived,
        //             out List<BroadcastItem> broadcastItems)
        //         {
        //             broadcastItems = new List<BroadcastItem>();
        // 
        //             BroadcastHeaderRepository broadcastRepository = new BroadcastHeaderRepository(_Connection);
        //             if (!broadcastRepository.TryFetchRawBroadcasts(out IEnumerable<BroadcastQueue> broadcastEntries))
        //             {
        //                 broadcastItems = null;
        //                 return false;
        //             }
        // 
        //             foreach (BroadcastQueue broadcastEntry in broadcastEntries.Take(maxBroadcastNumbersToFetch))
        //             {
        //                 BroadcastHeader broadcastHeader = broadcastEntry.BroadcastHeader;
        //                 if (broadcastHeader == null)
        //                 {
        //                     XSystemEvent.Publish(
        //                         nameof(MesInterface),
        //                         XSystemEventLevel.Error,
        //                         $"MES Broadcast Header ID {broadcastEntry.HeaderID} has no header data.");
        // //                     _ = broadcastRepository.Delete(rawBroadcast);
        //                     continue;
        //                 }
        //                 List<BroadcastDetail> broadcastDetails = broadcastHeader.BroadcastDetails;
        //                 if (broadcastDetails == null
        //                     || broadcastDetails.Count == 0
        //                     || broadcastDetails.Count != broadcastHeader.RowCount)
        //                 {
        //                     XSystemEvent.Publish(
        //                         nameof(MesInterface),
        //                         XSystemEventLevel.Error,
        //                         $"MES BroadcastHeader {broadcastHeader.HeaderID} has an invalid detail count.");
        // //                     _ = broadcastRepository.Delete(rawBroadcast);
        //                     continue;
        //                 }
        // 
        //                 bool outOfOrder = false;
        //                 int lastRotationReleased = BroadcastItem.RotationFromCsn(lastCsnReleased); ;
        //                 int rotationNumber = broadcastHeader.Rotation;
        //                 if (rotationNumber != largestRotationReceived + 1)
        //                 {
        //                     if (rotationNumber <= lastRotationReleased)
        //                     {
        //                         XSystemEvent.Publish(
        //                             nameof(MesInterface),
        //                             XSystemEventLevel.Warning,
        //                             $"Received a Rotation Number {rotationNumber} which is smaller than or equal to the rotation of the LastCsnReleased {lastCsnReleased}. Broadcast record discarded.");
        //                         continue;
        //                     }
        //                     outOfOrder = true;
        // //                     int modRotationNumber = rotationNumber % 10000;
        // //                     if (modRotationNumber != 0 && modRotationNumber <= Constant.MaxRotation)
        //                     if (!BroadcastItem.AutoSkipFromRotation(rotationNumber))
        //                     {
        //                         XSystemEvent.Publish(
        //                             nameof(MesInterface),
        //                             XSystemEventLevel.Warning,
        //                             $"Received Rotation Number {rotationNumber} out of order.");
        //                     }
        // //                     else
        // //                     {
        // //                         XSystemEvent.Publish(
        // //                             nameof(MesInterface),
        // //                             XSystemEventLevel.Notification,
        // //                             $"Millionth Broadcast Number Boundary Crossed ({broadcastNumber}).");
        // //                     }
        // 
        //                     if (rotationNumber > largestRotationReceived)
        //                     {
        //                         if (Math.Abs(largestRotationReceived - rotationNumber) > Constant.MaxBroadcastSkip)
        //                         {
        // //                             broadcastRepository.MarkAsProcessed(rawBroadcast);
        //                             XSystemEvent.Publish(
        //                                 nameof(MesInterface),
        //                                 XSystemEventLevel.Error,
        //                                 $"Received Rotation Number {rotationNumber} which is more than {Constant.MaxBroadcastSkip} larger than Largest Rotation Received. Broadcast record not processed.");
        //                             return broadcastItems.Count > 0;
        //                         }
        //                     }
        //                 }
        // 
        //                 if (outOfOrder)
        //                 {
        //                     int startRotation = largestRotationReceived + 1;
        //                     int endRotation = rotationNumber;
        //                     for (int missingRotationNumber = startRotation;
        //                         missingRotationNumber < endRotation;
        //                         missingRotationNumber++)
        //                     {
        //                         BroadcastStatus status = BroadcastStatus.Missing;
        // //                         int mod = missingRotationNumber % 10000;
        // //                         if (mod == 0 || mod > Constant.MaxRotation)
        //                         if (BroadcastItem.AutoSkipFromRotation(missingRotationNumber))
        //                         {
        //                             status = BroadcastStatus.Skip;
        //                         }
        // 
        //                         BroadcastItem missingBroadcastItem = new BroadcastItem
        //                         {
        //                             Status = status,
        //                             Csn = _FormatCsn(missingRotationNumber, VehicleRow.Row1),
        //                             VehicleSku = string.Empty,
        //                             Sku = string.Empty,
        //                             Vin = string.Empty,
        //                             PickMode = PickMode.BySku,
        //                             PickModeKey = string.Empty,
        //                             ReceivedOn = DateTime.Now,
        //                             VehicleRowCount = 0
        //                         };
        //                         broadcastItems.Add(missingBroadcastItem);
        //                     }
        //                 }
        // 
        //                 BroadcastHeader header = broadcastEntry.BroadcastHeader;
        //                 BroadcastDetail detail = header.BroadcastDetails.SingleOrDefault(d => d.VehicleRow == VehicleRow.Row1);
        //                 if (detail != null)
        //                 {
        //                     BroadcastItem row1BroadcastItem = new BroadcastItem
        //                     {
        //                         Status = BroadcastStatus.OK,
        //                         Csn = _FormatCsn(header.Rotation, VehicleRow.Row1),
        //                         VehicleSku = header.VehicleSku ?? string.Empty,
        //                         Sku = detail.Sku ?? string.Empty,
        //                         Vin = header.Vin ?? string.Empty,
        //                         PickMode = detail.PickMode,
        //                         PickModeKey = detail.PickModeKey ?? string.Empty,
        //                         ReceivedOn = DateTime.Now,
        //                         VehicleRowCount = header.RowCount
        //                     };
        //                     broadcastItems.Add(row1BroadcastItem);
        // 
        //                     if (broadcastEntry.BroadcastHeader.RowCount == 2)
        //                     {
        //                         detail = header.BroadcastDetails.Single(d => d.VehicleRow == VehicleRow.Row2);
        //                         if (detail != null)
        //                         {
        //                             BroadcastItem row2BroadcastItem = new BroadcastItem
        //                             {
        //                                 Status = BroadcastStatus.OK,
        //                                 Csn = _FormatCsn(header.Rotation, VehicleRow.Row2),
        //                                 VehicleSku = header.VehicleSku,
        //                                 Sku = detail.Sku,
        //                                 Vin = header.Vin,
        //                                 PickMode = detail.PickMode,
        //                                 PickModeKey = detail.PickModeKey,
        //                                 ReceivedOn = DateTime.Now,
        //                                 VehicleRowCount = header.RowCount
        //                             };
        //                             broadcastItems.Add(row2BroadcastItem);
        //                         }
        //                         else
        //                         {
        //                             //ERROR  No row 2
        //                         }
        //                     }
        //                 }
        //                 else
        //                 {
        //                     //ERROR No row 1
        //                 }
        //                 if (error || !_ValidateBroadcast(header.RowCount, row1BroadcastItem, row2BroadcastItem))
        //                 {
        // 
        //                 }
        // 
        // 
        // 
        // 
        // 
        //                 if (row1BroadcastItem.Rotation > largestRotationReceived)
        //                 {
        //                     largestRotationReceived = row1BroadcastItem.Rotation;
        //                 }
        //                 _ = broadcastRepository.TryMarkAsProcessed(broadcastEntry);
        //             }
        //             return broadcastItems.Count > 0;
        // 
        //         }

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
                holdCodes.Add(new HoldCodeItem
                {
                    HoldCode = Constant.NoHoldCode,
                    Description = Constant.NoHoldCodeDescription
                });
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(TryFetchHoldCodes));
                holdCodes = null;
                return false;
            }
        }

        public static bool TryFetchPendingStatusChangeRequests(
            out IEnumerable<StatusChangeQueue> pendingRequests)
        {
            try
            {
                StatusChangeQueueRepository repository = new StatusChangeQueueRepository(_Connection);
                return repository.TryFetchStatusChangeRequests(
                    out pendingRequests);
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(TryFetchPendingStatusChangeRequests));
                pendingRequests = null;
                return false;
            }
        }

        public static bool UpdateProcessedStatusChangeRequests(
            IEnumerable<StatusChangeQueue> processedRequests)
        {
            try
            {
                StatusChangeQueueRepository repository = new StatusChangeQueueRepository(_Connection);
                return repository.TryUpdateProcessedStatusChangeRequests(processedRequests);
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(UpdateProcessedStatusChangeRequests));
                return false;
            }
        }

        private static SqlConnection _Connection => new SqlConnection(
            XConfiguration.GetConnectionString(Constant.MesConnectionStringName));

    }
}
