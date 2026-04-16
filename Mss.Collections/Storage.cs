using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Collections
{
    public class Storage : XSharedArray<BinItem>
    {

        public bool IsStorableBinAvailable(
            CraneNumber craneNumber,
            BinSize binSize)
        {
            _ = Lock();
            try
            {
                return _GetStorableBin(craneNumber, binSize) != null;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocateAuditPick(
            CraneNumber craneNumber,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .FirstOrDefault(b =>
                        b.CraneNumber == craneNumber
                        && b.Audit
                        && !b.Disabled
                        && !b.NotUsable);
                if (binItem == null)
                {
                    return false;
                }
                PalletItem binPallet = binItem.Pallet;

                if (binItem.PickOnly
                    && binPallet.BinSize != BinSize.None
                    && !IsStorableBinAvailable(craneNumber, binItem.Pallet.BinSize))
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.GetAllocated;
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocatePurgePick(
            CraneNumber craneNumber,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .FirstOrDefault(b =>
                        b.CraneNumber == craneNumber
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Status == PalletStatus.Purge
                        && !b.Audit
                        && !b.Disabled
                        && !b.NotUsable);
                if (binItem == null)
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.GetAllocated;
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocateStack1Pick(
            CraneNumber craneNumber,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .FirstOrDefault(b =>
                        b.CraneNumber == craneNumber
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Sku == Constant.StackSku1
                        && !b.Audit
                        && !b.Disabled
                        && !b.NotUsable);
                if (binItem == null)
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.GetAllocated;
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocateStack2Pick(
            CraneNumber craneNumber,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .FirstOrDefault(b =>
                        b.CraneNumber == craneNumber
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Sku == Constant.StackSku2
                        && !b.Audit
                        && !b.Disabled
                        && !b.NotUsable);
                if (binItem == null)
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.GetAllocated;
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocateLoadPick(
            CraneNumber craneNumber,
            FifoMode fifoMode,
            LoadItem pickableLoadItem,
            out BinItem binItem)
        {
            BroadcastItem broadcastItem = pickableLoadItem.Broadcast;
            _ = Lock();
            try
            {
                if (fifoMode == FifoMode.Closest
                    && broadcastItem.PickMode == PickMode.BySku)
                {
                    binItem = this
                        .Where(b =>
                        {
                            PalletItem pallet = b.Pallet;
                            return b.CraneNumber == craneNumber
                                && pallet.Sku == broadcastItem.Sku
                                && pallet.Status == PalletStatus.OK
                                && b.BinStatus == BinStatus.Pickable
                                && !b.Audit
                                && !b.Disabled
                                && !b.NotUsable;
                        })
                        .OrderBy(b => b.BinNumber)
                        .FirstOrDefault();
                }
                else
                {
                    IEnumerable<BinItem> binItems;
                    if (broadcastItem.PickMode == PickMode.ByPalletID)
                    {
                        binItem = this
                            .FirstOrDefault(b =>
                            {
                                PalletItem pallet = b.Pallet;
                                return b.CraneNumber == craneNumber
                                    && pallet.PalletID == broadcastItem.PickModeKey
                                    && pallet.Status == PalletStatus.Reserved
                                    && b.BinStatus == BinStatus.Pickable
                                    && !b.Audit
                                    && !b.Disabled
                                    && !b.NotUsable;
                            });
                        if (binItem != null)
                        {
                            if (binItem.Pallet.Sku != broadcastItem.Sku)
                            {
                                binItem.BinStatus = BinStatus.Offline;
                                PalletItem foundPallet = binItem.Pallet;
                                foundPallet.Comment = $"RESERVED PALLET SKU MISMATCH! See System Events.";
                                binItem.Pallet = foundPallet;
                                this[binItem.NodeIndex] = binItem;
                                XSystemEvent.Publish(
                                    "Reserved Pallet SKU Mismatch",
                                    XSystemEventLevel.Error,
                                    $"Broadcast SKU {broadcastItem.Sku} does not match Reserved Pallet SKU {foundPallet.Sku}. (Bin Location:{binItem.LocationText} / Slug:{pickableLoadItem.SlugLetter.ToText()} / CSN:{broadcastItem.Csn})");
                                binItem = null;
                            }
                        }
                    }
                    else if (broadcastItem.PickMode == PickMode.ByJobID)
                    {
                        binItem = this
                            .FirstOrDefault(b =>
                            {
                                PalletItem pallet = b.Pallet;
                                return b.CraneNumber == craneNumber
                                    && pallet.JobID == broadcastItem.PickModeKey
                                    && pallet.Status == PalletStatus.Reserved
                                    && b.BinStatus == BinStatus.Pickable
                                    && !b.Audit
                                    && !b.Disabled
                                    && !b.NotUsable;
                            });
                        if (binItem != null)
                        {
                            if (binItem.Pallet.Sku != broadcastItem.Sku)
                            {
                                binItem.BinStatus = BinStatus.Offline;
                                PalletItem foundPallet = binItem.Pallet;
                                foundPallet.Comment = $"RESERVED PALLET SKU MISMATCH! See System Events.";
                                binItem.Pallet = foundPallet;
                                this[binItem.NodeIndex] = binItem;
                                XSystemEvent.Publish(
                                    "Reserved Pallet SKU Mismatch",
                                    XSystemEventLevel.Error,
                                    $"Broadcast SKU {broadcastItem.Sku} does not match Reserved Pallet SKU {foundPallet.Sku}. (Bin Location:{binItem.LocationText} / Slug:{pickableLoadItem.SlugLetter.ToText()} / CSN:{broadcastItem.Csn})");
                                binItem = null;
                            }
                        }
                    }
                    else
                    {
                        binItems = this
                                .Where(b =>
                                {
                                    PalletItem pallet = b.Pallet;
                                    return pallet.Sku == broadcastItem.Sku
                                        && pallet.Status == PalletStatus.OK
                                        && b.BinStatus == BinStatus.Pickable
                                        && !b.Audit
                                        && !b.Disabled
                                        && !b.NotUsable;
                                })
                                .OrderBy(b => b.Pallet.BuiltOn);
                        binItem = binItems.FirstOrDefault();
                        if (binItem != null)
                        {
                            if (binItem.CraneNumber != craneNumber)
                            {
                                binItem = fifoMode == FifoMode.BuildFifo
                                    ? null
                                    : binItems.FirstOrDefault(b => b.CraneNumber == craneNumber);
                            }
                        }
                    }
                    if (binItem != null)
                    {
                        binItem.BinStatus = BinStatus.GetAllocated;
                        this[binItem.NodeIndex] = binItem;
                    }
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public void ClearBinByPalletID(string palletID, bool audit)
        {
            if (palletID == Constant.NoPalletID)
            {
                return;
            }
            _ = Lock();
            try
            {
                BinItem binItem = this.FirstOrDefault(b => b.Pallet.PalletID == palletID);
                _ClearBin(binItem, audit);
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryFindByPalletID(string palletID, out PalletItem palletItem)
        {
            _ = Lock();
            try
            {
                palletItem = null;
                BinItem binItem = this.FirstOrDefault(b => b.Pallet.PalletID == palletID);
                if (binItem == null)
                {
                    return false;
                }
                palletItem = binItem.Pallet;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryFindBinByPalletID(string palletID, out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this.FirstOrDefault(b => b.Pallet.PalletID == palletID);
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryFindEmptyBin(
            CraneNumber craneNumber,
            BinSize binSize,
            out BinItem bin)
        {
            _ = Lock();
            try
            {
                bin = this
                    .FirstOrDefault(b =>
                        b.CraneNumber == craneNumber
                        && b.BinSize == binSize
                        && b.BinStatus == BinStatus.Empty
                        && !b.Disabled
                        && !b.PickOnly
                        && !b.NotUsable);
                return bin != null;
            }
            finally
            {
                Unlock();
            }
        }

        public void CompleteSemiAutoStore(
            int binIndex,
            PalletItem palletItem,
            BinStatus binStatus)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this[binIndex];
                binItem.Pallet = palletItem;
                binItem.BinStatus = binStatus;
                binItem.StoredOn = DateTime.Now;
                this[binItem.NodeIndex] = binItem;
            }
            finally
            {
                Unlock();
            }
        }
        public void EmptyBinByIndex(int binIndex)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this[binIndex];
                binItem.Clear(BinStatus.Empty);
                this[binItem.NodeIndex] = binItem;
            }
            finally
            {
                Unlock();
            }
        }
        public bool CompleteGet(CraneNumber craneNumber, string palletID)
        {
            return CompleteGet(craneNumber, palletID, false);
        }
        public bool CompleteGet(CraneNumber craneNumber, string palletID, bool semiAutoMode)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this.Where(b =>
                        b.CraneNumber == craneNumber
                        && b.Pallet.PalletID == palletID
                        && (b.BinStatus == BinStatus.GetAllocated || semiAutoMode)
                        && !b.Disabled
                        && !b.NotUsable)
                .FirstOrDefault();
                if (binItem != null)
                {
                    binItem.Clear(BinStatus.Empty);
                    this[binItem.NodeIndex] = binItem;
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }
        public bool CanStore(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return this.Any(b =>
                {
                    return b.CraneNumber == craneNumber
                        && b.BinStatus == BinStatus.Empty
                        && !b.Audit
                        && !b.NotUsable
                        && !b.PickOnly
                        && !b.Disabled;
                });
            }
            finally
            {
                Unlock();
            }
        }
        public string GetSkuFromJobID(string jobID)
        {
            _ = Lock();
            try
            {
                string sku = string.Empty;
                BinItem bin = this.FirstOrDefault(b => b.Pallet.JobID == jobID);
                if (bin != null)
                {
                    sku = bin.Pallet.Sku;
                }
                return sku;
            }
            finally
            {
                Unlock();
            }
        }
        //public string GetSkuFromPalletID(int palletID)
        //{
        //    Lock();
        //    try
        //    {
        //        string sku = string.Empty;
        //        BinItem bin = this.FirstOrDefault(b => b.Pallet.PalletID == palletID);
        //        if (bin != null)
        //        {
        //            sku = bin.Pallet.Sku;
        //        }
        //        return sku;
        //    }
        //    finally
        //    {
        //        Unlock();
        //    }
        //}

        public bool TryFindPickableBinByPalletID(
            CraneNumber craneNumber,
            string palletID,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = null;
                IEnumerable<BinItem> bins = this
                    .Where(b =>
                        b.Pallet.PalletID == palletID
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Status == PalletStatus.OK);
                if (bins.Count() == 1)
                {
                    if (bins.First().CraneNumber == craneNumber)
                    {
                        binItem = bins.First();
                    }
                }
                else if (bins.Count() > 1)
                {
                    foreach (BinItem bin in bins)
                    {
                        bin.SetDuplicatePalletIDHold();
                        this[bin.NodeIndex] = bin;
                    }
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }
        public bool TryFindPickableBinByJobID(CraneNumber craneNumber, string jobID, out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = null;
                IEnumerable<BinItem> bins = this
                    .Where(b =>
                        b.Pallet.JobID == jobID
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Status == PalletStatus.OK);
                if (bins.Count() == 1)
                {
                    if (bins.First().CraneNumber == craneNumber)
                    {
                        binItem = bins.First();
                    }
                }
                else if (bins.Count() > 1)
                {
                    foreach (BinItem bin in bins)
                    {
                        bin.SetDuplicateJobIDHold();
                        this[bin.NodeIndex] = bin;
                    }
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public (CraneNumber craneNumber, int skuCount)[] GetPrioritizedSkuCountPerCrane(string sku)
        {
            (CraneNumber craneNumber, int skuCount)[] skuCounts = new (CraneNumber craneNumber, int skuCount)[Constant.MaxCranes + 1];
            _ = Lock();
            try
            {
                for (CraneNumber craneNumber = CraneNumber.Crane1;
                        craneNumber <= Constant.LastCraneNumber;
                        craneNumber++)
                {
                    int count = 0;
                    if (sku.ValidSku())
                    {
                        count = this.Count(p =>
                            p.CraneNumber == craneNumber
                            && p.Pallet.Sku == sku);
                    }
                    skuCounts[craneNumber.Index()] = (craneNumber, count);
                }
                return skuCounts; //.OrderBy(c => c.skuCount).ToArray();
            }
            finally
            {
                Unlock();
            }
        }

        public int GetEmptyBinCount(CraneNumber craneNumber)
        {
            return GetEmptyBinCount(BinSize.None, craneNumber);
        }

        public int GetEmptyBinCount(BinSize binSize, CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return this.Count(b =>
                {
                    return (b.CraneNumber == craneNumber || craneNumber == CraneNumber.None)
                        && b.BinStatus == BinStatus.Empty
                        && (b.BinSize == binSize || binSize == BinSize.None)
                        && !b.Audit
                        && !b.Disabled
                        && !b.PickOnly
                        && !b.NotUsable;
                });
            }
            finally
            {
                Unlock();
            }
        }

//         public bool AreBinsAvailable(
//             CraneNumber craneNumber,
//             int preallocatedBinCount,
//             bool isPalletStack)
//         {
//             _ = Lock();
//             try
//             {
//                 int freeBins = this.Count(b =>
//                 {
//                     return
//                         (b.CraneNumber == craneNumber || craneNumber == CraneNumber.None)
//                         //&& b.StackOnly == isPalletStack
//                         && b.BinStatus == BinStatus.Empty
//                         && !b.Audit
//                         && !b.NotUsable
//                         && !b.PickOnly
//                         && !b.Disabled;
//                 }) - preallocatedBinCount;
//                 return freeBins > 0;
//             }
//             finally
//             {
//                 Unlock();
//             }
//         }
        public void MarkDuplicatesForAudit(string palletID)
        {
            MarkDuplicatesForAudit(palletID, -1);
        }
        public void MarkDuplicatesForAudit(string palletID, int nodeIndexToExclude)
        {
            _ = Lock();
            try
            {
                IEnumerable<BinItem> duplicates = this.Where(b =>
                {
                    return b.NodeIndex != nodeIndexToExclude
                        && b.Pallet.PalletID == palletID;
                });
                foreach (BinItem binItem in duplicates)
                {
                    binItem.MarkAsDuplicateForAudit();
                    this[binItem.NodeIndex] = binItem;
                }
            }
            finally
            {
                Unlock();
            }
        }

        public int Get1stRowPalletCount()
        {
            return GetPalletCount(CraneNumber.None, VehicleRow.Row1);
        }
        public int Get1stRowPalletCount(CraneNumber craneNumber)
        {
            return GetPalletCount(craneNumber, VehicleRow.Row1);
        }
        public int Get2ndRowPalletCount()
        {
            return Get2ndRowPalletCount(CraneNumber.None);
        }

        public int Get2ndRowPalletCount(CraneNumber craneNumber)
        {
            return GetPalletCount(craneNumber, VehicleRow.Row2);
        }

        public int GetPalletCount(CraneNumber craneNumber, VehicleRow vehicleRow)
        {
            _ = Lock();
            try
            {
                return this
                    .Count(bin =>
                    {
                        return (craneNumber == CraneNumber.None || bin.CraneNumber == craneNumber)
                            && bin.Pallet.VehicleRow == vehicleRow;
                    });
            }
            finally
            {
                Unlock();
            }
        }
        public Dictionary<string, SkuCountsByStatus> GetSkuCountsByStatus()
        {
            Dictionary<string, SkuCountsByStatus> counts = new Dictionary<string, SkuCountsByStatus>();
            _ = Lock();
            try
            {
                foreach (BinItem bin in this)
                {
                    PalletItem pallet = bin.Pallet;
                    if (!counts.TryGetValue(pallet.Sku, out SkuCountsByStatus count))
                    {
                        count = new SkuCountsByStatus(pallet.Sku);
                        counts[pallet.Sku] = count;
                    }
                    switch (pallet.Status)
                    {
                        case PalletStatus.OK:
                            counts[pallet.Sku].OKCount += 1;
                            break;
                        case PalletStatus.Hold:
                            counts[pallet.Sku].HoldCount += 1;
                            break;
                        case PalletStatus.Purge:
                            counts[pallet.Sku].PurgeCount += 1;
                            break;
                        case PalletStatus.Reserved:
                            counts[pallet.Sku].ReserveCount += 1;
                            break;
//                         case PalletStatus.Stack:
//                             counts[pallet.Sku].StackCount += 1;
//                             break;
                        case PalletStatus.Unknown:
                            counts[pallet.Sku].UnknownCount += 1;
                            break;
                    }
                }
                return counts;
            }
            finally
            {
                Unlock();
            }
        }
        public int GetSkuCount(PalletStatus palletStatuses, string sku)
        {
            return GetSkuCount(CraneNumber.None, palletStatuses, sku);
        }

        public int GetSkuCount(CraneNumber craneNumber, PalletStatus palletStatuses, string sku)
        {
            _ = Lock();
            try
            {
                return this.Count(b =>
                {
                    return b.CraneNumber == craneNumber
                        && b.Pallet.Sku == sku
                        && palletStatuses.IsFlagSet(b.Pallet.Status);
                });
            }
            finally
            {
                Unlock();
            }
        }

//         public Dictionary<string, int> GetPickableSkuCounts(
//             out List<string> reservedPalletIDs,
//             out List<string> reservedJobIDs)
//         {
//             reservedPalletIDs = new List<string>();
//             reservedJobIDs = new List<string>();
//             Dictionary<string, int> skuCounts = new Dictionary<string, int>();
// 
//             _ = Lock();
//             try
//             {
//                 IEnumerable<BinItem> pickableBins = this
//                     .Where(b =>
//                     {
//                         PalletItem palletItem = b.Pallet;
//                         return b.BinStatus == BinStatus.Pickable
//                             && (palletItem.Status == PalletStatus.OK
//                                 || palletItem.Status == PalletStatus.Reserved)
//                             && !b.Disabled
//                             && !b.NotUsable;
//                     });
//                 foreach (BinItem pickableBin in pickableBins)
//                 {
//                     PalletItem pallet = pickableBin.Pallet;
//                     if (pallet.Status == PalletStatus.Reserved)
//                     {
//                         reservedPalletIDs.Add(pallet.PalletID);
//                         reservedJobIDs.Add(pallet.JobID);
//                     }
//                     else // pallet.Status == PalletStatus.OK
//                     {
//                         string sku = pallet.Sku;
//                         if (skuCounts.TryGetValue(sku, out int count))
//                         {
//                             count++;
//                         }
//                         else
//                         {
//                             count = 1;
//                         }
//                         skuCounts[sku] = count;
//                     }
//                 }
//                 return skuCounts;
//             }
//             finally
//             {
//                 Unlock();
//             }
//         }

        public Dictionary<string, int> GetPickableSkuCounts(
            out List<PalletPickModeKeys> reservedPalletKeys)
        {
            reservedPalletKeys = new List<PalletPickModeKeys>();
            Dictionary<string, int> skuCounts = new Dictionary<string, int>();

            _ = Lock();
            try
            {
                IEnumerable<BinItem> pickableBins = this
                    .Where(b =>
                    {
                        PalletItem palletItem = b.Pallet;
                        return b.BinStatus == BinStatus.Pickable
                            && (palletItem.Status == PalletStatus.OK
                                || palletItem.Status == PalletStatus.Reserved)
                            && !b.Disabled
                            && !b.NotUsable;
                    });
                foreach (BinItem pickableBin in pickableBins)
                {
                    PalletItem pallet = pickableBin.Pallet;
                    if (pallet.Status == PalletStatus.Reserved)
                    {
                        reservedPalletKeys.Add(
                            new PalletPickModeKeys
                            {
                                PalletID = pallet.PalletID,
                                JobID = pallet.JobID
                            });
                    }
                    else // pallet.Status == PalletStatus.OK
                    {
                        string sku = pallet.Sku;
                        if (skuCounts.TryGetValue(sku, out int count))
                        {
                            count++;
                        }
                        else
                        {
                            count = 1;
                        }
                        skuCounts[sku] = count;
                    }
                }
                return skuCounts;
            }
            finally
            {
                Unlock();
            }
        }

        public Dictionary<string, int> GetSkuCounts(PalletStatus palletStatuses)
        {
            return GetSkuCounts(CraneNumber.None, palletStatuses);
        }

        public Dictionary<string, int> GetSkuCounts(
            CraneNumber craneNumber,
            PalletStatus palletStatuses)
        {
            Dictionary<string, int> skuCounts = new Dictionary<string, int>();
            _ = Lock();
            try
            {
                foreach (BinItem bin in this.Where(
                    bin => (craneNumber == CraneNumber.None
                    || bin.CraneNumber == craneNumber)
                    && bin.BinStatus == BinStatus.Pickable
                    && !bin.Disabled
                    && !bin.NotUsable))

                {
                    PalletItem pallet = bin.Pallet;
                    if (palletStatuses.IsFlagSet(pallet.Status))
                    {
                        string sku = pallet.Sku;
                        if (skuCounts.TryGetValue(sku, out int count))
                        {
                            count++;
                        }
                        else
                        {
                            count = 1;
                        }
                        skuCounts[sku] = count;
                    }
                }
                return skuCounts;
            }
            finally
            {
                Unlock();
            }
        }
        public bool IsSkuPickable(string sku, CraneNumber[] autoModeCranes)
        {
            _ = Lock();
            try
            {
                return this
                    .Any(b => autoModeCranes.Contains(b.CraneNumber)
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Status == PalletStatus.OK
                        && b.Pallet.Sku == sku);
            }
            finally
            {
                Unlock();
            }
        }
        public int GetPickableSkuCount(string sku)
        {
            return GetPickableSkuCount(CraneNumber.None, sku);
        }

        public int GetPickableSkuCount(CraneNumber craneNumber, string sku)
        {
            _ = Lock();
            try
            {
                return this
                    .Count(b => (craneNumber == CraneNumber.None || b.CraneNumber == craneNumber)
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Status == PalletStatus.OK
                        && b.Pallet.Sku == sku);
            }
            finally
            {
                Unlock();
            }
        }
        public void CleanUpAllocatedBins(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                IEnumerable<BinItem> bins = this.Where(b =>
                    b.CraneNumber == craneNumber
                    && (b.BinStatus == BinStatus.GetAllocated
                        || b.BinStatus == BinStatus.PutAllocated));
                foreach (BinItem bin in bins)
                {
                    bin.BinStatus = BinStatus.Offline;
                    bin.Audit = true;
                    bin.Pallet.Status = PalletStatus.Unknown;
                    this[bin.NodeIndex] = bin;
                }
            }
            finally
            {
                Unlock();
            }
        }

        public void ClearBinByLocation(int location, bool audit)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this.FirstOrDefault(b => b.Location == location);
                _ClearBin(binItem, audit);
            }
            finally
            {
                Unlock();
            }
        }

        private void _ClearBin(BinItem binItem, bool audit)
        {
            if (binItem == null)
            {
                return;
            }
            binItem.Clear();
            binItem.Audit = audit;
            if (audit)
            {
                binItem.BinStatus = BinStatus.Offline;
            }
            this[binItem.NodeIndex] = binItem;
        }

        public void GetSkuCountByCrane(string sku, out Dictionary<CraneNumber, int> skuByCrane)
        {
            _ = Lock();
            try
            {
                skuByCrane = new Dictionary<CraneNumber, int>();
                for (CraneNumber crane = CraneNumber.Crane1; crane <= CraneNumber.Crane3; crane++)
                {
                    skuByCrane[crane] = this.Count(bin => bin.Pallet.Sku == sku && bin.CraneNumber == crane);
                }
            }
            finally
            {
                Unlock();
            }
        }

        private BinItem _GetStorableBin(
            CraneNumber craneNumber,
            BinSize binSize)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                binSize,
                nameof(binSize),
                new[] { BinSize.Large, BinSize.Small } );

            if (binSize == BinSize.Small)
            {
                BinItem binItem = _GetStorableSmallBin(craneNumber)
                    ?? _GetStorableLargeBin(craneNumber);
                return binItem;
            }
            return _GetStorableLargeBin(craneNumber);
        }

        private BinItem _GetStorableSmallBin(CraneNumber craneNumber)
        {
            return this
                .FirstOrDefault(b =>
                    b.BinStatus == BinStatus.Empty
                    && b.CraneNumber == craneNumber
                    && b.BinSize == BinSize.Small
                    && !b.Disabled
                    && !b.PickOnly
                    && !b.Audit
                    && !b.NotUsable);
        }

        private BinItem _GetStorableLargeBin(CraneNumber craneNumber)
        {
            return this
                .FirstOrDefault(b =>
                    b.BinStatus == BinStatus.Empty
                    && b.CraneNumber == craneNumber
                    && b.BinSize == BinSize.Large
                    && !b.Disabled
                    && !b.PickOnly
                    && !b.Audit
                    && !b.NotUsable);
        }

        private BinItem _GetStorableStackBin(CraneNumber craneNumber)
        {
            return this
                .Reverse()
                .FirstOrDefault(b =>
                    b.BinStatus == BinStatus.Empty
                    && b.CraneNumber == craneNumber
                    && b.BinSize == BinSize.Small
                    && !b.Disabled
                    && !b.PickOnly
                    && !b.Audit
                    && !b.NotUsable)
                ?? this
                    .Reverse()
                    .FirstOrDefault(b =>
                        b.BinStatus == BinStatus.Empty
                        && b.CraneNumber == craneNumber
                        && b.BinSize == BinSize.Large
                        && !b.Disabled
                        && !b.PickOnly
                        && !b.Audit
                        && !b.NotUsable);
        }

        public bool IsStorableStackBinAvailable(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return _GetStorableStackBin(craneNumber) != null;
            }
            finally
            {
                Unlock();
            }
        }

        public bool AnyPickable(CraneNumber craneNumber, string sku)
        {
            return FindOldestPickableSku(
                craneNumber,
                sku,
                out _);
        }

        public bool FindOldestPickableSku(
            CraneNumber craneNumber,
            string sku,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .Where(b =>
                        (craneNumber == CraneNumber.None
                            || b.CraneNumber == craneNumber)
                        && b.BinStatus == BinStatus.Pickable
                        && b.Pallet.Status == PalletStatus.OK
                        && b.Pallet.Sku == sku
                        && !b.NotUsable
                        && !b.Disabled
                        && !b.Audit)
                    .OrderBy(b => b.Pallet.BuiltOn)
                    .FirstOrDefault();
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocatePut(
            CraneNumber craneNumber,
            PalletItem palletItem,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = null;
                if (palletItem.BinSize == BinSize.Small)
                {
                    binItem = _GetStorableSmallBin(craneNumber);
                }
                if (binItem == null)
                {
                    binItem = _GetStorableLargeBin(craneNumber);
                }
                if (binItem == null)
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.PutAllocated;
                binItem.StoredOn = DateTime.Now;
                binItem.Pallet = palletItem;
                binItem.Audit = palletItem.Status == PalletStatus.Unknown;
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool TryAllocateStackPut(
            CraneNumber craneNumber,
            PalletItem palletItem,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = _GetStorableStackBin(craneNumber);
                if (binItem != null)
                {
                    binItem.BinStatus = BinStatus.PutAllocated;
                    binItem.StoredOn = DateTime.Now;
                    binItem.Pallet = palletItem;
                    this[binItem.NodeIndex] = binItem;
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public bool CompletePut(CraneNumber craneNumber, string palletID, BinStatus binStatus)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this
                    .FirstOrDefault(b =>
                        b.CraneNumber == craneNumber
                        && b.Pallet.PalletID == palletID
                        && b.BinStatus == BinStatus.PutAllocated);
                if (binItem != null)
                {
                    binItem.StoredOn = DateTime.Now;
                    binItem.BinStatus = binStatus;
                    this[binItem.NodeIndex] = binItem;
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public void RollbackPut(string palletID, BinStatus binStatus, bool markForAudit)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this
                    .Where(bin => bin.BinStatus == BinStatus.PutAllocated
                        && bin.Pallet.PalletID == palletID)
                    .FirstOrDefault();
                if (binItem != null)
                {
                    binItem.Clear(binStatus, markForAudit);
                    this[binItem.NodeIndex] = binItem;
                }
            }
            finally
            {
                Unlock();
            }
        }

        public bool FindAudit(
                 CraneNumber craneNumber,
                 out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .FirstOrDefault(bin =>
                        bin.CraneNumber == craneNumber
                        && bin.Audit
                        && !bin.NotUsable
                        && !bin.Disabled);
                if (binItem == null)
                {
                    return false;
                }
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public void CompleteGetByLocation(int location)
        {
            _ = Lock();
            try
            {
                ClearBinByLocation(location, false);
            }
            finally
            {
                Unlock();
            }
        }

        public void RollbackGet(string palletID, BinStatus binStatus)
        {
            BinItem binItem;
            _ = Lock();
            try
            {
                binItem = this.FirstOrDefault(b => b.Pallet.PalletID == palletID && b.BinStatus == BinStatus.GetAllocated);
                if (binItem == null)
                {
                    return;
                }
                binItem.BinStatus = binStatus;
                if (binStatus == BinStatus.Empty)
                {
                    binItem.Clear(BinStatus.Empty);
                }
                this[binItem.NodeIndex] = binItem;
            }
            finally
            {
                Unlock();
            }
        }

        public bool CompletePutByLocation(int location)
        {
            _ = Lock();
            try
            {
                BinItem binItem = this
                    .FirstOrDefault(b =>
                        b.BinStatus == BinStatus.PutAllocated
                        && b.Location == location);
                if (binItem == null)
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.Pickable;
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }

        public bool AllocateOfflineStore(
            CraneNumber craneNumber,
            PalletItem palletItem,
            int currentAuditAttempts,
            int maxAuditAttempts,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = null;
                int vertical = 1;
                while (vertical <= Constant.MaxVertical)
                {
                    binItem = this
                        .Reverse()
                        .FirstOrDefault(b =>
                            b.BinVerticalNumber == vertical
                            && b.CraneNumber == craneNumber
                            && b.BinStatus == BinStatus.Empty
                            && !b.Audit
                            && !b.NotUsable
                            && !b.Disabled
                            && !b.PickOnly);
                    if (binItem != null)
                    {
                        break;
                    }
                    vertical++;
                }
                if (binItem != null)
                {
                    bool flagForAudit = false;
                    if (palletItem.PalletID != Constant.NoPalletID)
                    {
                        flagForAudit = currentAuditAttempts < maxAuditAttempts;

                    }
                    binItem.BinStatus = BinStatus.PutAllocated;
                    binItem.Pallet = palletItem;
                    binItem.StoredOn = DateTime.Now;
                    binItem.Audit = flagForAudit;
//                     binItem.AuditAttempts = !flagForAudit ? 0 : currentAuditAttempts;
                    this[binItem.NodeIndex] = binItem;
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }
        public bool AllocateNormalStore(
            CraneNumber craneNumber,
            PalletItem palletItem,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = this
                    .FirstOrDefault(b =>
                        b.BinStatus == BinStatus.Empty
                        && b.CraneNumber == craneNumber
                        && !b.Audit
                        && !b.NotUsable
                        && !b.Disabled
                        && !b.PickOnly);

                if (binItem != null)
                {
                    binItem.BinStatus = BinStatus.PutAllocated;
                    binItem.Pallet = palletItem;
                    binItem.StoredOn = DateTime.Now;
//                     binItem.AuditAttempts = 0;
                    this[binItem.NodeIndex] = binItem;
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }
        public bool GetOldestPickableBin(
            CraneNumber craneNumber,
            string sku,
            FifoMode fifoMode,
            out BinItem binItem)
        {
            _ = Lock();
            try
            {
                binItem = null;
                if (fifoMode == FifoMode.CraneFifo)
                {
                    binItem = this
                        .Where(b =>
                        {
                            PalletItem pallet = b.Pallet;
                            return b.CraneNumber == craneNumber
                                && b.BinStatus == BinStatus.Pickable
                                && pallet.Sku == sku
                                && pallet.Status == PalletStatus.OK
                                && !b.Audit
                                && !b.Disabled
                                && !b.NotUsable;
                        })
                        .OrderBy(b => b.Pallet.BuiltOn)
                        .FirstOrDefault();
                }
                else if (fifoMode == FifoMode.BuildFifo)
                {
                    binItem = this
                        .Where(b =>
                        {
                            PalletItem pallet = b.Pallet;
                            return b.BinStatus == BinStatus.Pickable
                                && pallet.Sku == sku
                                && pallet.Status == PalletStatus.OK
                                && !b.Audit
                                && !b.Disabled
                                && !b.NotUsable;
                        })
                        .OrderBy(b => b.Pallet.BuiltOn)
                        .FirstOrDefault();

                    if (binItem != null
                        && binItem.CraneNumber != craneNumber)
                    {
                        binItem = null;
                    }
                    //                     else
                    //                     {
                    //                         PalletItem palletItem = binItem.Pallet;
                    // 
                    //                         XSystemEvent.Publish(
                    //                             "Load Pallet Assignment",
                    //                             XSystemEventLevel.Telemetry,
                    //                             $" Load Pallet Assignment: Load Pallet {palletItem.PalletID} PS={palletItem.Sku}. BS={sku} (CSN {broadcastItem.Csn})");
                    //                     }
                }
                return binItem != null;
            }
            finally
            {
                Unlock();
            }
        }



























    }

}



