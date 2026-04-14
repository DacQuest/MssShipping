using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DacQuest.DFX.Core;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Common;
using Mss.Collections;
using System.Data;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Data.LoadArchive
{
    [Table("LoadArchive")]
    public class LoadArchive
    {
        [Key, Identity]
        public int ID { get; set; }
        public int LoadNumber { get; set; }
        public string TrailerID { get; set; }
        public int PalletCount { get; set; }
        public DateTime ArchivedOn { get; set; } = Constant.BeginningOfTime;
        public int PreviousRotationNumber { get; set; }
//         public DateTime StartedOn { get; set; } = Constant.BeginningOfTime;
//         public DateTime DoneOn { get; set; } = Constant.BeginningOfTime;
//         public DateTime LoadedOn { get; set; } = Constant.BeginningOfTime;
        public DateTime FirstPalletTimestamp { get; set; } = Constant.BeginningOfTime;
        public DateTime LastPalletTimestamp { get; set; } = Constant.BeginningOfTime;
        public DateTime LoadDoneTimestamp { get; set; } = Constant.BeginningOfTime;

        [LeftJoin("LoadItemArchive", "ID", "LoadArchiveID")]
        public List<LoadItemArchive> LoadItems { get; set; }

        [NotMapped]
        public string ShipDate => LoadDoneTimestamp.ToString("MM/dd/yyyy");
        [NotMapped]
        public string ShipTime => LoadDoneTimestamp.ToString("HH:mm:ss");
        [NotMapped]
        public int PickTimeMinutes
        {
            get
            {
                if (FirstPalletTimestamp == Constant.BeginningOfTime
                    || LastPalletTimestamp == Constant.BeginningOfTime
                    || LastPalletTimestamp <= FirstPalletTimestamp)
                {
                    return 0;
                }
                return (int)(LastPalletTimestamp - FirstPalletTimestamp).TotalMinutes;
            }
        }
        [NotMapped]
        public string FormattedPickTimeMinutes => PickTimeMinutes == 0 ? "" : $"{PickTimeMinutes}";

        [NotMapped]
        public int SmallestRotationNumber
        {
            get
            {
                IEnumerable<LoadBroadcastArchive> broadcasts = LoadItems.Select(l => l.Broadcast);
                return LoadItems
                    .Where(l => l.Broadcast.RotationNumber > 0)
                    .Select(l => l.Broadcast)
                    .Min(b => b.RotationNumber);
            }
        }

        [NotMapped]
        public int LargestRotationNumber
        {
            get
            {
                IEnumerable<LoadBroadcastArchive> broadcasts = LoadItems.Select(l => l.Broadcast);
                return LoadItems
                    .Select(l => l.Broadcast)
                    .Max(b => b.RotationNumber);
            }
        }

        public static bool BuildLoadArchive(
            Slug load,
            int loadNumber,
            string trailerID,
            DateTime firstPalletTimestamp,
            DateTime lastPalletTimestamp,
            DateTime loadDoneTimestamp,
            DateTime now,
            int previousBroadcastNumber,
            out LoadArchive loadArchive,
            out string error)
        {
            load.Lock();
            try
            {
                loadArchive = new LoadArchive
                {
                    LoadNumber = loadNumber,
                    TrailerID = trailerID,
                    PalletCount = load.Count(l => l.Status == LoadItemStatus.Done),
                    ArchivedOn = now,
                    PreviousRotationNumber = previousBroadcastNumber,
                    FirstPalletTimestamp = firstPalletTimestamp,
                    LastPalletTimestamp = lastPalletTimestamp,
                    LoadDoneTimestamp = loadDoneTimestamp,
                    LoadItems = new List<LoadItemArchive>()
                };

                IEnumerable<LoadItem> loadItems = load
                    .Where(l => l.Status == LoadItemStatus.Done)
                    .OrderBy(l => l.Broadcast.Csn);
                foreach (LoadItem loadItem in loadItems)
                {
                    LoadItemArchive loadItemArchive = new LoadItemArchive
                    {
                        LoadIndex = loadItem.NodeIndex,
                        Status = loadItem.Status,
                        CraneNumber = loadItem.Crane,
//                         InsertEmpty = loadItem.InsertEmpty,
                    };

                    BroadcastItem broadcastItem = loadItem.Broadcast;
                    LoadBroadcastArchive broadcast = new LoadBroadcastArchive
                    {
                        Status = broadcastItem.Status,
                        RotationNumber = broadcastItem.Rotation,
                        Csn = broadcastItem.Csn,
                        Sku = broadcastItem.Sku,
                        VehicleSku = broadcastItem.VehicleSku,
                        Vin = broadcastItem.Vin,
                        PickMode = broadcastItem.PickMode,
                        PickModeValue = broadcastItem.PickModeKey,
                        ReceivedOn = broadcastItem.ReceivedOn,
                        VehicleRowCount = broadcastItem.VehicleRowCount
                    };
                    loadItemArchive.Broadcast = broadcast;

                    PalletItem palletItem = loadItem.Pallet;
                    LoadPalletArchive pallet = new LoadPalletArchive
                    {
                        PalletID = palletItem.PalletID,
                        Status = palletItem.Status,
                        HoldCode = palletItem.HoldCode,
                        JobID = palletItem.JobID,
                        Sku = palletItem.Sku,
                        BuiltOn = palletItem.BuiltOn,
                        Comment = palletItem.Comment
                    };
                    loadItemArchive.Pallet = pallet;
                    loadArchive.LoadItems.Add(loadItemArchive);
                }
                error = string.Empty;
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent("Build Load Archive");
                error = x.Message;
                loadArchive = null;
                return false;
            }
            finally
            {
                load.Unlock();
            }
        }

        public static bool ArchiveLoadData(
            LoadArchive loadArchive,
            out string errorMessage)
        {
            try
            {
                loadArchive.ArchivedOn = DateTime.Now;
                using (IDbConnection connection = new SqlConnection(XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName)))
                {
                    LoadArchiveRepository loadRepository = new LoadArchiveRepository(connection);
                    LoadItemArchiveRepository loadItemRepository = new LoadItemArchiveRepository(connection);
                    LoadBroadcastArchiveRepository broadcastRepository = new LoadBroadcastArchiveRepository(connection);
                    LoadPalletArchiveRepository palletRepository = new LoadPalletArchiveRepository(connection);
                    if (!loadRepository.Insert(loadArchive))
                    {
                        errorMessage = $"Failed to insert record into LoadArchive table for Load {loadArchive.LoadNumber}.";
                        return false;
                    }
                    foreach (LoadItemArchive loadItemArchive in loadArchive.LoadItems)
                    {
                        loadItemArchive.LoadArchiveID = loadArchive.ID;
                        if (!loadItemRepository.Insert(loadItemArchive))
                        {
                            errorMessage = $"Failed to insert record into LoadItemArchive table for CSN {loadItemArchive.Broadcast.Csn}.";
                            return false;
                        }
                        int loadItemArchiveID = loadItemArchive.ID;
                        LoadBroadcastArchive broadcastArchive = loadItemArchive.Broadcast;
                        broadcastArchive.LoadItemArchiveID = loadItemArchiveID;
                        if (!broadcastRepository.Insert(broadcastArchive))
                        {
                            errorMessage = $"Failed to insert record into LoadBroadArchive table for CSN {broadcastArchive.Csn}.";
                            return false;
                        }
                        LoadPalletArchive palletArchive = loadItemArchive.Pallet;
                        palletArchive.LoadItemArchiveID = loadItemArchiveID;
                        if (!palletRepository.Insert(palletArchive))
                        {
                            errorMessage = $"Failed to insert record into LoadPalletArchive table for Pallet {palletArchive.PalletID}.";
                            return false;
                        }
                    }
                }
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent("Archive Load Data");
                errorMessage = x.Message;
                return false;
            }
            finally
            {

            }

        }
    }
}
