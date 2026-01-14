using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Flags]
    public enum LoadItemStatus
    {
        Invalid = 0x0000,
        Waiting = 0x0001,  // waiting for Ship Order
        Pending = 0x0002,  // waiting to be Pickable, the Ship Order is not active
        Pickable = 0x0004,  // Ship Order active, no pallet, waiting for one
        Picking = 0x0008,  // pallet being picked
        Picked = 0x0010,  // pallet picked, on crane
        Dropped = 0x0020,  // pallet dropped, on way to Load Director
        Sequenced = 0x0040,  // pallet passed Load Director, next in lane, sequence guaranteed
        Done = 0x0080  // pallet passed Transfer (move complete)

    }
}
