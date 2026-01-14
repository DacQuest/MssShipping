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
        Invalid = 0x0000,
        Empty = 0x0001,
        Pickable = 0x0002,
        GetAllocated = 0x0004,
        PutAllocated = 0x0008,
        Offline = 0x0010,
        OfflineNoRead = 0x0020,
        OfflineDuplicatePalletID = 0x0040

    }
}
