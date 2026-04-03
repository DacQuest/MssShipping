using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.SnapInViews;
using Mss.Common;
using Mss.Data.LoadArchive;
using MicroOrm.Dapper.Repositories.SqlGenerator.Filters;

namespace Mss.Views
{
    public partial class ShipperArchiveView : XSnapInView
    {
        private BindingSource _bindingSource = new BindingSource();
        private List<LoadArchive> _loadArchives;
        private int _currentIndex = -1;
        private bool _useShipperReport = true;

        public ShipperArchiveView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {

            dataGrid.DataSource = _bindingSource;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.AutoSize = false;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            DataGridViewTextBoxColumn column;

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Date",
                DataPropertyName = "ShipDate",
                Name = "DateColumn",
                ReadOnly = true,
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Time",
                DataPropertyName = "ShipTime",
                Name = "TimeColumn",
                ReadOnly = true,
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Load Number",
                DataPropertyName = "LoadNumber",
                Name = "LoadNumberColumn",
                ReadOnly = true,
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Trailer ID",
                DataPropertyName = "TrailerID",
                Name = "TrailerIDColumn",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                MinimumWidth = 80
            };
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Pallet Count",
                DataPropertyName = "PalletCount",
                Name = "PalletCountColumn",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                MinimumWidth = 80
            };
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Duration",
                DataPropertyName = "FormattedPickTimeMinutes",
                Name = "FormattedPickTimeMinutesColumn",
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.Columns.Add(column);

            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            cmbReportType.SelectedIndex = 0;

            //             _UpdateGrid(true);
            cmbArchiveCount.SelectedIndex = 1;
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
        }

        private void _DataGrid_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGrid.CurrentCell.RowIndex;
            if (_loadArchives.Count >= index && index > -1)
            {
                _currentIndex = index;
                btnPrint.Enabled = true;
            }
            else
            {
                btnPrint.Enabled = false;
            }
        }

        private void _CmbArchiveCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cmbArchiveCount.SelectedIndex;
            if (index > -1)
            {
                string selectedItem = (string)cmbArchiveCount.SelectedItem;
                if (uint.TryParse(selectedItem.RightOfLast(' '), out uint count))
                {
                    UseWaitCursor = true;
                    try
                    {
                        if (!_FetchArchives(count, out _loadArchives))
                        {
                            MessageBox.Show(
                                this,
                                "Failed to fetch Shippers from Archive!",
                                "Error!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            cmbArchiveCount.SelectedIndex = -1;
                            return;
                        }
                        _bindingSource.DataSource = _loadArchives;
                    }
                    finally
                    {
                        UseWaitCursor = false;
                    }
                }
                else
                {
                    cmbArchiveCount.SelectedIndex = -1;
                }
            }
        }

        private bool _FetchArchives(uint count, out List<LoadArchive> loadArchives)
        {
            loadArchives = null;
            try
            {
                string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    LoadArchiveRepository loadRepository = new LoadArchiveRepository(connection);
                    LoadItemArchiveRepository loadItemRepository = new LoadItemArchiveRepository(connection);
                    LoadBroadcastArchiveRepository broadcastRepository = new LoadBroadcastArchiveRepository(connection);
                    LoadPalletArchiveRepository palletRepository = new LoadPalletArchiveRepository(connection);
                    loadArchives = loadRepository
                        .SetLimit(count)
                        .SetOrderBy(OrderInfo.SortDirection.DESC, l => l.LoadNumber)
                        .FindAll()
                        .ToList();
                    foreach (LoadArchive loadArchive in loadArchives)
                    {
                        IEnumerable<LoadItemArchive> loadItems = loadItemRepository.FindAll(l => l.LoadArchiveID == loadArchive.ID);

                        int minLoadItemArchiveID = loadItems.Min(l => l.ID);
                        int maxLoadItemArchiveID = loadItems.Max(l => l.ID);

                        IEnumerable<LoadBroadcastArchive> loadBroadcastArchives = broadcastRepository.FindAll(b =>
                            b.LoadItemArchiveID >= minLoadItemArchiveID && b.LoadItemArchiveID <= maxLoadItemArchiveID);
                        IEnumerable<LoadPalletArchive> loadPalletArchives = palletRepository.FindAll(p =>
                            p.LoadItemArchiveID >= minLoadItemArchiveID && p.LoadItemArchiveID <= maxLoadItemArchiveID);

                        foreach (LoadItemArchive loadItem in loadItems)
                        {
                            loadItem.Broadcast = loadBroadcastArchives.SingleOrDefault(b => b.LoadItemArchiveID == loadItem.ID);
                            loadItem.Pallet = loadPalletArchives.SingleOrDefault(p => p.LoadItemArchiveID == loadItem.ID);
                        }

                        loadArchive.LoadItems = loadItems.OrderBy(l => l.Broadcast.Csn).ToList();
                    }
                }
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent("Fetch Shipper Archives");
                return false;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_currentIndex > -1)
            {
                LoadArchive loadArchive = _loadArchives[_currentIndex];
                ShipperReport report = new ShipperReport(loadArchive);
                report.ShowPreview();
//                 if (_useShipperReport)
//                 {
//                     ShipperReport report = new ShipperReport(loadArchive);
//                     report.ShowPreview();
//                 }
//                 else
//                 {
//                     LoadManifestReport report = new LoadManifestReport(loadArchive);
//                     report.ShowPreview();
//                 }
            }
            else
            {
                btnPrint.Enabled = false;
            }
        }

        private void _CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = cmbReportType.SelectedIndex;
            if (selectedIndex == -1)
            {
                _useShipperReport = true;
                cmbReportType.SelectedIndex = 0;
                return;
            }
            _useShipperReport = selectedIndex == 0;
        }

        //protected override void AutoSubscribe()
        //{
        //}

        //public override Boolean ViewClosing(bool force)
        //{
        //    return true;
        //}

        //public override void ViewClosed()
        //{
        //}

        //protected override MenuStrip GetViewMenuStrip()
        //{
        //    return null;
        //}

        //protected override Boolean ProcessEnterKey()
        //{
        //    return false;
        //}

        //protected override Boolean ProcessEscapeKey()
        //{
        //    return false;
        //}

    }

}
