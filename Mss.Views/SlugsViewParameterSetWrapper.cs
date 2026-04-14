using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Views
{
    [Serializable]
    public class SlugsViewParameterSetWrapper : XConfigurationParameterSet
    {

        private bool _allowReprintLabel = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowReprintLabel => _allowReprintLabel;

        private bool _allowEditItem = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowEditItem => _allowEditItem;

        private bool _allowRollback = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowRollback => _allowRollback;

        private bool _allowLimitLoad = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowLimitLoad => _allowLimitLoad;

        private bool _allowShortLoad = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowShortLoad => _allowShortLoad;

        private bool _allowAcceptLoad = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowAcceptLoad => _allowAcceptLoad;

        private bool _allowAbortLoad = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowAbortLoad => _allowAbortLoad;

        private bool _allowReleaseBroadcast = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowReleaseBroadcast => _allowReleaseBroadcast;

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
