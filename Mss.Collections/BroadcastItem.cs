using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mss.Collections
{
    [Serializable]
    public partial class BroadcastItem : XDataItem
    {
        public int PickModeJobID => int.Parse(PickModeValue);

        public string PickModePalletID => PickModeValue;

        public bool IsFrontBackBroadcast => Csn.EndsWith(Constant.VehicleRow1CsnCode);

        public bool IsBackBroadcast => Csn.EndsWith(Constant.VehicleRow2CsnCode);

        public string ReceivedOnText => ReceivedOn > Constant.BeginningOfTime
            ? ReceivedOn.ToString(Constant.DateTimeFormat)
            : string.Empty;

        public string GetStateDetails(int leadingSpaceCount)
        {
            string spaces = string.Concat(Enumerable.Repeat(' ', leadingSpaceCount));

            StringBuilder details = new StringBuilder();
            _ = details.Append($"\r\n{spaces}Status:   {Status.ToText()}");
            _ = details.Append($"\r\n{spaces}CSN:   {Csn}");
            _ = details.Append($"\r\n{spaces}SKU:   {Sku}");
            _ = details.Append($"\r\n{spaces}VIN:   {Vin}");
            _ = details.Append($"\r\n{spaces}Pick Mode:   {PickMode.ToText()}");
            _ = details.Append($"\r\n{spaces}Rotation:   {RotationNumber}");
            _ = details.Append($"\r\n{spaces}Vehicle SKU:   {VehicleSku}");
            _ = details.Append($"\r\n{spaces}ReceivedOn:   {ReceivedOnText}");
            _ = details.Append($"\r\n{spaces}Vehicle Row Count:   {VehicleRowCount}");

            return details.ToString();
        }

    }
}
