using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mss.Common;

namespace Mss.Views
{
    public partial class AbortLoadConfirmationDlg : Form
    {
        public bool RecoverBroadcast { get; set; } = true;

        public AbortLoadConfirmationDlg(string slugName)
        {
            InitializeComponent();
            _chkAutoRecoverBroadcast.Checked = true;
            _lblMessage.Text = $"Are you absolutely certain that you want to ABORT the load on {slugName}?";
        }

        private void _BtnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void _BtnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void _ChkAutoRecoverBroadcast_CheckedChanged(object sender, EventArgs e)
        {
            RecoverBroadcast = _chkAutoRecoverBroadcast.Checked;
        }
    }
}
