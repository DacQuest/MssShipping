using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mss.Common;


namespace Mss.Collections
{
    public abstract class Pit : XSharedDictionary<string, PitItem>
    {
        public abstract Levels Level { get; }

        public void Set(
            Levels level,
            PalletItem palletItem,
            PitCode pitCode)
        {
            _ = Lock();
            try
            {
                _ = Remove(palletItem.PalletID);
                PitItem pitItem = PitItem.Create(palletItem, level, pitCode);
                this[pitItem.PalletID] = pitItem;
            }
            finally
            {
                Unlock();
            }
        }

        public int[] AssignedCraneCounts
        {
            get
            {
                int[] assignedCounts = new int[Constant.MaxCranes + 1];
                _ = Lock();
                try
                {
                    for (CraneNumber craneNumber = CraneNumber.Crane1;
                            craneNumber <= CraneNumber.Crane4;
                            craneNumber++)
                    {
                        assignedCounts[craneNumber.Index()] = Values
                            .Count(p => p.PitCode == craneNumber.AssignedPitCode());
                    }
                    return assignedCounts;
                }
                finally
                {
                    Unlock();
                }
            }
        }
    }
}
