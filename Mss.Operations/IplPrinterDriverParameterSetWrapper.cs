using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Operations
{
    class IplPrinterDriverParameterSetWrapper : XConfigurationParameterSet
    {
        private string _formatPlaceholderStartDelimiter = "[[";
        private string _formatPlaceholderEndDelimiter = "]]";
        private bool _singleDefinitionFile = false;
        private string _labelDefinitionPath = @"..\Labels";
        private XConfigurationValueList _labelDefinitionNames = null;

        [XConfigurationProperty(
           @"",
           false,
           ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
           DefaultValue = "[[")]
        public string FormatPlaceholderStartDelimiter => _formatPlaceholderStartDelimiter;

        [XConfigurationProperty(
           @"",
           false,
           ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
           DefaultValue = "]]")]
        public string FormatPlaceholderEndDelimiter => _formatPlaceholderEndDelimiter;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "true",
            PickListValues = "true,false")]
        public bool SingleDefinitionFile => _singleDefinitionFile;

        [XConfigurationProperty(
            @"",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt)]
        public string LabelDefinitionPath => _labelDefinitionPath;

        [XConfigurationProperty(
            @"",
            true,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt)]
        public XConfigurationValueList LabelDefinitionNames => _labelDefinitionNames;
    }
}
