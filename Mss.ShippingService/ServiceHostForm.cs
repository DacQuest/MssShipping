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

        private void ServiceHostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XMessageBox.Show(
                this,
                string.Format("Are you sure you want to close {0}?", XApplication.FullInstanceName),
                "Confirm Close",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
