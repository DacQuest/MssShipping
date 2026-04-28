using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mss.Views
{
    public partial class PitMultiDeleteForm : Form
    {
        public PitMultiDeleteForm()
        {
            InitializeComponent();
        }
        public PitMultiDeleteForm(string pitName)
        {
            InitializeComponent();
            _lblMessage.Text = $"Do you want to Delete all the selected {pitName} pallets?";
        }

        private void _BtnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void _BtnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
