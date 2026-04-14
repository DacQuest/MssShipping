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
using DacQuest.DFX.DataItemEditors;
using DacQuest.DFX.Core.DataItems;
using Mss.Collections;

namespace Mss.Views
{
    public partial class AdminBroadcastView : XSharedDictionaryProxyEditorView<string, BroadcastItem>
    {
        public AdminBroadcastView()
        {
            InitializeComponent();
        }

        //protected override String GetDictionaryName()
        //{
        //    return Constant.;
        //}

        //protected override string GetKeyValidatorName()
        //{
        //    return Constant.;
        //}

        protected override string GetKeyInfo(object key)
        {
            return key == null
                ? string.Empty
                : $"CSN {key})";
        }

        //protected override void OpenView()
        //{
        //    base.OpenView();
        //}

        //protected override void ProcessParameters(XConfigurationParameterSet parameters)
        //{
        //}

        //protected override void AutoSubscribe()
        //{
        //}

        //public override bool ViewClosing(bool force)
        //{
        //    return true;
        //}

        //public override void ViewClosed()
        //{
        //}

        //protected override MenuStrip GetViewMenuStrip()
        //{
        //    return null;
        //}

        //protected override bool ProcessEnterKey()
        //{
        //    return false;
        //}

        //protected override bool ProcessEscapeKey()
        //{
        //    return false;
        //}

    }
}
