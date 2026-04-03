using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;
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

//         private Levels _level = Levels.None;
//         [XConfigurationProperty(
//             @"",
//             true,
//             ValidationRegexString = "^Upper$|^Lower$",
//             PickListValues = "Lower, Upper")]
//         public Levels Level => _level;

    }
}
