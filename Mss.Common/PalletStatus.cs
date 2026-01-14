using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum PalletStatus
    {
        Invalid = 0x00,
        OK = 0x01,
        Hold = 0x02,
        QAPick = 0x04,
        Purge = 0x08,
        Unknown = 0x10
    }
}
