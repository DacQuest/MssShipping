using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Views
{
    [Serializable]
    public class PitViewParameterSetWrapper : XConfigurationParameterSet
    {
        private string _collectionName = string.Empty;
        private string _displayName = string.Empty;
        private bool _allowEditing = false;
        private bool _showDestination = false;

        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^AssignmentPit$|^UpperPit$|^LowerPit$",
            PickListValues = "AssignmentPit,UpperPit,LowerPit")]
        public string CollectionName => _collectionName;

        [XConfigurationProperty(
            @"",
            true)]
        public string DisplayName => _displayName;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowEditing => _allowEditing;

//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool ShowDestination => _showDestination;
    }
}
