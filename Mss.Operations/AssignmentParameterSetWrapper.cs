using DacQuest.DFX.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    [Serializable]
    public class AssignmentParameterSetWrapper : XConfigurationParameterSet
    {

        private int _number = 0;
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^[1-3]$",
            PickListValues = "1,2,3")]
        public int Number => _number;


    }
}
