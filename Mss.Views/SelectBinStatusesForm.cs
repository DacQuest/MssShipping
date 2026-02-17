using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mss.Collections;
using Mss.Common;

namespace Mss.Views
{
    public partial class SelectBinStatusesForm : Form
    {
        public BinStatus BinStatuses
        {
            get;
            set;
        }
        public SelectBinStatusesForm()
        {
            InitializeComponent();
            BinStatuses = BinStatus.Invalid;
        }

        private void _EnableOKButton()
        {
            btnOK.Enabled = BinStatuses > BinStatus.Invalid;
        }

        private void _ChkEmpty_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEmpty.Checked)
            {
                BinStatuses |= BinStatus.Empty;
            }
            else
            {
                BinStatuses &= ~BinStatus.Empty;
            }
            _EnableOKButton();
        }

        private void _ChkPickable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPickable.Checked)
            {
                BinStatuses |= BinStatus.Pickable;
            }
            else
            {
                BinStatuses &= ~BinStatus.Pickable;
            }
            _EnableOKButton();
        }

        private void _ChkPutAllocated_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPutAllocated.Checked)
            {
                BinStatuses |= BinStatus.PutAllocated;
            }
            else
            {
                BinStatuses &= ~BinStatus.PutAllocated;
            }
            _EnableOKButton();
        }

        private void _ChkGetAllocated_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGetAllocated.Checked)
            {
                BinStatuses |= BinStatus.GetAllocated;
            }
            else
            {
                BinStatuses &= ~BinStatus.GetAllocated;
            }
            _EnableOKButton();
        }

        private void _ChkOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOffline.Checked)
            {
                BinStatuses |= BinStatus.Offline;
            }
            else
            {
                BinStatuses &= ~BinStatus.Offline;
            }
            _EnableOKButton();
        }

        private void _ChkOfflineDuplicate_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkOfflineDuplicate.Checked)
            {
                BinStatuses |= BinStatus.OfflineDuplicatePalletID;
            }
            else
            {
                BinStatuses &= ~BinStatus.OfflineDuplicatePalletID;
            }
            _EnableOKButton();
        }
    }
}
