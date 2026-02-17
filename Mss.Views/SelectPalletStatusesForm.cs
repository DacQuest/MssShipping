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
    public partial class SelectPalletStatusesForm : Form
    {
        public PalletStatus PalletStatuses
        {
            get;
            set;
        }
        public SelectPalletStatusesForm()
        {
            InitializeComponent();
            PalletStatuses = PalletStatus.Invalid;
        }

        private void _ChkOK_CheckedChanged(object sender, EventArgs e)
        {
            _ApplyStatus(_chkOK.Checked, PalletStatus.OK);
            _EnableOKButton();
        }

        private void _ChkHold_CheckedChanged(object sender, EventArgs e)
        {
            _ApplyStatus(_chkHold.Checked, PalletStatus.Hold);
            _EnableOKButton();
        }

        //private void _ChkQCSort_CheckedChanged(object sender, EventArgs e)
        //{
        //    _ApplyStatus(_chkQAPick.Checked, PalletStatus.QAPick);
        //    _EnableOKButton();
        //}

        private void _ChkUnknown_CheckedChanged(object sender, EventArgs e)
        {
            _ApplyStatus(_chkUnknown.Checked, PalletStatus.Unknown);
            _EnableOKButton();
        }

//         private void _ChkLearPick_CheckedChanged(object sender, EventArgs e)
//         {
//             _ApplyStatus(_chkLearPick.Checked, PalletStatus.LearPick);
//             _EnableOKButton();
//         }
// 
//         private void _ChkReviewPick_CheckedChanged(object sender, EventArgs e)
//         {
//             _ApplyStatus(_chkReviewPick.Checked, PalletStatus.ReviewPick);
//             _EnableOKButton();
//         }

        //private void chkAudit_CheckedChanged(object sender, EventArgs e)
        //{
        //    _ApplyStatus(chkAudit.Checked, PalletStatus.Audit);
        //    _EnableOKButton();
        //}

        //private void chkQuarantine_CheckedChanged(object sender, EventArgs e)
        //{
        //    _ApplyStatus(chkQuarantine.Checked, PalletStatus.Quarantine);
        //    _EnableOKButton();
        //}

        //private void chkEngChange_CheckedChanged(Object sender, EventArgs e)
        //{
        //    _ApplyStatus(chkEngChange.Checked, PalletStatus.EngChange);
        //    _EnableOKButton();
        //}

        //private void chkPriority_CheckedChanged(Object sender, EventArgs e)
        //{
        //    _ApplyStatus(chkPriority.Checked, PalletStatus.Priority);
        //    _EnableOKButton();
        //}

        private void _ApplyStatus(bool addStatus, PalletStatus palletStatus)
        {
            if (addStatus)
            {
                PalletStatuses |= palletStatus;
            }
            else
            {
                PalletStatuses &= ~palletStatus;
            }
        }

        private void _EnableOKButton()
        {
            btnOK.Enabled = PalletStatuses > PalletStatus.Invalid;
        }

    }
}
