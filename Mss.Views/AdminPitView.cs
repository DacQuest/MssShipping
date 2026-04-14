using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.DataItemEditors;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;

namespace Mss.Views
{
    public partial class AdminPitView : XSharedDictionaryProxyEditorView<string, PitItem>
    {
        public AdminPitView()
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
                : string.Format("Pallet ID {0}", key.ToString());
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
