using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.Strings;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Collections
{
    public class StorageProxy : XSharedArrayProxy<BinItem>
    {
        public List<BinItem> Items => ProxyList.ToList();

        public void GetSkuCounts(
            BinStatus binStatuses,
            PalletStatus palletStatuses,
            out Dictionary<string, int> skuCounts)
        {
            skuCounts = new Dictionary<string, int>();
            foreach (BinItem bin in Items)
            {
                PalletItem pallet = bin.Pallet;
                if (binStatuses.IsFlagSet(bin.BinStatus)
                    && palletStatuses.IsFlagSet(pallet.Status))
                {
                    int count = 0;
                    string sku = pallet.Sku;
                    count = 0;
                    if (skuCounts.TryGetValue(sku, out count))
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
        }

        public int GetEmptyLargeBinCount(CraneNumber craneNumber)
            => Items
                .Count(b =>
                    (b.CraneNumber == craneNumber || craneNumber == CraneNumber.None)
                    && b.BinStatus == BinStatus.Empty
                    && b.BinSize == BinSize.Large
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);

        public void GetEmptyLargeBinCountPerCrane(
            out int crane1Count,
            out int crane2Count,
            out int crane3Count,
            out int crane4Count)
        {
            crane1Count = GetEmptyLargeBinCount(CraneNumber.Crane1);
            crane2Count = GetEmptyLargeBinCount(CraneNumber.Crane2);
            crane3Count = GetEmptyLargeBinCount(CraneNumber.Crane3);
            crane4Count = GetEmptyLargeBinCount(CraneNumber.Crane4);
        }

        public int GetEmptySmallBinCount(CraneNumber craneNumber)
            => Items
                .Count(b =>
                    (b.CraneNumber == craneNumber || craneNumber == CraneNumber.None)
                    && b.BinStatus == BinStatus.Empty
                    && b.BinSize == BinSize.Small
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);

        public void GetEmptySmallBinCountPerCrane(
            out int crane1Count,
            out int crane2Count,
            out int crane3Count,
            out int crane4Count)
        {
            crane1Count = GetEmptySmallBinCount(CraneNumber.Crane1);
            crane2Count = GetEmptySmallBinCount(CraneNumber.Crane2);
            crane3Count = GetEmptySmallBinCount(CraneNumber.Crane3);
            crane4Count = GetEmptySmallBinCount(CraneNumber.Crane4);
        }

        public int GetPalletCount(CraneNumber craneNumber)
        {
            return Items
                .Count(b => b.CraneNumber == craneNumber
                    && b.Pallet.Sku != Constant.StackSku1
                    && b.Pallet.Sku != Constant.StackSku2
                    && b.Pallet.Status != PalletStatus.Invalid);
        }

        public void GetPalletCountPerCrane(
            out int crane1Count,
            out int crane2Count,
            out int crane3Count,
            out int crane4Count)
        {
            crane1Count = GetPalletCount(CraneNumber.Crane1);
            crane2Count = GetPalletCount(CraneNumber.Crane2);
            crane3Count = GetPalletCount(CraneNumber.Crane3);
            crane4Count = GetPalletCount(CraneNumber.Crane4);
        }

        public int GetStackCount(
            string stackSku,
            CraneNumber craneNumber)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                stackSku,
                nameof(stackSku),
                new string[] { Constant.StackSku1, Constant.StackSku2 });

            return Items
                .Count(b =>
                {
                    PalletItem pallet = b.Pallet;
                    return b.CraneNumber == craneNumber
                        && pallet.Status != PalletStatus.Invalid
                        && pallet.Sku == stackSku;
                });
        }

        public void GetStackCountPerCrane(
            string stackSku,
            out int crane1Count,
            out int crane2Count,
            out int crane3Count,
            out int crane4Count)
        {
            crane1Count = GetStackCount(stackSku, CraneNumber.Crane1);
            crane2Count = GetStackCount(stackSku, CraneNumber.Crane2);
            crane3Count = GetStackCount(stackSku, CraneNumber.Crane3);
            crane4Count = GetStackCount(stackSku, CraneNumber.Crane4);
        }

        public List<string> GetAllSkusInStorage()
        {
            return Items
                .Select(b => b.Pallet.Sku)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }
        public List<string> GetAllPalletIDsInStorage()
    => Items
        .Select(b => b.Pallet.PalletID)
        .Where(p => !p.IsNullOrWhiteSpace())
        .Distinct()
        .OrderBy(p => p)
        .ToList();


    }

}

