using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum PitCode
    {
        Unknown   = 0,  // Unknown Pit Code /Unknown Pallet
        Assigned1 = 1,
        Assigned2 = 2,
        Assigned3 = 3,
        Assigned4 = 4,
        Purge     = 5,
        Stack     = 6
    }

    public static class PitCodeExtensions
    {
        public static bool IsAssigned(this PitCode pitCode)
            => pitCode == PitCode.Assigned1
                || pitCode == PitCode.Assigned2
                || pitCode == PitCode.Assigned3
                || pitCode == PitCode.Assigned4;

        public static bool IsAssigned(this PitCode pitCode, CraneNumber craneNumber)
            => pitCode.AssignedCrane() == craneNumber;

        public static CraneNumber AssignedCrane(this PitCode pitCode)
        {
            switch (pitCode)
            {
                case PitCode.Assigned1:
                    return CraneNumber.Crane1;
                case PitCode.Assigned2:
                    return CraneNumber.Crane2;
                case PitCode.Assigned3:
                    return CraneNumber.Crane3;
                case PitCode.Assigned4:
                    return CraneNumber.Crane4;
                default:
                    return CraneNumber.None;
            }
        }

    }

}
