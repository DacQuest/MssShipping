using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
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
using System.Threading.Tasks;

namespace Mss.Data
{
    public static class MesInterface
    {
        public static string FetchPalletStoredProcedureName = "SHIPSP_Pallet_Info";

        public static bool TryFetchPalletItem(
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

                    SqlParameter palletIDParam = command.Parameters.Add("@PalletID", SqlDbType.VarChar, 10);
                    palletIDParam.Direction = ParameterDirection.Input;
                    palletIDParam.Value = palletID;

                    SqlParameter destinationRequestParam = command.Parameters.Add("@DestinationRequest", SqlDbType.Bit);
                    destinationRequestParam.Direction = ParameterDirection.Input;
                    destinationRequestParam.Value = requestDestination;

                    SqlParameter jobIDParam = command.Parameters.Add("@JobID", SqlDbType.Int);
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
                        palletItem = new PalletItem
                        {
                            PalletID = palletID,
                            Status = (PalletStatus)statusParam.Value,
                            JobID = (int)jobIDParam.Value,
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
        internal async static Task<bool> FetchBroadcast(
            int maxBroadcastNumbersToFetch,
            int lastRotationReleased,
            Ref<int> largestRotationReceived,
            Out<List<BroadcastItem>> broadcastItems)
        {
            broadcastItems.Value = new List<BroadcastItem>();



            BroadcastRepository broadcastRepository = new BroadcastRepository(_Connection);
            Out<IEnumerable<BroadcastQueue>> rawBroadcasts = new Out<IEnumerable<BroadcastQueue>>();
            if (!await broadcastRepository.FetchRawBroadcastsAsync(rawBroadcasts))
            {
                return false;
            }


//             using (SqlConnection connection = _Connection)
//             {
//                 BroadcastRepository broadcastRepository = new BroadcastRepository(connection);
//                 if (!broadcastRepository.FetchRawBroadcast(out List<Broadcast_Queue> broadcastQueue))
//                 {
//                     return false;
//                 }
            foreach (BroadcastQueue rawBroadcast in rawBroadcasts.Value)
            {
                BroadcastHeader broadcastHeader = rawBroadcast.BroadcastHeader;
                if (broadcastHeader == null)
                {
                    XSystemEvent.Publish(
                        nameof(MesInterface),
                        XSystemEventLevel.Error,
                        $"MES Broadcast Header ID {rawBroadcast.HeaderID} has no header data.");
                    _ = broadcastRepository.Delete(rawBroadcast);
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
                    broadcastRepository.Delete(rawBroadcast);
                    continue;
                }

                bool outOfOrder = false;
                int rotationNumber = broadcastHeader.Rotation;
                if (rotationNumber != largestRotationReceived.Value + 1)
                {
                    if (rotationNumber < lastRotationReleased)
                    {
                        XSystemEvent.Publish(
                            nameof(MesInterface),
                            XSystemEventLevel.Warning,
                            $"Received Rotation Number {rotationNumber} which is smaller than LastRotationReleased {lastRotationReleased}. Discarding broadcast record.");
                        continue;
                    }
                    outOfOrder = true;
                    int modRotationNumber = rotationNumber % 10000;
                    if (modRotationNumber != 0 && modRotationNumber != 9999)
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

                    if (rotationNumber > largestRotationReceived.Value)
                    {
                        if (Math.Abs(largestRotationReceived.Value - rotationNumber) > Constant.MaxBroadcastSkip)
                        {
                            broadcastRepository.MarkAsProcessed(rawBroadcast);
                            XSystemEvent.Publish(
                                nameof(MesInterface),
                                XSystemEventLevel.Error,
                                $"Received Rotation Number {rotationNumber} which is more than {Constant.MaxBroadcastSkip} larger than Largest Rotation Received. Broadcast record not processed.");
                            return broadcastItems.Value.Count > 0;
                        }
                    }
                }

                if (outOfOrder)
                {
                    int startRotation = largestRotationReceived.Value + 1;
                    int endRotation = rotationNumber;
                    for (int missingRotationNumber = startRotation;
                        missingRotationNumber < endRotation;
                        missingRotationNumber += 1)
                    {
                        BroadcastStatus status = BroadcastStatus.Missing;
                        int modRotationNumber = rotationNumber % 10000;
                        if (modRotationNumber == 0 || modRotationNumber == 9999)
                        {
                            status = BroadcastStatus.Skip;
                        }

                        BroadcastItem missingBroadcastItem = new BroadcastItem
                        {
                            Status = status,
                            Csn = $"{missingRotationNumber.ToString(Constant.BroadcastNumberTextFormat)}{Constant.CsnDelimiter}{(int)VehicleRow.Row1}",
                            InternalSequenceNumber = Broadcast.GetRow1InternalSequenceNumber(missingRotationNumber),
                            ReceivedOn = DateTime.Now,
                            ModelCode = string.Empty
                        };
                        broadcastItems.Add(missingBroadcastItem);

                        BroadcastItem missing2ndBroadcastItem = new BroadcastItem
                        {
                            Status = status,
                            Csn = $"{missingRotationNumber.ToString(Constant.BroadcastNumberTextFormat)}{Constant.CsnDelimiter}{(int)VehicleRow.Row2}",
                            InternalSequenceNumber = Broadcast.GetRow2InternalSequenceNumber(missingRotationNumber),
                            ReceivedOn = DateTime.Now,
                            ModelCode = string.Empty
                        };
                        broadcastItems.Add(missing2ndBroadcastItem);
                    }
                }

                string modelCode = string.Empty;
                if (!string.IsNullOrWhiteSpace(broadcastHeader.Model_Code))
                {
                    modelCode = broadcastHeader.Model_Code;
                }
                BroadcastItem row1BroadcastItem = new BroadcastItem
                {
                    Csn = $"{broadcastNumber.ToString(Constant.BroadcastNumberTextFormat)}{Constant.CsnDelimiter}{(int)VehicleRow.Row1}",
                    InternalSequenceNumber = Broadcast.GetRow1InternalSequenceNumber(broadcastNumber),
                    Status = BroadcastStatus.OK,
                    Sku = broadcastDetails
                        .Where(bd => bd.Item_Type == Constant.Row1SkuCode)
                        .Select(bd => bd.Item_Nbr)
                        .FirstOrDefault(),
                    Vin = broadcastHeader.VIN_Ref_Nbr,
                    ReceivedOn = DateTime.Now,
                    ModelCode = modelCode
                };

                string row2SkuCode = Constant.Row2SkuCode;
                if (string.IsNullOrWhiteSpace(row1BroadcastItem.Sku))
                {
                    row1BroadcastItem.Sku = string.Empty;
                    row1BroadcastItem.Status = BroadcastStatus.Missing;
                }
                else
                {
                    string sku3rd = broadcastDetails
                        .Where(bd => bd.Item_Type == Constant.Row3SkuCode)
                        .Select(bd => bd.Item_Nbr)
                        .FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(sku3rd))
                    {
                        // If SKU3 is present and ModelCode ends in 74,
                        // the 3rd row sku is actually the second row sku.
                        // Here we tell the code below to use SKU3 to
                        // acquire the second row sku.
                        if (row1BroadcastItem.ModelCode.Right(2) == "74")
                        {
                            row2SkuCode = Constant.Row3SkuCode;
                        }
                        else
                        {
                            row1BroadcastItem.Sku3rd = sku3rd;
                        }
                    }
                }

                BroadcastItem row2BroadcastItem = new BroadcastItem
                {
                    Csn = $"{broadcastNumber.ToString(Constant.BroadcastNumberTextFormat)}{Constant.CsnDelimiter}{(int)VehicleRow.Row2}",
                    InternalSequenceNumber = Broadcast.GetRow2InternalSequenceNumber(broadcastNumber),
                    Status = BroadcastStatus.OK,
                    Sku = broadcastDetails
                        //                             .Where(bd => bd.Item_Type == Constant.Row2SkuCode)
                        .Where(bd => bd.Item_Type == row2SkuCode)
                        .Select(bd => bd.Item_Nbr)
                        .FirstOrDefault(),
                    Row2ConsolePart = broadcastDetails
                        .Where(bd => bd.Item_Type == Constant.Row2ConsoleCode)
                        .Select(bd => bd.Item_Nbr)
                        .FirstOrDefault(),
                    Vin = broadcastHeader.VIN_Ref_Nbr,
                    ReceivedOn = DateTime.Now,
                    ModelCode = modelCode
                };
                if (string.IsNullOrWhiteSpace(row2BroadcastItem.Sku))
                {
                    row2BroadcastItem.Sku = string.Empty;
                    row2BroadcastItem.Status = BroadcastStatus.Missing;
                }

                broadcastItems.Add(row1BroadcastItem);
                broadcastItems.Add(row2BroadcastItem);
                if (row1BroadcastItem.BroadcastNumber > largestRotationReceived)
                {
                    largestRotationReceived = row1BroadcastItem.BroadcastNumber;
                }

                broadcastRepository.Delete(rawBroadcast);
            }
//             }
            return true;

        }
//         private async static Task<IEnumerable<BroadcastQueue>> _FetchRawBroadcast()
//         {
//             BroadcastRepository broadcastRepository = new BroadcastRepository(_Connection);
//             return await broadcastRepository.FetchRawBraodcastAsync();
//         }

        public bool TryFetchPalletStatusChangeRequests(
            out IEnumerable<PalletStatusChangeRequest> requests,
            out string fault)
        {

        }

        private static SqlConnection _Connection => new SqlConnection(
            XConfiguration.GetConnectionString(Constant.MesConnectionStringName));

    }
}
