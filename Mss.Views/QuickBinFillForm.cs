using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.Strings;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Views
{
    public partial class QuickBinFillForm : Form
    {
        public string PalletID
        {
            get;
            private set;
        }

        public QuickBinFillForm(string caption)
        {
            InitializeComponent();
            Text = caption;
        }

        private void _TxtPalletID_TextChanged(object sender, EventArgs e)
        {
            PalletID = txtPalletID.Text;
            btnOK.Enabled = !PalletID.IsNullOrWhiteSpace();
        }

    }
}
