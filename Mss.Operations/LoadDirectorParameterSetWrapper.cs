using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;
using Mss.Common;

namespace Mss.Operations
{
    [Serializable]
    public class LoadDirectorParameterSetWrapper : XConfigurationParameterSet
    {
        private Levels _level = Levels.None;
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^Lower$|^Upper$",
            PickListValues = "Lower, Upper")]
        public Levels Level => _level;

        private bool _autoReleaseNonLoadPallets = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AutoReleaseNonLoadPallets => _autoReleaseNonLoadPallets;

        private bool _manualLabelPrinterAvailable = true;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "true",
            PickListValues = "true,false")]
        public bool ManualLabelPrinterAvailable => _manualLabelPrinterAvailable;

        private bool _printLeftLabelFirst = true;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "true",
            PickListValues = "true,false")]
        public bool PrintLeftLabelFirst => _printLeftLabelFirst;

    }
}
