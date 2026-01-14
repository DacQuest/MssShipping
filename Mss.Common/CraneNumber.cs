using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum CraneNumber
    {
        None = 0,
        Crane1 = 1,
        Crane2 = 2,
        Crane3 = 3,
        Crane4 = 4,
    }
    public static int Index(this CraneNumber craneNumber)
    => (int)craneNumber;

    public static PitCode AssignedPitCode(this CraneNumber craneNumber)
    {
        switch (craneNumber)
        {
            case CraneNumber.Crane1:
                return PitCode.Assigned1;
            case CraneNumber.Crane2:
                return PitCode.Assigned2;
            case CraneNumber.Crane3:
                return PitCode.Assigned3;
            case CraneNumber.Crane4:
                return PitCode.Assigned4;
            default:
                return PitCode.None;
        }
    }

}
