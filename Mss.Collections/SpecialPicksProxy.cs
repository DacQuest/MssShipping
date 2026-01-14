using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.Strings;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    public class SpecialPicksProxy : XSharedDictionaryProxy<string, SpecialPickItem>
    {
        public string GetSpecialPickCode(string vin, VehicleRow vehicleRow)
        {
            string code = string.Empty;
            if (TryGetItem(vin.Right(Constant.ShortVinLength), out SpecialPickItem specialPickItem))
            {
                if (vehicleRow == VehicleRow.Row1)
                {
                    if (specialPickItem.FrontPickMode == PickMode.ByPalletID)
                    {
                        code = "P";
                    }
                    else if (specialPickItem.FrontPickMode == PickMode.ByJobID)
                    {
                        code = "J";
                    }
                }
                else // row 2
                {
                    if (specialPickItem.RearPickMode == PickMode.ByPalletID)
                    {
                        code = "P";
                    }
                    else if (specialPickItem.RearPickMode == PickMode.ByJobID)
                    {
                        code = "J";
                    }
                }
            }
            return code;
        }

        public bool IsSpecialPickPallet(int palletID, int jobID)
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

    }
}
