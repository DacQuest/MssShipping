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
    public partial class ReprintLabelConfirmationForm : Form
    {
        public ReprintLabelConfirmationForm(
            bool shippingLabel,
            string message)
        {
            InitializeComponent();

            Text = shippingLabel
                ? "Reprint Shipping Label"
                : "Reprint Load Label";
            lblMessage.Text = message;
        }
    }
}
