using DacQuest.DFX.Core.Configuration;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    [Serializable]
    public class CraneParameterSetWrapper : XConfigurationParameterSet
    {
        public CraneNumber CraneNumber => (CraneNumber)Number;

        private CraneFunction[] _pickFunctionPriority;
        public CraneFunction[] PickFunctionPriority => _pickFunctionPriority;

        private int _number = 0;
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^[1-4]$",
            PickListValues = "1,2,3,4")]
        public int Number => _number;

        private XConfigurationValueList _cranePickFunctionPriority = null;
        [XConfigurationProperty(
            @"",
            true,
            ValidDirectives = XConfigurationDirectives.Alias)]
        public XConfigurationValueList CranePickFunctionPriority => _cranePickFunctionPriority;

        public override bool ValidateParameterSet(
            string sectionName,
            string itemName,
            string parameterSetName,
            ref List<string> errors)
        {
            try
            {
                _pickFunctionPriority = _cranePickFunctionPriority
                    .Select(fs => Enum.Parse(typeof(CraneFunction), fs))
                    .Cast<CraneFunction>()
                    .ToArray();
                return true;
            }
            catch
            {
                errors.Add($"[{sectionName}/{itemName}/Parameters/{nameof(CranePickFunctionPriority)}] One or more values in the list are invalid.");
                return false;
            }
        }

    }
}
