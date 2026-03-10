using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum LoadPickPriority
    {
        SmallerLoadID = 0,
        Balanced = 1,
        LoadA = 2,
        LoadB = 3,
        LoadAOnly = 4,
        LoadBOnly = 5
    }
}
