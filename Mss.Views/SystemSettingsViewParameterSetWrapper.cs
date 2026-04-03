using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Views
{
    [Serializable]
    public class SystemSettingsViewParameterSetWrapper : XConfigurationParameterSet
    {
        private bool _allowEditing = false;
        private bool _confirmEdits = false;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowEditing => _allowEditing;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool ConfirmEdits => _confirmEdits;

    }
}
