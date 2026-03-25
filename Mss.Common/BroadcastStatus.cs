using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Flags]
    public enum BroadcastStatus
    {
        Invalid = 0x00,
        OK      = 0x01,
        Shipped = 0x02,
        Skip    = 0x04,
        Missing = 0x08,
    }
}
