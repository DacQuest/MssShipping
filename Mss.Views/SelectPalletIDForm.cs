using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core.MessageBox;

namespace Mss.Views
{
    public partial class SelectPalletIDForm : Form
    {
        private readonly List<string> _palletIDs;
        public string PalletID
        {
            get;
            private set;
        }

        public SelectPalletIDForm(List<string> palletIDs)
        {
            InitializeComponent();

            _palletIDs = palletIDs;
        }

        private void _SelectPalletIDForm_Load(object sender, EventArgs e)
        {
            _cmbPalletID.Items.AddRange(_palletIDs.OrderBy(s => s).ToArray());
            _cmbPalletID.SelectedIndex = -1;
            _cmbPalletID.Text = string.Empty;
            PalletID = string.Empty;
        }

        private void _CmbPalletID_SelectedIndexChanged(object sender, EventArgs e)
        {
            PalletID = _cmbPalletID.SelectedIndex > -1
                ? (string)_cmbPalletID.SelectedItem
                : string.Empty;
        }

        private void _CmbPalletID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                PalletID = _cmbPalletID.Text;
            }
        }

        private void _CmbPalletID_Leave(object sender, EventArgs e)
        {
            PalletID = _cmbPalletID.Text;
        }

        private bool _ValidatePalletID()
        {
            PalletID = _cmbPalletID.Text;
            if (string.IsNullOrWhiteSpace(PalletID))
            {
                _ = XMessageBox.Show(
                    this,
                    "You must select a Pallet ID.",
                    "No Pallet ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _cmbPalletID.Focus();
                _cmbPalletID.SelectAll();
                return false;
            }
            if (!_palletIDs.Contains(PalletID))
            {
                _ = XMessageBox.Show(
                    this,
                    $"'{PalletID}' is not a known Pallet ID.",
                    "Unknown Pallet ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _ = _cmbPalletID.Focus();
                _cmbPalletID.SelectAll();
                return false;
            }
            return true;
        }

        private void _BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = _ValidatePalletID() ? DialogResult.OK : DialogResult.None;
        }

    }
}
