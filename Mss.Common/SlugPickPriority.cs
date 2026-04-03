using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum SlugPickPriority
    {
        SmallerLoadID = 0,
        Balanced = 1,
        SlugA = 2,
        SlugB = 3,
        SlugAOnly = 4,
        SlugBOnly = 5
    }
}
