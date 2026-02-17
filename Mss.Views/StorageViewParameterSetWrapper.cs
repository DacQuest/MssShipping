using DacQuest.DFX.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Views
{
    [Serializable]
    public class StorageViewParameterSetWrapper : XConfigurationParameterSet
    {
        private bool _allowBinEditing = false;
        private bool _allowBulkEditing = false;
        private bool _allowPalletStatusEditing = false;
        private bool _allowAuditPicks = false;
        private bool _goToFirstSearchResult = true;
        private int _maxBank1Horizontal = 41;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowBinEditing => _allowBinEditing;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowBulkEditing => _allowBulkEditing;

        //         [XConfigurationProperty(
        //             @"",
        //             false,
        //             DefaultValue = "true",
        //             PickListValues = "true,false")]
        //         public bool AllowMarkAudit => _allowMarkAudit;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowPalletStatusEditing => _allowPalletStatusEditing;

        //[XConfigurationProperty(
        //    @"",
        //    false,
        //    DefaultValue = "false",
        //    PickListValues = "true,false")]
        //public bool AllowSkuEditing => _allowSkuEditing;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowAuditPicks => _allowAuditPicks;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "true",
            PickListValues = "true,false")]
        public bool GoToFirstSearchResult => _goToFirstSearchResult;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "41")]
        public int MaxBank1Horizontal => _maxBank1Horizontal;
    }
}
