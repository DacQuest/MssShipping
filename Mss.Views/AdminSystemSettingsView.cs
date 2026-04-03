using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core.Threading;
using DacQuest.DFX.DataItemEditors;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;
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
    public partial class AdminSystemSettingsView : XSharedSingleItemProxyEditorView<SystemSettingsItem>
    {
        public AdminSystemSettingsView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {
            base.OpenView();
        }

        //protected override String GetArrayName()
        //{
        //    return Constant.StorageName;
        //}

        //protected override string GetIndexInfo(int index)
        //{
        //    return base.GetIndexInfo(index);
        //}

        //protected override void OpenView()
        //{
        //    base.OpenView();
        //}

        //protected override void ProcessViewParameters(XConfigurationParameterSet viewParameters)
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
