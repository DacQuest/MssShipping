using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DacQuest.DFX.Core.Strings;
using Mss.Common;
using Mss.Collections;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Data.LoadArchive
{
    public partial class ShipperReport : XtraReport
    {
        public ShipperReport(LoadArchive loadArchive)
        {
            InitializeComponent();
            _PrepareReport(loadArchive);
        }

        private void _PrepareReport(LoadArchive loadArchive)
        {
            xrLblDate.Text = loadArchive.ArchivedOn.ToString("MM/dd/yyyy");
            xrLblTime.Text = loadArchive.ArchivedOn.ToString("HH:mm:ss");

            xrLblTrailerNumber.Text = loadArchive.TrailerNumber.ToString();
            xrLblShipmentNumber.Text = loadArchive.LoadNumber.ToString();

//             xrLblNetWeight.Text = XConfiguration.GetAlias(Constant.ShipperReportNetWeightAliasName);
//             xrLblTareWeight.Text = XConfiguration.GetAlias(Constant.ShipperReportTareWeightAliasName);
//             xrLblGrossWeight.Text = XConfiguration.GetAlias(Constant.ShipperReportGrossWeightAliasName);


            string lowCsn = loadArchive
                .LoadItems.Where(l => l.Status > LoadItemStatus.Invalid)
                .Min(l => l.Broadcast.Csn)
                .ToString();
            string highCsn = loadArchive
                .LoadItems.Where(l => l.Status > LoadItemStatus.Invalid)
                .Max(l => l.Broadcast.Csn)
                .ToString();

            xrLblQuantity.Text = (loadArchive.PalletCount / 2).ToString();

            xrLblLowCsn.Text = lowCsn;
            xrLblHighCsn.Text = highCsn;
//             string barcodeValue = string.Format(
//                 "T{0}F{1}L{2}P{3}",
//                 loadArchive.TrailerNumber,
//                 lowCsn,
//                 highCsn,
//                 loadArchive.PreviousRotationNumber);
            string barcodeValue = "Not Implemented!";
            xrBarCode.Text = barcodeValue;
            xrLblBarcodeValue.Text = barcodeValue;

        }

    }
}
