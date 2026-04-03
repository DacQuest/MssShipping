using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Mss.Data;
using Mss.Collections;
using Mss.Common;

namespace Mss.Views
{
    public partial class FullInventoryReport : XtraReport
    {
        public FullInventoryReport(List<BinItem> items)
        {
            InitializeComponent();

            DateTime now = DateTime.Now;
            xrLabelDateTime.Text = now.ToShortDateString() + "\n" + now.ToShortTimeString();
            DataSource = items
                .OrderBy(item => item.CraneNumber)
                .ThenBy(item => item.BinSideNumber)
                .ThenBy(item => item.BinHorizontalNumber)
                .ThenBy(item => item.BinVerticalNumber);
        }

    }
}
