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
        Invalid      = 0x0000,
        Waiting      = 0x0001,  // waiting for Broadcast
        Pending      = 0x0002,  // waiting to be Pickable, the Broadcast is not active
        Pickable     = 0x0004,  // Broadcast active, no pallet, waiting for one
        Picking      = 0x0008,  // pallet being picked
        Picked       = 0x0010,  // pallet dropped off by crane but has not passed Load Director
        Presequenced = 0x0020,  // pallet passed Load Director, possibly next in lane, can be resequenced
        Sequenced    = 0x0040,  // pallet passed Load Director, next in lane, sequence guaranteed
        Done         = 0x0080,  // pallet passed Transfer (move complete)
        Loadable     = 0x0100   // entire load Done and data has been archived, waiting to be loaded into trailer

    }
}
