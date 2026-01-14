using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mss.Collections
{
    [Serializable]
    public partial class PitItem : XDataItem
        {
            //         public string PalletID => Pallet.PalletID;

            public static PitItem Create(
                PalletItem palletItem, PitCode pitcode)
                => new PitItem
                {
                    PalletID = palletItem.PalletID,
                    PitCode = pitcode,
                    Pallet = palletItem,
                    SetOn = DateTime.Now
                };
        private static readonly PitCode[] _assignedCodes = new[]
{
            PitCode.Assigned1,
            PitCode.Assigned2,
            PitCode.Assigned3,
            PitCode.Assigned4,
        };

        public CraneNumber AssignedCrane
        {
            get
            {
                switch (PitCode)
                {
                    case PitCode.Assigned1:
                        return CraneNumber.Crane1;
                    case PitCode.Assigned2:
                        return CraneNumber.Crane2;
                    case PitCode.Assigned3:
                        return CraneNumber.Crane3;
                    case PitCode.Assigned4:
                        return CraneNumber.Crane4;
                    case PitCode.Assigned5:
                        return CraneNumber.Crane5;
                    default:
                        return CraneNumber.None;
                }
            }
        }


    }
}
