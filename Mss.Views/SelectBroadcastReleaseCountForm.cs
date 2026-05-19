using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Views
{
    public partial class SelectBroadcastReleaseCountForm : Form
    {
        public SelectBroadcastReleaseCountForm(
            SlugLetter slugLetter,
            List<int> releasableCounts,
            int releasableBroadcastCount)
        {
            InitializeComponent();

            _lblMessage.Text = $"Select the count of Broadcast Records to RELEASE to Slug {slugLetter.Letter()}.";

            _cmbCountToRelease.Items.AddRange(releasableCounts.Cast<object>().ToArray());
            _cmbCountToRelease.SelectedIndex = 0;
        }

        public int CountToRelease { get; private set; } = 0;

        private void _BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void _BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void _CmbCountToRelease_SelectedIndexChanged(object sender, EventArgs e)
        {
            CountToRelease = (int)_cmbCountToRelease.SelectedItem;
            _btnOK.Enabled = CountToRelease > 0;
        }
    }
}
