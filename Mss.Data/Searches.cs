using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Strings;

using Mss.Collections;
using Mss.Common;

namespace Mss.Data
{
    public static class Searches
    {

        public static List<BinItem> DuplicatePallets(List<BinItem> binItems)
        {
            List<BinItem> duplicates = new List<BinItem>();
            foreach (IGrouping<string, BinItem> group in binItems
                .Where(b => b.Pallet.PalletID.ValidPalletID())
                .GroupBy(b => b.Pallet.PalletID)
                .Where(g => g.Count() > 1))
            {
                duplicates.AddRange(group);
            }
            return duplicates.OrderBy(b => b.Pallet.PalletID).ToList();
        }

        public static List<BinItem> Audits(List<BinItem> binItems)
        {
            return binItems.Where(b => b.Audit).ToList();
        }

        public static List<BinItem> PickOnly(List<BinItem> binItems)
        {
            return binItems.Where(b => b.PickOnly).ToList();
        }

        public static List<BinItem> Disabled(List<BinItem> binItems)
        {
            return binItems.Where(b => b.Disabled).ToList();
        }

        public static List<BinItem> ByCrane(
            List<BinItem> binItems,
            CraneNumber craneNumber)
        {
            return binItems.Where(b => b.CraneNumber == craneNumber).ToList();
        }

        public static List<BinItem> BySku(
            List<BinItem> binItems,
            string sku)
        {
            return binItems.Where(b =>
            {
                return sku == Constant.StackSku
                    ? sku == b.Pallet.Sku
                    : sku.Contains(b.Pallet.Sku);

            }).ToList();
        }

        public static List<BinItem> BySkus(
            List<BinItem> binItems,
            List<String> skus)
        {
            return binItems.Where(b => skus.Contains(b.Pallet.Sku)).ToList();
        }

        public static List<BinItem> ByPalletID(
            List<BinItem> binItems,
            string palletID)
        {
            return binItems.Where(b => b.Pallet.PalletID == palletID).ToList();
        }

        public static List<BinItem> ByPalletStatus(
            List<BinItem> binItems,
            PalletStatus statuses)
        {
            return binItems.Where(b => statuses.IsFlagSet(b.Pallet.Status)).ToList();
        }

        public static List<BinItem> ByBinStatus(
            List<BinItem> binItems,
            BinStatus statuses)
        {
            return binItems.Where(b => statuses.IsFlagSet(b.BinStatus)).ToList();
        }

        public static List<BinItem> ByVehicleRow(
            List<BinItem> binItems,
            List<VehicleRow> vehicleRows)
        {
            return binItems.Where(b => vehicleRows.Any(r => r == b.Pallet.VehicleRow)).ToList();
        }

        // By Bin Reservation
        //        public static List<BinItem> ByOfflineBin(
        //            List<BinItem> binItems)
        //        {
        //            return binItems.Where(item => item.OfflineBin).ToList();
        //        }

        public static List<BinItem> ByBuiltOn(
            List<BinItem> binItems,
            DateTime start,
            DateTime end)
        {
            return binItems.Where(
                b =>
                {
                    DateTime builtOnDateTime = b.Pallet.BuiltOn;
                    return builtOnDateTime >= start && builtOnDateTime <= end;
                }).ToList();
        }

    }
}
