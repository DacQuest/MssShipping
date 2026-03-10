using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Threading;
using DacQuest.DFX.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mss.ShippingService
{
    public partial class ServiceHostForm : XMainForm
    {
        public ServiceHostForm()
        {
            InitializeComponent();
        }

        private void _ServiceHostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XMessageBox.Show(
                this,
                "Are you sure you want to shut down the Mississauga Shipping Service?",
                "Confirm Shut Down",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void _BtnShutDown_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
