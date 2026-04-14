using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Views
{
    [Serializable]
    public class BroadcastViewParameterSetWrapper : XConfigurationParameterSet
    {

        private bool _allowEdit = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowEdit => _allowEdit;

        private bool _allowRelease = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowRelease => _allowRelease;

        private bool _allowRecover = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowRecover => _allowRecover;

//         private bool _closeAfterRelease = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool CloseAfterRelease
//         {
//             get
//             {
//                 return _closeAfterRelease;
//             }
//         }

    }
}
