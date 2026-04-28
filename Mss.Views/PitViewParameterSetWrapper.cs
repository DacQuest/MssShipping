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
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^AssignmentPit$|^UpperPit$|^LowerPit$",
            PickListValues = "AssignmentPit,UpperPit,LowerPit")]
        public string CollectionName => _collectionName;

        private string _displayName = string.Empty;
        [XConfigurationProperty(
            @"",
            true)]
        public string DisplayName => _displayName;

        private bool _allowDeleting = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowDeleting => _allowDeleting;

        private bool _allowAdding = false;
        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowAdding => _allowAdding;

//         private bool _showDestination = false;
//         [XConfigurationProperty(
//             @"",
//             false,
//             DefaultValue = "false",
//             PickListValues = "true,false")]
//         public bool ShowDestination => _showDestination;

    }
}
