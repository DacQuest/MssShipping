using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    public class SpecialPicks : XSharedDictionary<string, SpecialPickItem>
    {
        public bool IsSpecialPickPallet(
    int palletID,
    int jobID)
        {
            Lock();
            try
            {
                return Values.Any(s =>
                {
                    return (palletID > Constant.NoPalletID
                            && s.FrontPickMode == PickMode.ByPalletID
                            && s.FrontPickModeKey == palletID)
                        || (jobID > Constant.NoJobID
                            && s.FrontPickMode == PickMode.ByJobID
                            && s.FrontPickModeKey == jobID)
                        || (palletID > Constant.NoPalletID
                            && s.RearPickMode == PickMode.ByPalletID
                            && s.RearPickModeKey == palletID)
                        || (jobID > Constant.NoJobID
                            && s.RearPickMode == PickMode.ByJobID
                            && s.RearPickModeKey == jobID);
                });
            }
            finally
            {
                Unlock();
            }
        }

        public void Finalize(VehicleRow vehicleRow, string vin)
        {
            string shortVin = vin.Right(Constant.ShortVinLength);
            if (TryGetItem(shortVin, out SpecialPickItem specialPickItem))
            {
                PickMode frontPickMode = specialPickItem.FrontPickMode;
                if (vehicleRow == VehicleRow.Row1
                    || (frontPickMode != PickMode.ByJobID
                        && frontPickMode != PickMode.ByPalletID))
                {
                    Remove(shortVin);
                }
            }
        }

    }
}
