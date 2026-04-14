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
        AS1     =  1,  // Assignment 1 (Destinations: Lower or Upper/20%)
        AS2     =  2,  // Assignment 2 (Destinations: Upper or 20%)
        AS3     =  3,  // Assignment 3 (Destinations: Upper or Lower for 20%)
        UR1     =  4,  // Upper Router 1 (Incorporates Upper Sizing)
        LR1     =  5,  // Lower Router 1 (Incorporates Lower Sizing)
        UR2     =  6,  // Upper Router 2
        LR2     =  7,  // Lower Router 2
        UR3     =  8,  // Upper Router 3
        LR3     =  9,  // Lower Router 3
        UR4     = 10,  // Upper Router 4
        LR4     = 11,  // Lower Router 4
        CR1     = 12,  // Crane 1
        CR2     = 13,  // Crane 2
        CR3     = 14,  // Crane 3
        CR4     = 15,  // Crane 4
        ULD     = 16,  // Upper Load Director
        LLD     = 17,  // Lower Load Director
        URR     = 18,  // Upper Recirc Router
        LRR     = 19,  // Lower Recirc Router
        URB     = 20,  // Upper Recirc Buffer
        LRB     = 21,  // Lower Recirc Buffer
        UP      = 22,  // Upper Purge
        LP      = 23,  // Lower Purge
        UT      = 24,  // Upper Transfer
        LT      = 25,  // Lower Transfer
        TLA     = 26,  // Trailer Load A
        TLB     = 27   // Trailer Load B
    }
}
