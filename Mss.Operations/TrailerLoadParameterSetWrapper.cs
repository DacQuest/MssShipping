using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.Strings;
using Mss.Collections;
using Mss.Common;

namespace Mss.Operations
{
    [Serializable]
    public class TrailerLoadParameterSetWrapper : XConfigurationParameterSet
    {
        private SlugLetter _slugLetter = SlugLetter.None;
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^[AB]$",
            PickListValues = "A, B")]
        public SlugLetter SlugLetter => _slugLetter;

        private short _shipperReportCopies = 1;
        [XConfigurationProperty(
            @"",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
            DefaultValue = "1")]
        public short ShipperReportCopies => _shipperReportCopies;

        private string _shipperReportPrinterName = string.Empty;
        [XConfigurationProperty(
            @"",
            true,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt)]
        public string ShipperReportPrinterName => _shipperReportPrinterName;

        private string _secondaryShipperReportPrinterName = string.Empty;
        [XConfigurationProperty(
            @"",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt)]
        public string SecondaryShipperReportPrinterName =>
            _secondaryShipperReportPrinterName.IsNullOrWhiteSpace()
                ? ShipperReportPrinterName
                : _secondaryShipperReportPrinterName;

    }
}
