using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum  PickMode
    {
        BySku = 0,
        ByPalletID = 1, // checks for matching SKU
        ByJobID = 2, // checks for matching SKU

    }
}
