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

        public bool IsStack => Sku == Constant.FrontStackSku
            || Sku == Constant.RearStackSku;

        public string BuiltOnText => BuiltOn > Constant.BeginningOfTime
            ? BuiltOn.ToString(Constant.DateTimeFormat)
            : string.Empty;

        public string GetStateDetails(int leadingSpaceCount)
        {
            string spaces = string.Concat(Enumerable.Repeat(' ', leadingSpaceCount));

            StringBuilder details = new StringBuilder();
            _ = details.Append($"\r\n{spaces}Pallet ID:   {PalletID}");
            _ = details.Append($"\r\n{spaces}Status:   {Status.ToText()}");
            _ = details.Append($"\r\n{spaces}Hold Code:   {HoldCode}");
            _ = details.Append($"\r\n{spaces}Sku:   {Sku}");
            _ = details.Append($"\r\n{spaces}JobID:   {JobID}");
            _ = details.Append($"\r\n{spaces}Vehicle Row:   {VehicleRow.ToText()}");
            _ = details.Append($"\r\n{spaces}Built On:   {BuiltOnText}");
            _ = details.Append($"\r\n{spaces}Comment:   {Comment}");

            return details.ToString();
        }

    }
}
