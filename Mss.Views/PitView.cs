using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.SnapInViews;
using DevExpress.Data.Selection;
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

        private readonly string _palletIDColumnName = "PalletIDColumn";

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
            _pitProxy.DataItemChanged += _PalletsProxy_DataItemChanged;
            _pitProxy.CollectionRefreshed += _PalletsProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.HoldCodesName, out _holdCodesProxy);

            _dgvPit.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvPit.ColumnHeadersHeight = 30; // Set to desired height in pixels
            _dgvPit.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);

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
                HeaderText = "Added On",
                DataPropertyName = "SetOnText",
                Name = "SetOnColumn",
                MinimumWidth = 160,
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
                MinimumWidth = 200,
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            _ = _dgvPit.Columns.Add(column);

            _UpdateGrid();

            navigatorBtnAddItem.Visible = _parameters.AllowAdding;
            navigatorBtnDelete.Visible = _parameters.AllowDeleting;
//             navigatorBtnQuickAdd.Visible = _parameters.AllowEditing
//                 && _parameters.CollectionName == Constant.InboundPitName;
            navigatorBtnAddSingleEmpty.Visible = false;
        }

        private void _UpdateGrid()
        {
            List<PitItem> pitItems = _pitProxy.Values;

            PitItem.SetHoldCodes(_holdCodesProxy.Values.ToDictionary(h => h.HoldCode, h => h.Description));

            UpdateTitle(_pitProxy.Count);

            _bindingSource.DataSource = pitItems;
            _dgvPit.Update();
        }

        private void _PalletsProxy_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _UpdateGrid();
        }

        private void _PalletsProxy_CollectionRefreshed(object sender, EventArgs e)
        {
            _UpdateGrid();
        }

        public void UpdateTitle(int rowCount)
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
            navigatorBtnDelete.Enabled = false;
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _pitProxy.Refresh();
            _holdCodesProxy.Refresh();
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
                _pitProxy.DataItemChanged -= _PalletsProxy_DataItemChanged;
                _pitProxy.CollectionRefreshed -= _PalletsProxy_CollectionRefreshed;
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
            navigatorBtnDelete.Enabled = _dgvPit.SelectedRows.Count > 0;
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
