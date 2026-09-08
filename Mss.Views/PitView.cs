using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.SnapInViews;
using DevExpress.Data.Selection;
using DevExpress.XtraGauges.Core.Customization;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mss.Views
{
    public partial class PitView : XSnapInView
    {
        private BindingSource _bindingSource = new BindingSource();
        private PitViewParameterSetWrapper _parameters;
        private PitProxy _pitProxy;
        private HoldCodesProxy _holdCodesProxy;
        private List<PitItem> _selectedPitItems = new List<PitItem>();
        private Font _rowHeaderFont = new Font("Segoe UI", 9, FontStyle.Bold);
        private readonly string _palletIDColumnName = "PalletIDColumn";

        private GridHeaderIndex _gridHeaderIndex = GridHeaderIndex.Pallet;
        private bool[] _columnSortAscending = { true, true, true, true, true, true, true, true };

        private enum GridHeaderIndex
        {
            PitCode = 0,
            Row,
            Pallet,
            Sku,
            JobID,
            Status,
            HoldCode,
            Added
        }

        public PitView()
        {
            InitializeComponent();

            _dgvPit.DataSource = _bindingSource;
            _dgvPit.AutoGenerateColumns = false;
            _dgvPit.AutoSize = false;
            _dgvPit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        protected override void OpenView()
        {
            XProxyCache.Acquire(_parameters.CollectionName, out _pitProxy);
            _pitProxy.DataItemChanged += _PitProxy_DataItemChanged;
            _pitProxy.CollectionRefreshed += _PitProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.HoldCodesName, out _holdCodesProxy);

            _dgvPit.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvPit.ColumnHeadersHeight = 30; // Set to desired height in pixels
            _dgvPit.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            _dgvPit.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);

//             DataGridViewImageColumn imageColumn;
            DataGridViewTextBoxColumn column;

//             imageColumn = new DataGridViewImageColumn
//             {
//                 DataPropertyName = "StatusImage",
//                 HeaderText = "",
//                 Name = "StatusImageColumn"
//             };
//             _ = _dgvPit.Columns.Add(imageColumn);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "PIT Code",
                DataPropertyName = "PitCode",
                Name = "PitCodeColumn",
                MinimumWidth = 120,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Row",
                DataPropertyName = "PalletVehicleRowText",
                Name = "VehicleRowColumn",
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Pallet",
                DataPropertyName = "PalletID",
                Name = _palletIDColumnName,
                MinimumWidth = 80,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "SKU",
                DataPropertyName = "PalletSku",
                Name = "SkuColumn",
                MinimumWidth = 120,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Job ID",
                DataPropertyName = "PalletJobID",
                Name = "JobIDColumn",
                MinimumWidth = 100,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "PalletStatusText",
                Name = "StatusColumn",
                MinimumWidth = 120,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Hold Code",
                DataPropertyName = "HoldCodeDescription",
                Name = "HoldCodeColumn",
                MinimumWidth = 250,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _dgvPit.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Added",
                DataPropertyName = "SetOnText",
                Name = "SetOnColumn",
                MinimumWidth = 160,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            _ = _dgvPit.Columns.Add(column);

            _RefreshGrid();

            _navigatorBtnAddItem.Visible = _parameters.AllowAdding;
            _navigatorBtnDelete.Visible = _parameters.AllowDeleting;
//             navigatorBtnQuickAdd.Visible = _parameters.AllowEditing
//                 && _parameters.CollectionName == Constant.InboundPitName;
            _navigatorBtnAddSingleEmpty.Visible = false;
        }

        private void _RefreshGrid()
        {
            _UpdateTitle(_pitProxy.Count);
            PitItem.SetHoldCodes(_holdCodesProxy.Values.ToDictionary(h => h.HoldCode, h => h.Description));
            IEnumerable<PitItem> pitItems = _pitProxy.Values;

            switch (_gridHeaderIndex)
            {
                case GridHeaderIndex.PitCode:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.PitCode.ToString())
                        : pitItems.OrderByDescending(p => p.PitCode.ToString());
                    break;
                case GridHeaderIndex.Added:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.SetOn)
                        : pitItems.OrderByDescending(p => p.SetOn);
                    break;
                case GridHeaderIndex.Row:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.Pallet.VehicleRow)
                        : pitItems.OrderByDescending(p => p.Pallet.VehicleRow);
                    break;
                case GridHeaderIndex.Sku:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.Pallet.Sku)
                        : pitItems.OrderByDescending(p => p.Pallet.Sku);
                    break;
                case GridHeaderIndex.JobID:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.Pallet.JobID)
                        : pitItems.OrderByDescending(p => p.Pallet.JobID);
                    break;
                case GridHeaderIndex.Status:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.Pallet.Status)
                        : pitItems.OrderByDescending(p => p.Pallet.Status);
                    break;
                case GridHeaderIndex.HoldCode:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.Pallet.HoldCode)
                        : pitItems.OrderByDescending(p => p.Pallet.HoldCode);
                    break;
                case GridHeaderIndex.Pallet:
                default:
                    pitItems = _columnSortAscending[(int)_gridHeaderIndex]
                        ? pitItems.OrderBy(p => p.PalletID)
                        : pitItems.OrderByDescending(p => p.PalletID);
                    break;
            }

            _dgvPit.SuspendLayout();
//             _bindingSource.DataSource = pitItems.OrderBy(p => p.PalletID);
            if (pitItems.Count() == 0)
            {
                pitItems = null;
            }
            _bindingSource.DataSource = pitItems;



            List<PitItem> selectedPitItems = _selectedPitItems.ToList();
            List<string> selectedPalletIDs = selectedPitItems.Select(e => e.PalletID).ToList();

            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in _dgvPit.Rows)
            {
                PitItem pitItem = (PitItem)row.DataBoundItem;
                if (selectedPalletIDs.Contains(pitItem.PalletID))
                {
                    selectedRows.Add(row);
                }
            }
            if (_dgvPit.Rows.Count == 0)
            {
                //selectedRow.Selected = true;
                //_currentEvent = selectedRow.DataBoundItem as XSystemEventMessageData;
//                _selectedEvents.Clear();
            }
            else if (_dgvPit.Rows.Count == 1)
            {
                _dgvPit.Rows[0].Selected = true;
//                _selectedEvents.Clear();
//                selectedEvents.Add((XSystemEventMessageData)dgvSystemEvents.Rows[0].DataBoundItem);
            }
            else
            {
                List<PitItem> pitItemsToRemove = new List<PitItem>();
                foreach (PitItem pitItem in selectedPitItems)
                {
                    if (!selectedRows.Any(r => ((PitItem)r.DataBoundItem).PalletID == pitItem.PalletID))
                    {
                        pitItemsToRemove.Add(pitItem);
                    }
                }
                foreach (PitItem removeItem in pitItemsToRemove)
                {
                    _ = selectedPitItems.Remove(removeItem);
                }
                foreach (DataGridViewRow row in _dgvPit.Rows)
                {
                    row.Selected = false;
                }
                foreach (DataGridViewRow row in selectedRows)
                {
                    row.Selected = true;
                }
            }

            _dgvPit.Update();
            _dgvPit.ResumeLayout();
            _navigatorBtnDelete.Enabled = selectedRows.Count > 0;
        }

        private void _PitProxy_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _RefreshGrid();
        }

        private void _PitProxy_CollectionRefreshed(object sender, EventArgs e)
        {
            _RefreshGrid();
        }

        private void _UpdateTitle(int rowCount)
        {
            string collectionName = _parameters.DisplayName;
            _lblCollectionName.Text = rowCount == 1
                ? $"{collectionName}  ( 1 Pallet )"
                : $"{collectionName}  ( {rowCount} Pallets )";
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (PitViewParameterSetWrapper)parameters;
        }

        private void _NavigatorBtnDelete_Click(object sender, EventArgs e)
        {
            List<string> palletIDs = new List<string>();
            foreach (DataGridViewRow row in _dgvPit.SelectedRows)
            {
                palletIDs.Add(row.Cells[_palletIDColumnName].Value.ToString());
            }

            if(palletIDs.Count > 0)
            {
                string pitName = _pitProxy.CollectionConfiguration.FriendlyName;
                if (palletIDs.Count == 1)
                {
                    using (PitDeleteForm form = new PitDeleteForm(palletIDs[0], pitName))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            _ = _pitProxy.Remove(palletIDs[0]);
                        }
                    }
                }
                else // count > 1
                {
                    using (PitMultiDeleteForm form = new PitMultiDeleteForm(pitName))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string palletID in palletIDs)
                            {
                                _ = _pitProxy.Remove(palletID);
                            }
                        }
                    }
                }
            }
            _navigatorBtnDelete.Enabled = false;
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _holdCodesProxy.Refresh();
            _pitProxy.Refresh();
        }

        private void _NavigatorBtnAddItem_Click(object sender, EventArgs e)
        {
            Levels level = Levels.None;
            switch (_pitProxy.CollectionName)
            {
                case Constant.UpperPitName:
                    level = Levels.Upper;
                    break;
                case Constant.LowerPitName:
                    level = Levels.Upper;
                    break;
                case Constant.AssignmentPitName:
                    level = Levels.None;
                    break;
            }
            using (PitAddForm form = new PitAddForm(level, _pitProxy, _holdCodesProxy.Values))
            {
                if (form.ShowDialog()==DialogResult.OK)
                {
                    // Nothing needed.
                }
            }

        }

        private void _NavigatorBtnQuickAdd_Click(object sender, EventArgs e)
        {
//             NumberEntryForm form = new NumberEntryForm("Add Pallet", 0);
//             if (form.ShowDialog(this) == DialogResult.OK)
//             {
//                 if (!MesInterface.FetchPalletItem(form.Value, out PalletItem palletItem))
//                 {
//                     XMessageBox.Show(
//                         this,
//                         "No data exists for this pallet.",
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                     return;
//                 }
//                 _pitProxy.Update(palletItem.PalletID, palletItem);
//             }
//             form.Dispose();
        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            if (_pitProxy != null)
            {
                _pitProxy.DataItemChanged -= _PitProxy_DataItemChanged;
                _pitProxy.CollectionRefreshed -= _PitProxy_CollectionRefreshed;
                _pitProxy.Close();
                _pitProxy = null;
            }
            if (_holdCodesProxy != null)
            {
                _holdCodesProxy.Close();
                _holdCodesProxy = null;
            }
            return true;
        }

        private void _NavigatorBtnAddSingleEmpty_Click(object sender, EventArgs e)
        {
//             NumberEntryForm form = new NumberEntryForm("Enter Unique ID", 0);
//             if (form.ShowDialog(this) != DialogResult.OK)
//             {
//                 return;
//             }
//             int palletID = form.Value;
//             form.Dispose();
//             if (palletID <= 0)
//             {
//                 return;
//             }
//             string sku = string.Empty;
//             string comment = string.Empty;
//             if (XValueValidator.Validate(Constant.PalletIDValidatorName, palletID))
//             {
//                 //sku = XConfiguration.GetAlias(Constant.FirstRowEmptyPalletSkuName);
//                 comment = "Front Row Empty Pallet";
//             }
//             else
//             {
//                 XMessageBox.Show(
//                     this,
//                     "Not a valid Unique ID.",
//                     "Error",
//                     MessageBoxButtons.OK,
//                     MessageBoxIcon.Error);
//                 return;
//             }
//             PalletItem palletItem = new PitItem
//             {
//                 PalletID = palletID,
//                 Sku = sku,
//                 Status = PalletStatus.OK,
//                 InitialStatus = PalletStatus.OK,
//                 ReceivedOn = DateTime.Now,
//                 Comment = comment
//             };
// 
//             _pitProxy.Update(palletItem.PalletID, palletItem);
        }

        private void _DgvPit_SelectionChanged(object sender, EventArgs e)
        {
            _selectedPitItems.Clear();

            if (_dgvPit.SelectedRows.Count > 0 )
            {
                _selectedPitItems.AddRange(_dgvPit
                    .SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(p => (PitItem)p.DataBoundItem));
            }

            _navigatorBtnDelete.Enabled = _selectedPitItems.Count > 0;
        }

        private void _DgvPit_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left
                || e.RowIndex >= 0)
            {
                return;
            }
            int index = (int)_gridHeaderIndex;
            if (index == e.ColumnIndex)
            {
                _columnSortAscending[index] = !_columnSortAscending[index];
            }
            _gridHeaderIndex = (GridHeaderIndex)e.ColumnIndex;

            _RefreshGrid();
        }

        public override void ViewClosed()
        {
            _rowHeaderFont?.Dispose();
            base.ViewClosed();
        }

        private void _DgvPit_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            string rowNum = (e.RowIndex + 1).ToString();

            StringFormat centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle headerBounds = new Rectangle(
                e.RowBounds.Left,
                e.RowBounds.Top,
                grid.RowHeadersWidth,
                e.RowBounds.Height);

            e.Graphics.DrawString(
                rowNum,
                _rowHeaderFont,
                SystemBrushes.ControlText,
                headerBounds,
                centerFormat);
        }

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
