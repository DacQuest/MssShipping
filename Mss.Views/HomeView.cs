using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using System.Collections;
using DacQuest.DFX.Core.Strings;
using System.Data.SqlClient;

namespace Mss.Views
{
    public partial class HomeView : XSnapInView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            return true;
        }

        //*************************************************************************

        //public override void ViewClosed()
        //{
        //}

        //protected override MenuStrip GetViewMenuStrip()
        //{
        //    return null;
        //}

        //protected override Boolean ProcessEnterKey()
        //{
        //    return false;
        //}

        //protected override Boolean ProcessEscapeKey()
        //{
        //    return false;
        //}

    }
}
