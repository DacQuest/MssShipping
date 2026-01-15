using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum PitCode
    {
        None = 0,  // No valid Pit Code
        Assigned1 = 1,
        Assigned2 = 2,
        Assigned3 = 3,
        Assigned4 = 4
 
    }

    public static class PitCodeExtensions
    {
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
