using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Mss.Collections;

namespace Mss.Views
{
    public partial class SearchResultsReport : DevExpress.XtraReports.UI.XtraReport
    {
        public SearchResultsReport(List<BinItem> items)
        {
            InitializeComponent();

            DateTime now = DateTime.Now;
            xrLabelDateTime.Text = now.ToShortDateString() + "\n" + now.ToShortTimeString();
//            DataSource = items.OrderBy(item => item.BinAisleNumber).ToList();
            DataSource = items;
        }

    }
}
