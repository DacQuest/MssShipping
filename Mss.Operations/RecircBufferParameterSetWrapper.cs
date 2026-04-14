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
    public class RecircBufferParameterSetWrapper : XConfigurationParameterSet
    {
        private Levels _level = Levels.None;

        [XConfigurationProperty(
            @"",
            true,
            ValidationRegexString = "^Lower$|^Upper$",
            PickListValues = "Lower, Upper")]
        public Levels Level
        {
            get
            {
                return _level;
            }
        }
    }
}
