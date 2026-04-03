using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum OperationCode
    {
        Unknown =  0,
        AS      =  1,  // Assignment
        AS2     =  2,  // 20% Assignment
        UR1     =  3,  // Upper Router 1
        LR1     =  4,  // Lower Router 1
        UR2     =  5,  // Upper Router 2
        LR2     =  6,  // Lower Router 2
        UR3     =  7,  // Upper Router 3
        LR3     =  8,  // Lower Router 3
        UR4     =  9,  // Upper Router 4
        LR4     = 10,  // Lower Router 4
        CR1     = 11,  // Crane 1
        CR2     = 12,  // Crane 2
        CR3     = 13,  // Crane 3
        CR4     = 14,  // Crane 4
        ULD     = 15,  // Upper Load Director
        LLD     = 16,  // Lower Load Director
        URR     = 17,  // Upper Recirc Router
        LRR     = 18,  // Lower Recirc Router
        URL     = 19,  // Upper Recirc Loop
        LRL     = 20,  // Lower Recirc Loop
        UP      = 21,  // Upper Purge
        LP      = 22,  // Lower Purge
        UT      = 23,  // Upper Transfer
        LT      = 24,  // Lower Transfer
        TLA     = 25,  // Trailer Load A
        TLB     = 26   // Trailer Load B
    }
}
