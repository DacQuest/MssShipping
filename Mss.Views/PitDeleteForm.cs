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
    public partial class PitDeleteForm : Form
    {
        public PitDeleteForm()
        {
            InitializeComponent();
        }
        public PitDeleteForm(string palletID, string pitName)
        {
            InitializeComponent();
            _lblMessage.Text = $"Do you want to Delete Pallet {palletID} from the {pitName} PIT?";
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
