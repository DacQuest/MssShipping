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
        public bool TryFindByPalletID(string palletID, out PalletItem palletItem)
        {
            Lock();
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


        public void CompleteSemiAutoStore(
            int binIndex,
            PalletItem palletItem,
            BinStatus binStatus)
        {
            Lock();
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
            Lock();
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
            Lock();
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
            Lock();
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
        public string GetSkuFromJobID(int jobID)
        {
            Lock();
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
            Lock();
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
        public bool TryFindPickableBinByJobID(CraneNumber craneNumber, int jobID, out BinItem binItem)
        {
            Lock();
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
        public int GetEmptyBinCount()
        {
            return GetEmptyBinCount(CraneNumber.None);
        }

        public int GetEmptyBinCount(CraneNumber craneNumber)
        {
            Lock();
            try
            {
                return this.Count(b =>
                {
                    return (b.CraneNumber == craneNumber || craneNumber == CraneNumber.None)
                        && b.BinStatus == BinStatus.Empty;
                });
            }
            finally
            {
                Unlock();
            }
        }
        public bool AreBinsAvailable(
            CraneNumber craneNumber,
            int preallocatedBinCount,
            bool isPalletStack)
        {
            Lock();
            try
            {
                int freeBins = this.Count(b =>
                {
                    return
                        (b.CraneNumber == craneNumber || craneNumber == CraneNumber.None)
                        //&& b.StackOnly == isPalletStack
                        && b.BinStatus == BinStatus.Empty
                        && !b.Audit
                        && !b.NotUsable
                        && !b.PickOnly
                        && !b.Disabled;
                }) - preallocatedBinCount;
                return freeBins > 0;
            }
            finally
            {
                Unlock();
            }
        }
        public void MarkDuplicatesForAudit(string palletID)
        {
            MarkDuplicatesForAudit(palletID, -1);
        }
        public void MarkDuplicatesForAudit(string palletID, int nodeIndexToExclude)
        {
            Lock();
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
            Lock();
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
            Lock();
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
                        case PalletStatus.Reserve:
                            counts[pallet.Sku].QAPickCount += 1;
                            break;
                        case PalletStatus.Stack:
                            counts[pallet.Sku].QAPickCount += 1;
                            break;
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
            Lock();
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
        public Dictionary<string, int> GetSkuCounts(PalletStatus palletStatuses)
        {
            return GetSkuCounts(CraneNumber.None, palletStatuses);
        }

        public Dictionary<string, int> GetSkuCounts(
            CraneNumber craneNumber,
            PalletStatus palletStatuses)
        {
            Dictionary<string, int> skuCounts = new Dictionary<string, int>();
            Lock();
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
            Lock();
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
            Lock();
            try
            {
                return this
                    .Count(bin => craneNumber == CraneNumber.None || bin.CraneNumber == craneNumber
                        && bin.BinStatus == BinStatus.Pickable
                        && bin.Pallet.Status == PalletStatus.OK
                        && bin.Pallet.Sku == sku);
            }
            finally
            {
                Unlock();
            }
        }
        public void CleanUpAllocatedBins(CraneNumber craneNumber)
        {
            Lock();
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
            Lock();
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
        public void ClearBinByPalletID(string palletID, bool audit)
        {
            if (palletID == Constant.NoPalletID)
            {
                return;
            }
            Lock();
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
            Lock();
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
        public bool GetStorableBin(CraneNumber craneNumber, out BinItem binItem)
        {
            binItem = this.FirstOrDefault(b =>
                b.BinStatus == BinStatus.Empty
                && b.CraneNumber == craneNumber
                //&& !b.StackOnly
                && !b.Disabled
                && !b.PickOnly
                && !b.Audit
                && !b.NotUsable);
            return binItem != null;
        }

        public bool GetStorableStackBin(CraneNumber craneNumber, out BinItem binItem)
        {
            binItem = this.FirstOrDefault(b =>
                b.BinStatus == BinStatus.Empty
                && b.CraneNumber == craneNumber
                //&& b.StackOnly
                && !b.Disabled
                && !b.PickOnly
                && !b.Audit
                && !b.NotUsable);
            return binItem != null;
        }

        public bool AllocatePut(
            CraneNumber craneNumber,
            PalletItem palletItem,
            int maxAuditAttempts,
            out BinItem binItem)
        {
            Lock();
            try
            {
                if (!GetStorableBin(craneNumber, out binItem))
                {
                    return false;
                }
                binItem.BinStatus = BinStatus.PutAllocated;
                binItem.StoredOn = DateTime.Now;
                binItem.Pallet = palletItem;
                binItem.Audit = palletItem.Status == PalletStatus.Unknown;
                if (binItem.Audit)
                {
                    if (binItem.AuditAttempts >= maxAuditAttempts)
                    {
                        binItem.Audit = false;
                        binItem.AuditAttempts = 0;
                    }
                    else
                    {
                        binItem.AuditAttempts++;
                    }
                }
                else
                {
                    binItem.AuditAttempts = 0;
                }
                this[binItem.NodeIndex] = binItem;
                return true;
            }
            finally
            {
                Unlock();
            }
        }
        public bool CompletePut(CraneNumber craneNumber, string palletID, BinStatus binStatus)
        {
            Lock();
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
            Lock();
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
            Lock();
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
            Lock();
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
            Lock();
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
        public bool AllocateOfflineStore(
            CraneNumber craneNumber,
            PalletItem palletItem,
            int currentAuditAttempts,
            int maxAuditAttempts,
            out BinItem binItem)
        {
            Lock();
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
                    binItem.AuditAttempts = !flagForAudit ? 0 : currentAuditAttempts;
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
            Lock();
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
                    binItem.AuditAttempts = 0;
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
            Lock();
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



