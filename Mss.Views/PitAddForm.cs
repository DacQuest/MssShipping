using DacQuest.DFX.Core;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Strings;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mss.Views
{
    public partial class PitAddForm : Form
    {
        private readonly Levels _level = Levels.None;
        private readonly PitProxy _pitProxy;
        private List<HoldCodeItem> _holdCodes;
        private string _palletID = string.Empty;
        private PitCode _pitCode = PitCode.Unknown;
        private PalletStatus _newPalletStatus = PalletStatus.Invalid;
        private int _holdCode = Constant.NoHoldCode;
        private bool _overrideStatus = false;
        private PitItem _pitItem = null;


        public PitAddForm(Levels level, PitProxy pitProxy, List<HoldCodeItem> holdCodes)
        {
            InitializeComponent();

            _level = level;
            _pitProxy = pitProxy;
            _holdCodes = holdCodes;

            _cmbNewPalletStatus.AddEnumItem(PalletStatus.OK);
            _cmbNewPalletStatus.AddEnumItem(PalletStatus.Reserved);
            _cmbNewPalletStatus.AddEnumItem(PalletStatus.Hold);
            _cmbNewPalletStatus.AddEnumItem(PalletStatus.Purge);

            _PopulatePitCodeCombo();

            _cmbHoldCode.ValueMember = nameof(HoldCodeItem.HoldCode);
            _cmbHoldCode.DisplayMember = nameof(HoldCodeItem.Description);
            _cmbHoldCode.DataSource = _holdCodes.OrderBy(h => h.Description).ToArray();
            _cmbHoldCode.SelectedItem = Constant.NoHoldCode;
        }

        private void _PopulatePitCodeCombo()
        {
            if (_pitProxy.CollectionName == Constant.AssignmentPitName)
            {
                _cmbPitCode.AddEnumItem(PitCode.Lower);
                _cmbPitCode.AddEnumItem(PitCode.Upper);
                _cmbPitCode.AddEnumItem(PitCode.Twenty);
                _cmbPitCode.AddEnumItem(PitCode.Purge);
            }
            else
            {
                _cmbPitCode.AddEnumItem(PitCode.Assigned1);
                _cmbPitCode.AddEnumItem(PitCode.Assigned2);
                _cmbPitCode.AddEnumItem(PitCode.Assigned3);
                _cmbPitCode.AddEnumItem(PitCode.Assigned4);
                _cmbPitCode.AddEnumItem(PitCode.Purge);
                _cmbPitCode.AddEnumItem(PitCode.Stack);
            }
            _cmbPitCode.SelectedIndex = -1;
        }

        private void _EnableAddButton()
        {
            bool enabled = _palletID.ValidPalletID()
                && _pitCode != PitCode.Unknown;

            enabled = enabled
                && (!_overrideStatus
                    || (_overrideStatus && _newPalletStatus != PalletStatus.Invalid && _newPalletStatus != PalletStatus.Hold)
                    || (_overrideStatus && _newPalletStatus == PalletStatus.Hold && _holdCode != Constant.NoHoldCode));

            _btnAdd.Enabled = enabled;
        }

        private void _TxtPalletID_TextChanged(object sender, EventArgs e)
        {
            _palletID = _txtPalletID.Text;
            _EnableAddButton();
        }

        private void _cmbPitCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbPitCode.SelectedIndex > -1)
            {
                _pitCode = _cmbPitCode.GetSelectedEnumItem<PitCode>();
            }
            _EnableAddButton();
        }

        private void _ChkOverridePalletStatus_CheckedChanged(object sender, EventArgs e)
        {
            _overrideStatus = _chkOverridePalletStatus.Checked;
            _cmbNewPalletStatus.Enabled = _overrideStatus;
            if (!_overrideStatus)
            {
                _cmbHoldCode.Enabled = false;
            }
            _EnableAddButton();
        }

        private void _CmbNewPalletStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbNewPalletStatus.SelectedIndex > -1)
            {
                _newPalletStatus = _cmbNewPalletStatus.GetSelectedEnumItem<PalletStatus>();
                _cmbHoldCode.Enabled = _newPalletStatus == PalletStatus.Hold;
            }
            else
            {
                _cmbHoldCode.Enabled = false;
            }
            _EnableAddButton();
        }

        private void _CmbHoldCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbHoldCode.SelectedItem != null)
            {
                _holdCode = (int)_cmbHoldCode.SelectedValue;
            }
            _EnableAddButton();
        }

        private void _BtnAdd_Click(object sender, EventArgs e)
        {
            if (_pitProxy.ContainsKey(_palletID))
            {
                _ = XMessageBox.Show(
                    $"The {_pitProxy.CollectionName.LeftOfLast("P")} PIT already contains a pallet with Pallet ID {_palletID}.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (!MesInterface.TryFetchPalletItem(
                OperationCode.Unknown,
                _palletID,
                out PalletItem palletItem,
                out string fault))
            {
                string message = $"An error occurred while fetching the data for Pallet {_palletID}:\n\n{fault}";
                _ = XMessageBox.Show(
                    message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
                return;
            }
            if (_overrideStatus)
            {
                palletItem.Status = _newPalletStatus;
                if (_newPalletStatus == PalletStatus.Hold)
                {
                    palletItem.HoldCode = _holdCode;
                }
            }
            _pitItem = PitItem.Create(palletItem, _level, _pitCode);
            if (!_pitProxy.Update(_palletID, _pitItem))
            {
                _ = XMessageBox.Show(
                    $"Failed to Add Pallet {_palletID} to the {_pitProxy.CollectionName.LeftOfLast("P")} PIT.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _ = XMessageBox.Show(
                this,
                $"Successfully add Pallet {_palletID} to {_pitProxy.CollectionName.LeftOfLast("P")} PIT!",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
        }

        private void _BtnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}
