using DacQuest.DFX.Core;
using Mss.Collections;
using Mss.Common;
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
    public partial class BulkSystemSettingsEditForm : Form
    {
        public BulkSystemSettingsEditForm(string message)
        {
            InitializeComponent();

            _lblMessage.Text = message;
        }
    }
}
