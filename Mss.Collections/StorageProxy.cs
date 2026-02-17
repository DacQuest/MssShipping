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

        public int GetEmptyBinCount(CraneNumber craneNumber)
        {
            return Items
                .Count(b =>
                {
                    return b.CraneNumber == craneNumber
                        && b.BinStatus == BinStatus.Empty
                        && !b.Audit
                        && !b.PickOnly
                        && !b.Disabled
                        && !b.NotUsable;
                });
        }

        public void GetEmptyBinCountPerCrane(
            out int crane1Count,
            out int crane2Count,
            out int crane3Count,
            out int crane4Count)
        {
            crane1Count = GetEmptyBinCount(CraneNumber.Crane1);
            crane2Count = GetEmptyBinCount(CraneNumber.Crane2);
            crane3Count = GetEmptyBinCount(CraneNumber.Crane3);
            crane4Count = GetEmptyBinCount(CraneNumber.Crane4);
        }

        public int GetPalletCount(CraneNumber craneNumber)
        {
            return Items
                .Count(b => b.CraneNumber == craneNumber
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

