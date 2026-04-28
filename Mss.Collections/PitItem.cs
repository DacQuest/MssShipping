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

        private static object _lockObject = new object();
        private static Dictionary<int, string> _holdCodes = null;

        public static void SetHoldCodes(Dictionary<int, string> holdCodes)
        {
            lock (_lockObject)
            {
                _holdCodes = holdCodes;
            }
        }

        public static string GetHoldCodeDescription(int holdCode)
        {
            if (holdCode == Constant.NoHoldCode)
            {
                return string.Empty;
            }
            lock (_lockObject)
            {
                return _holdCodes == null
                    || !_holdCodes.TryGetValue(holdCode, out string description)
                    ? "Unknown Hold Code"
                    : description;
            }
        }

        public string HoldCodeDescription => Pallet == null || Pallet.Status != PalletStatus.Hold
            ? string.Empty
            : GetHoldCodeDescription(Pallet.HoldCode);

        public string SetOnText => SetOn > Constant.BeginningOfTime
            ? SetOn.ToString("G")
            : string.Empty;

        public string PalletVehicleRowText => Pallet == null
            ? string.Empty
            : Pallet.VehicleRowText;

        public string PalletSku => Pallet == null
            ? string.Empty
            : Pallet.Sku;

        public string PalletJobID => Pallet == null
            ? string.Empty
            : Pallet.JobID;

        public string PalletStatusText => Pallet == null
            ? string.Empty
            : Pallet.StatusText;

        public static PitItem Create(
            PalletItem palletItem,
            Levels level,
            PitCode pitcode)
                => new PitItem
                {
                    PalletID = palletItem.PalletID,
                    Level = level,
                    PitCode = pitcode,
                    Pallet = palletItem,
                    SetOn = DateTime.Now
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
                    default:
                        return CraneNumber.None;
                }
            }
        }


    }
}
