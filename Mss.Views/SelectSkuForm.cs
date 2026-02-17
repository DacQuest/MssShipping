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
    public partial class SelectSkuForm : Form
    {
        private readonly List<string> _skus;
        public string Sku
        {
            get;
            private set;
        }

        public SelectSkuForm(List<string> skus)
        {
            InitializeComponent();

            _skus = skus;
        }

        private void _SelectSkuForm_Load(object sender, EventArgs e)
        {
            _cmbSku.Items.AddRange(_skus.OrderBy(s => s).ToArray());
            _cmbSku.SelectedIndex = -1;
            _cmbSku.Text = string.Empty;
            Sku = string.Empty;
        }

        private void _CmbSku_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sku = _cmbSku.SelectedIndex > -1
                ? (string)_cmbSku.SelectedItem
                : string.Empty;
        }

        private void _CmbSku_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                Sku = _cmbSku.Text;
            }
        }

        private void _CmbSku_Leave(object sender, EventArgs e)
        {
            Sku = _cmbSku.Text;
        }

        private bool _ValidateSku()
        {
            Sku = _cmbSku.Text;
            if (string.IsNullOrWhiteSpace(Sku))
            {
                _ = XMessageBox.Show(
                    this,
                    "You must select a SKU.",
                    "No SKU",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _cmbSku.Focus();
                _cmbSku.SelectAll();
                return false;
            }
            if (!_skus.Contains(Sku))
            {
                _ = XMessageBox.Show(
                    this,
                    $"'{Sku}' is not a valid SKU.",
                    "Invalid SKU",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _ = _cmbSku.Focus();
                _cmbSku.SelectAll();
                return false;
            }
            return true;
        }

        private void _BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = _ValidateSku() ? DialogResult.OK : DialogResult.None;
        }

    }
}
