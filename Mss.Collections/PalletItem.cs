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
    public partial class PalletItem : XDataItem
    {
        public VehicleRow VehicleRow => XValueValidator.Validate(Constant.Row1PalletIDValidatorName, PalletID)
        ? VehicleRow.Row1
        : VehicleRow.Row2;

    }
}
