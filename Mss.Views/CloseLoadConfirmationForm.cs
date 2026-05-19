using Mss.Common;
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
    public partial class CloseLoadConfirmationForm : Form
    {
        public CloseLoadConfirmationForm(SlugLetter slugLetter, bool reopenLoad)
        {
            InitializeComponent();

            string action;
            if (reopenLoad)
            {
                Text = "Confirm Reopen Load";
                action = "REOPEN";
                _pbAction.Image = Properties.Resources.GreenPlus48;
            }
            else
            {
                Text = "Confirm Close Load";
                action = "CLOSE";
                _pbAction.Image = Properties.Resources.RedMinus48;
            }
            _lblMessage.Text = $"Are you certain that you want to {action} the Load on {slugLetter.SlugDisplayName()}?";
        }

        private void _BtnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void _BtnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
