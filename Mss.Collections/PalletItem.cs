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

        public bool IsStack => IsFrontStack || IsRearStack;

        public bool IsFrontStack => Sku == Constant.StackSku1;

        public bool IsRearStack => Sku == Constant.StackSku2;

        public BinSize BinSize => VehicleRow == VehicleRow.Row1 && !IsStack
            ? BinSize.Large
            : BinSize.Small;

        public string BuiltOnText => BuiltOn > Constant.BeginningOfTime
            ? BuiltOn.ToString(Constant.LongDateTimeFormat24)
            : string.Empty;

        public string StatusText => Status.ToText();
        public string HoldCodeText => HoldCode.ToText();

        public string VehicleRowText => VehicleRow.ToText();

        public string GetStateDetails(int leadingSpaceCount)
        {
            string spaces = string.Concat(Enumerable.Repeat(' ', leadingSpaceCount));

            StringBuilder details = new StringBuilder();
            _ = details.Append($"\r\n{spaces}Pallet ID:   {PalletID}");
            _ = details.Append($"\r\n{spaces}Status:   {StatusText}");
            _ = details.Append($"\r\n{spaces}Hold Code:   {HoldCode}");
            _ = details.Append($"\r\n{spaces}Sku:   {Sku}");
            _ = details.Append($"\r\n{spaces}JobID:   {JobID}");
            _ = details.Append($"\r\n{spaces}Vehicle Row:   {VehicleRowText}");
            _ = details.Append($"\r\n{spaces}Built On:   {BuiltOnText}");
            _ = details.Append($"\r\n{spaces}Comment:   {Comment}");

            return details.ToString();
        }

    }
}
