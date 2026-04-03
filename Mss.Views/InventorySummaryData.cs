using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Views
{
    public class InventorySummaryData
    {
        public string Sku { get; private set; }
        public int Total { get; set; } = 0;
        public int Crane1 { get; set; } = 0;
        public int Crane2 { get; set; } = 0;
        public int Crane3 { get; set; } = 0;
        public int Crane4 { get; set; } = 0;
        public int UpperPit { get; set; } = 0;
        public int LowerPit { get; set; } = 0;
        public int OK { get; set; } = 0;
        public int Hold { get; set; } = 0;
        public int Reserved { get; set; } = 0;
        public int Purge { get; set; } = 0;
        public int Unknown { get; set; } = 0;

        public string TotalText
        {
            get
            {
                Total = Crane1 + Crane2 + Crane3 + Crane4;
                return Total == 0 ? string.Empty : Total.ToString();
            }
        }

        public string Crane1Text => Crane1 == 0 ? string.Empty : Crane1.ToString();
        public string Crane2Text => Crane2 == 0 ? string.Empty : Crane2.ToString();
        public string Crane3Text => Crane3 == 0 ? string.Empty : Crane3.ToString();
        public string Crane4Text => Crane4 == 0 ? string.Empty : Crane4.ToString();
        public string UpperPitText => UpperPit == 0 ? string.Empty : UpperPit.ToString();
        public string LowerPitText => LowerPit == 0 ? string.Empty : LowerPit.ToString();
        public string OKText => OK == 0 ? string.Empty : Total.ToString();
        public string HoldText => Hold == 0 ? string.Empty : Hold.ToString();
        public string ReservedText => Reserved == 0 ? string.Empty : Reserved.ToString();
        public string PurgeText => Purge == 0 ? string.Empty : Purge.ToString();
        public string UnknownText => Unknown == 0 ? string.Empty : Unknown.ToString();
        public InventorySummaryData(string sku)
        {
            Sku = sku;
        }

        public static List<InventorySummaryData> FetchSummaryData(
            StorageProxy storageProxy,
            PitProxy upperPitProxy,
            PitProxy lowerPitProxy)
        {
            List<InventorySummaryData> summaryData = new List<InventorySummaryData>();
            List<string> storageSkus = storageProxy.GetAllSkusInStorage();
            List<string> upperPitSkus = upperPitProxy.GetAllSkusInPit();
            List<string> lowerPitSkus = lowerPitProxy.GetAllSkusInPit();
            List<string> skus = storageSkus
                .Concat(upperPitSkus)
                .Concat(lowerPitSkus)
                .Distinct()
                .ToList();
            foreach (string sku in skus)
            {
                InventorySummaryData data = new InventorySummaryData(sku);
                foreach (BinItem bin in storageProxy.Items)
                {
                    PalletItem pallet = bin.Pallet;
                    if (pallet.Sku != sku)
                    {
                        continue;
                    }
                    switch (bin.CraneNumber)
                    {
                        case CraneNumber.Crane1:
                            data.Crane1++;
                            break;
                        case CraneNumber.Crane2:
                            data.Crane2++;
                            break;
                        case CraneNumber.Crane3:
                            data.Crane3++;
                            break;
                        case CraneNumber.Crane4:
                            data.Crane4++;
                            break;
                    }
                    switch (pallet.Status)
                    {
                        case PalletStatus.OK:
                            data.OK++;
                            break;
                        case PalletStatus.Hold:
                            data.Hold++;
                            break;
                        case PalletStatus.Reserved:
                            data.Reserved++;
                            break;
                        case PalletStatus.Purge:
                            data.Purge++;
                            break;
                        case PalletStatus.Unknown:
                            data.Unknown++;
                            break;
                    }
                }
                int pitCount = upperPitProxy
                    .Values
                    .Count(p =>
                    {
                        PalletItem pallet = p.Pallet;
                        return p.PitCode.IsAssigned()
                            && pallet.Status == PalletStatus.OK
                            && pallet.Sku == sku;

                    });
                data.UpperPit += pitCount;
                pitCount = lowerPitProxy
                    .Values
                    .Count(p =>
                    {
                        PalletItem pallet = p.Pallet;
                        return p.PitCode.IsAssigned()
                            && pallet.Status == PalletStatus.OK
                            && pallet.Sku == sku;

                    });
                data.LowerPit += pitCount;
                summaryData.Add(data);
            }
            return summaryData.OrderBy(item => item.Sku).ToList();
        }

    }
}
