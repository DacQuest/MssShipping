using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using Mss.Common;

namespace Mss.Views
{
    public partial class AcceptLoadConfirmationDlg : Form
    {
        public AcceptLoadConfirmationDlg(SlugLetter slugLetter)
        {
            InitializeComponent();

            _lblMessage.Text = $"Are you certain that you want to ACCEPT the Load on Slug {slugLetter.ToText()}?";
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
