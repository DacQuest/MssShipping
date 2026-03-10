using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public enum PalletEvent
    {
        None = 0,
        Arrival,
        MoveCommandSent,
        DepartureCompleted,
        GetSent,
        GetCompleted,
        PutSent,
        PutCompleted,
        InvalidLocationError,
        LocationFullError,
        LocationEmptyError,
        RecoveringPalletOnStartUp
    }
}
