using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Flags]
    public enum BinStatus
    {
        Invalid                  = 0x00,
        Empty                    = 0x01,
        Pickable                 = 0x02,
        GetAllocated             = 0x04,
        PutAllocated             = 0x08,
        Offline                  = 0x10,
        OfflineDuplicatePalletID = 0x20,
        OfflineNoRead            = 0x40

    }
}
