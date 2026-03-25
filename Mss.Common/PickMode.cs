using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum PickMode
    {
        BySku      = 0,                            //!!! Dave wants 1
        ByPalletID = 1, // checks that SKU matches //!!! Dave wants 2
        ByJobID    = 2, // checks that SKU matches //!!! Dave wants 4

    }
}
