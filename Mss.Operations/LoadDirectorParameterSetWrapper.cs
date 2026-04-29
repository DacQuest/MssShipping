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

        private bool _autoReleaseNonLoadPalletsInManualMode = true;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "true",
            PickListValues = "true,false")]
        public bool AutoReleaseNonLoadPalletsInManualMode => _autoReleaseNonLoadPalletsInManualMode;

        private bool _manualLabelPrinterAvailable = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool ManualLabelPrinterAvailable => _manualLabelPrinterAvailable;

        //         private bool _manualLabelPrinterAvailable = false;
        //         [XConfigurationProperty(
        //             @"",
        //             false,
        //             DefaultValue = "false",
        //             PickListValues = "true,false")]
        //         public bool ManualLabelPrinterAvailable => _manualLabelPrinterAvailable;


    }
}
