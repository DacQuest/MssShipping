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
using Mss.Common;
using Mss.Collections;

namespace Mss.Views
{
    public partial class BroadcastAdminEditorView : XSharedDictionaryProxyEditorView<String, BroadcastItem>
    {
        public BroadcastAdminEditorView()
        {
            InitializeComponent();
        }

        protected override String GetDictionaryName()
        {
            return Constant.BroadcastName;
        }

//         protected override String GetKeyValidatorName()
//         {
//             return Constant.CsnValidatorName;
//         }

        protected override String GetKeyInfo(Object key)
        {
            if (key == null)
            {
                return String.Empty;
            }
            else
            {
                return string.Format("Broadcast Sequence Number {0}", key.ToString());
            }
        }

//        protected override void OpenView()
//        {
//            base.OpenView();
//        }

//        protected override void ProcessViewParameters(XConfigurationParameterSet viewParameters)
//        {
//        }

        //protected override void AutoSubscribe()
        //{
        //}

        //public override Boolean ViewClosing(bool force)
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
