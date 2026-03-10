using DacQuest.DFX.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mss.Common;

namespace Mss.Operations
{
    [Serializable]
    public class RouterParameterSetWrapper : XConfigurationParameterSet
    {
        public CraneNumber CraneNumber => (CraneNumber)_number;

        private int _number = 0;
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^[1-4]$",
            PickListValues = "1,2,3,4")]
        public int Number => _number;

        private Levels _level = Levels.None;
        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^Lower$|^Upper$",
            PickListValues = "Lower$, Upper$")]
        public Levels Level => _level;

        //private int _crane1InboundBufferSize = 0;
        //[XConfigurationProperty(
        //    @"",
        //    true,
        //    ValidDirectives = XConfigurationDirectives.Alias,
        //    DefaultValue = "0",
        //    ValidationRegexString = "^0$|^1$|^2$",
        //    PickListValues = "0,1,2")]
        //public int Crane1InboundBufferSize => _crane1InboundBufferSize;

        //private int _crane2InboundBufferSize = 0;
        //[XConfigurationProperty(
        //    @"",
        //    true,
        //    ValidDirectives = XConfigurationDirectives.Alias,
        //    ValidationRegexString = "^0$|^1$|^2$",
        //    PickListValues = "0,1,2")]
        //public int Crane2InboundBufferSize => _crane2InboundBufferSize;

    }
}
