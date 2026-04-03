using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Mss.Collections;
using Mss.Common;

namespace Mss.Views
{
    public partial class InventorySummaryReport : XtraReport
    {
        public InventorySummaryReport(
            StorageProxy storageProxy,
            PitProxy upperPitProxy,
            PitProxy lowerPitProxy)
        {
            InitializeComponent();
            DateTime now = DateTime.Now;
            xrLabelDateTime.Text = now.ToShortDateString() + "\n" + now.ToShortTimeString();

//             List<InventorySummaryData> reportData = new List<InventorySummaryData>();
//             List<string> storageSkus = storageProxy.GetAllSkusInStorage();
//             List<string> upperPitSkus = upperPitProxy.GetAllSkusInPit();
//             List<string> lowerPitSkus = lowerPitProxy.GetAllSkusInPit();
//             List<string> skus = storageSkus
//                 .Concat(upperPitSkus)
//                 .Concat(lowerPitSkus)
//                 .Distinct()
//                 .ToList();
//             foreach (string sku in skus)
//             {
//                 InventorySummaryData data = new InventorySummaryData(sku);
//                 foreach (BinItem bin in storageProxy.Items)
//                 {
//                     PalletItem pallet = bin.Pallet;
//                     if (pallet.Sku != sku)
//                     {
//                         continue;
//                     }
//                     switch (bin.CraneNumber)
//                     {
//                         case CraneNumber.Crane1:
//                             data.Crane1++;
//                             break;
//                         case CraneNumber.Crane2:
//                             data.Crane2++;
//                             break;
//                         case CraneNumber.Crane3:
//                             data.Crane3++;
//                             break;
//                         case CraneNumber.Crane4:
//                             data.Crane4++;
//                             break;
//                     }
//                     switch (pallet.Status)
//                     {
//                         case PalletStatus.OK:
//                             data.OK++;
//                             break;
//                         case PalletStatus.Hold:
//                             data.Hold++;
//                             break;
//                         case PalletStatus.Reserved:
//                             data.Reserved++;
//                             break;
//                         case PalletStatus.Purge:
//                             data.Purge++;
//                             break;
//                         case PalletStatus.Unknown:
//                             data.Unknown++;
//                             break;
//                     }
//                 }
//                 int pitCount = upperPitProxy
//                     .Values
//                     .Count(p =>
//                     {
//                         PalletItem pallet = p.Pallet;
//                         return p.PitCode.IsAssigned()
//                             && pallet.Status == PalletStatus.OK
//                             && pallet.Sku == sku;
// 
//                     });
//                 data.UpperPit += pitCount;
//                 pitCount = lowerPitProxy
//                     .Values
//                     .Count(p =>
//                     {
//                         PalletItem pallet = p.Pallet;
//                         return p.PitCode.IsAssigned()
//                             && pallet.Status == PalletStatus.OK
//                             && pallet.Sku == sku;
// 
//                     });
//                 data.LowerPit += pitCount;
//                 reportData.Add(data);
//             }
            DataSource = InventorySummaryData.FetchSummaryData(
                storageProxy,
                upperPitProxy,
                lowerPitProxy);
        }

//         public InventorySummaryReport(List<BinItem> items)
//         {
//             InitializeComponent();
//             DateTime now = DateTime.Now;
//             xrLabelDateTime.Text = now.ToShortDateString() + "\n" + now.ToShortTimeString();
// 
//             Dictionary<string, InventorySummaryData> reportData = new Dictionary<string, InventorySummaryData>();
// 
//             foreach (BinItem item in items)
//             {
//                 PalletItem palletItem = item.Pallet;
//                 if (palletItem.Status != PalletStatus.Invalid)
//                 {
//                     if (!reportData.TryGetValue(palletItem.Sku, out InventorySummaryData data))
//                     {
//                         data = new InventorySummaryData(palletItem.Sku);
//                         reportData.Add(data.Sku, data);
//                     }
//                     switch (palletItem.Status)
//                     {
//                         case PalletStatus.OK:
//                             data.OK++;
//                             break;
//                         case PalletStatus.Hold:
//                             data.Hold++;
//                             break;
//                         case PalletStatus.QAPick:
//                             data.QAPick++;
//                             break;
//                         case PalletStatus.Unknown:
//                             data.Unknown++;
//                             break;
//                     }
//                     switch (item.CraneNumber)
//                     {
//                         case CraneNumber.Crane1:
//                             data.Crane1++;
//                             break;
//                         case CraneNumber.Crane2:
//                             data.Crane2++;
//                             break;
//                     }
//                 }
//             }
// 
//             DataSource = reportData.OrderBy(item => item.Value.Sku).Select(item => item.Value).ToList();
//         }

    }
}
