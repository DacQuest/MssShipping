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
    public partial class DateRangeForm : Form
    {
        public DateTime Start
        {
            get;
            private set;
        }

        public DateTime End
        {
            get;
            private set;
        }

        //        public DateRangeForm(DateTime start, DateTime end)
        public DateRangeForm()
        {
            InitializeComponent();

            End = DateTime.Now;
            Start = End - new TimeSpan(1, 0, 0, 0);
            //            if (start != null)
            //            {
            //                Start = start;
            //            }
            //            if (end != null)
            //            {
            //                End = end;
            //            }

            dtStart.Value = Start;
            dtEnd.Value = End;
        }

        private void dtStart_ValueChanged(object sender, EventArgs e)
        {
            Start = (DateTime)dtStart.Value;
            _EnableOKButton();
        }

        private void dtEnd_ValueChanged(object sender, EventArgs e)
        {
            End = (DateTime)dtEnd.Value;
            _EnableOKButton();
        }

        private void _EnableOKButton()
        {
            btnOK.Enabled = Start < End;
        }
    }
}
