using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Operations;
using Mss.Common;

namespace Mss.Views
{
    [Serializable]
    public class LoadDirectorViewParameterSetWrapper : XOperationUIViewParameterSetWrapper
    {

        private Levels _level = Levels.None;
        [XConfigurationProperty(
            @"",
            true,
            PickListValues = "Upper,Lower")]
        public Levels Level => _level;

        private bool _allowReprintLabel = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowReprintLabel => _allowReprintLabel;

//         private bool _allowItemEdit = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool AllowItemEdit => _allowItemEdit;
// 
//         private bool _allowRollback = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool AllowRollback => _allowRollback;
// 
//         private bool _allowAcceptLoad = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool AllowAcceptLoad => _allowAcceptLoad;
// 
//         private bool _allowAbortLoad = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool AllowAbortLoad => _allowAbortLoad;
// 
//         private bool _allowBroadcastRelease = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool AllowBroadcastRelease => _allowBroadcastRelease;

        private bool _showShortages = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool ShowShortages => _showShortages;

        private int _flashGridCellTimeoutSeconds = 0;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "0",
            ValidDirectives = XConfigurationDirectives.Alias)]
        public int FlashGridCellTimeoutSeconds => _flashGridCellTimeoutSeconds;

    }
}
