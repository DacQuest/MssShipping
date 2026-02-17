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
    public partial class ConvertAllStatusesForm : Form
    {
        private bool _applyToAll = false;
        private bool _useBulkMode => true;
        private bool _stop = false;

        public ConvertAllStatusesForm()
        {
            InitializeComponent();
        }

        public void SetMessage(string message)
        {
            lblMessage.Text = message;
        }

        public bool ApplyToAll => _applyToAll;
        public bool UseBulkMode => _useBulkMode;
        public bool Stop => _stop;

        private void chkApplyToAll_CheckedChanged(object sender, EventArgs e)
        {
            _applyToAll = chkApplyToAll.Checked;
            chkUseBulkMode.Enabled = _applyToAll;
            if (!_applyToAll)
            {
                chkUseBulkMode.Checked = false;
            }
        }

        public void SetProgressMode(
            int count,
            string message)
        {
            chkApplyToAll.Visible = false;
            chkUseBulkMode.Visible = false;
            btnYes.Visible = false;
            btnNo.Visible = false;
            btnCancel.Visible = false;
            btnStop.Visible = true;
            btnStop.Enabled = true;

            progressBar.Maximum = count;
            progressBar.Minimum = 1;

            Text = string.Format("Converting {0} of {1}...", progressBar.Value, progressBar.Maximum);
            lblMessage.Text = message;

            progressBar.Visible = true;
            progressBar.Value = 2;
            progressBar.Refresh();
            Application.DoEvents();
        }

        public void PulseProgress(string message)
        {
            lblMessage.Text = message;
            Text = string.Format("Converting {0} of {1}...", progressBar.Value, progressBar.Maximum);
            if (progressBar.Value < progressBar.Maximum)
            {
                progressBar.Value = progressBar.Value + 1;
            }
            progressBar.Refresh();
            Application.DoEvents();
        }

        private void chkUseBulkMode_CheckedChanged(object sender, EventArgs e)
        {
            //_useBulkMode = chkUseBulkMode.Checked;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _stop = true;
        }
    }
}
