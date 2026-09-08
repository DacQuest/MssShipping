using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;
// using DevExpress.XtraRichEdit.Model;
using Mss.Common;
using DevExpress.XtraReports.UI;
using SourceGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// using DevExpress.ChartRangeControlClient.Core;

namespace Mss.Views
{
    public partial class InventorySummaryView : XSnapInView
    {
        private readonly BindingSource _bindingSource = new BindingSource();
        private StorageProxy _storageProxy;
        private PitProxy _upperPitProxy;
        private PitProxy _lowerPitProxy;

        public InventorySummaryView()
        {
            InitializeComponent();

            _dgvInventory.DataSource = _bindingSource;
            _dgvInventory.AutoGenerateColumns = false;
            _dgvInventory.AutoSize = false;
            _dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        protected override void OpenView()
        {
            XProxyCache.Acquire(Constant.StorageName, out _storageProxy);
            _storageProxy.DataItemChanged += _StorageProxy_DataItemChanged;
            _storageProxy.CollectionRefreshed += _StorageProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.UpperPitName, out _upperPitProxy);
            _upperPitProxy.DataItemChanged += _PitProxy_DataItemChanged;
            _upperPitProxy.CollectionRefreshed += _PitProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.LowerPitName, out _lowerPitProxy);
            _lowerPitProxy.DataItemChanged += _PitProxy_DataItemChanged;
            _lowerPitProxy.CollectionRefreshed += _PitProxy_CollectionRefreshed;

//             _dgvInventory.EnableHeadersVisualStyles = false;
            _dgvInventory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvInventory.ColumnHeadersHeight = 30;
            _dgvInventory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);


            _dgvInventory.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);

//             _dgvInventory.RowsDefaultCellStyle.BackColor = Color.LightGray;
//             _dgvInventory.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

//             _dgvInventory.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
//             _dgvInventory.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

            DataGridViewTextBoxColumn column;

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "SKU",
                DataPropertyName = "Sku",
                Name = "SkuColumn",
                MinimumWidth = 110,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = "TotalText",
                Name = "TotalColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Crane 1",
                DataPropertyName = "Crane1Text",
                Name = "Crane1Column",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Crane 2",
                DataPropertyName = "Crane2Text",
                Name = "Crane2Column",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Crane 3",
                DataPropertyName = "Crane3Text",
                Name = "Crane3Column",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Crane 4",
                DataPropertyName = "Crane4Text",
                Name = "Crane4Column",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Upper PIT",
                DataPropertyName = "UpperPitText",
                Name = "UpperPitColumn",
                MinimumWidth = 110,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Lower PIT",
                DataPropertyName = "LowerPitText",
                Name = "LowerPitColumn",
                MinimumWidth = 110,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "OK",
                DataPropertyName = "OKText",
                Name = "OKColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Reserved",
                DataPropertyName = "ReservedText",
                Name = "ReservedColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Hold",
                DataPropertyName = "HoldText",
                Name = "HoldColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Purge",
                DataPropertyName = "PurgeText",
                Name = "PurgeColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
            };
            _ = _dgvInventory.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Unknown",
                DataPropertyName = "UnknownText",
                Name = "UnknownColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            _ = _dgvInventory.Columns.Add(column);

            //var boldHeaderFont = new Font(
            //_dgvInventory.ColumnHeadersDefaultCellStyle.Font,
            //FontStyle.Bold);

            //foreach (DataGridViewColumn col in _dgvInventory.Columns)
            //{
            //    col.HeaderCell.Style.Font = boldHeaderFont;
            //}

            _storageProxy.Refresh();
            _upperPitProxy.Refresh();
            _lowerPitProxy.Refresh();
        }

        private void _PitProxy_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _StorageProxy_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _StorageProxy_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _PitProxy_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateGrid();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _storageProxy.Refresh();
            _upperPitProxy.Refresh();
            _lowerPitProxy.Refresh();
        }

        private void _UpdateGrid()
        {
//             List<InventorySummaryData> summaryData = new List<InventorySummaryData>();
//             List<string> storageSkus = _storageProxy.GetAllSkusInStorage();
//             List<string> upperPitSkus = _upperPitProxy.GetAllSkusInPit();
//             List<string> lowerPitSkus = _lowerPitProxy.GetAllSkusInPit();
//             List<string> skus = storageSkus
//                 .Concat(upperPitSkus)
//                 .Concat(lowerPitSkus)
//                 .Distinct()
//                 .ToList();
//             foreach (string sku in skus)
//             {
//                 InventorySummaryData data = new InventorySummaryData(sku);
//                 foreach (BinItem bin in _storageProxy.Items)
//                 {
//                     PalletItem pallet = bin.Pallet;
//                     if (pallet.Sku == sku)
//                     {
//                         switch (bin.CraneNumber)
//                         {
//                             case CraneNumber.Crane1:
//                                 data.Crane1++;
//                                 break;
//                             case CraneNumber.Crane2:
//                                 data.Crane2++;
//                                 break;
//                         }
//                         switch (pallet.Status)
//                         {
//                             case PalletStatus.OK:
//                                 data.OK++;
//                                 break;
//                             case PalletStatus.Hold:
//                                 data.Hold++;
//                                 break;
//                             case PalletStatus.Reserved:
//                                 data.Reserved++;
//                                 break;
//                             case PalletStatus.Purge:
//                                 data.Purge++;
//                                 break;
//                             case PalletStatus.Unknown:
//                                 data.Unknown++;
//                                 break;
//                         }
//                     }
//                 }
//                 int pitCount = _pitProxy
//                     .Values
//                     .Count(p =>
//                     {
//                         PalletItem pallet = p.Pallet;
//                         return p.PitCode.IsAssigned()
//                             && pallet.Status == PalletStatus.OK
//                             && pallet.Sku == sku;
// 
//                     });
//                 data.Pit += pitCount;
//                 summaryData.Add(data);
//             }

            _bindingSource.DataSource = InventorySummaryData.FetchSummaryData(
                _storageProxy,
                _upperPitProxy,
                _lowerPitProxy);
            _dgvInventory.Update();
        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            if (_storageProxy != null)
            {
                _storageProxy.DataItemChanged -= _StorageProxy_DataItemChanged;
                _storageProxy.CollectionRefreshed -= _StorageProxy_CollectionRefreshed;
                XProxyCache.Release(_storageProxy);
                _storageProxy = null;
            }
            if (_upperPitProxy != null)
            {
                _upperPitProxy.DataItemChanged -= _PitProxy_DataItemChanged;
                _upperPitProxy.CollectionRefreshed -= _PitProxy_CollectionRefreshed;
                XProxyCache.Release(_upperPitProxy);
                _upperPitProxy = null;
            }
            if (_lowerPitProxy != null)
            {
                _lowerPitProxy.DataItemChanged -= _PitProxy_DataItemChanged;
                _lowerPitProxy.CollectionRefreshed -= _PitProxy_CollectionRefreshed;
                XProxyCache.Release(_lowerPitProxy);
                _lowerPitProxy = null;
            }
            return true;
        }

        private void _InventorySummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InventorySummaryReport report = new InventorySummaryReport(
                _storageProxy,
                _upperPitProxy,
                _lowerPitProxy);
            ReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowPreview();
        }

        private void _FullInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullInventoryReport report = new FullInventoryReport(_storageProxy.Items);
            IReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowPreview();
        }

        //public override void ViewClosed()
        //{
        //}

        //protected override MenuStrip GetViewMenuStrip()
        //{
        //    return null;
        //}

        //protected override bool ProcessEnterKey()
        //{
        //    return false;
        //}

        //protected override bool ProcessEscapeKey()
        //{
        //    return false;
        //}

    }
}
