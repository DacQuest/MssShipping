using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DacQuest.DFX.Core.Strings;
// using DevExpress.XtraCharts;

namespace Mss.Views
{
    public partial class StringEntryForm : Form
    {
        public string Value
        {
            get;
            private set;
        }

        public StringEntryForm(string title, string initialValue)
        {
            InitializeComponent();

            Text = title;
            Value = initialValue;
            _txtString.Text = Value;
        }

        private void _TxtString_TextChanged(object sender, EventArgs e)
        {
            Value = _txtString.Text;
            _btnOK.Enabled = !Value.IsNullOrWhiteSpace();
        }
    }
}
