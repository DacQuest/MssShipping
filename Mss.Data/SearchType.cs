using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data
{
    public enum SearchType
    {
        None = 0,
        ByCrane = 1,
        BySku = 2,
        BySkus = 3,
        ByPalletID = 4,
        Disabled = 5,
        Audits = 6,
        PickOnly = 7,
        DuplicatePallets = 8,
        ByPalletStatus = 9,
        ByBinStatus = 10,
        ByVehicleRow = 11,
        ByBuiltOn = 12,
        AdvancedSearch = 13
    }
}
