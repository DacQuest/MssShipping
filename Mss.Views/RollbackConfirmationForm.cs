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
    public partial class RollbackConfirmationForm : Form
    {
        public RollbackConfirmationForm(string message)
        {
            InitializeComponent();

            lblMessage.Text = message;
        }
    }
}
