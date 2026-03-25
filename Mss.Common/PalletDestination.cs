using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum PalletDestination
    {
        None    = 0,  //!!! Dave didn't include this
        Storage = 1,  //!!! Dave wants 1
        Purge   = 2,  //!!! Dave wants 2
        Console = 3,  //!!! Dave wants 4
        Hold    = 4   //!!! Dave wants 8
    }
}
