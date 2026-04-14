using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.Strings;
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
//         public int PickModeJobID => int.Parse(PickModeValue);
// 
//         public string PickModePalletID => PickModeValue;

        public bool IsFrontBackBroadcast => Csn.EndsWith(Constant.VehicleRow1CsnSuffix);

        public bool IsBackBroadcast => Csn.EndsWith(Constant.VehicleRow2CsnSuffix);

        public bool IsAutoSkip => AutoSkipFromRotation(Rotation);

        public int Rotation => RotationFromCsn(Csn);

        public static string MakeCsn(int rotationNumber, string csnSuffix)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                csnSuffix,
                nameof(csnSuffix),
                new string[] { Constant.VehicleRow1CsnSuffix, Constant.VehicleRow2CsnSuffix });

            return $"{rotationNumber.ToString(Constant.RotationNumberTextFormat)}{csnSuffix}";
        }

        public static bool AutoSkipFromRotation(int rotation)
        {
            int mod = rotation % 10000;
            return mod == 0 || mod > Constant.MaxRotation;
        }

        public static int RotationFromCsn(string csn)
        {
            if (csn.Length < 2)
            {
                throw new ArgumentException(
                    "Length of 'csn' must be at least 2 characters long",
                    nameof(csn));
            }
            string rotationText = csn.Left(csn.Length - 1);
            if (!rotationText.All(c => char.IsDigit(c)))
            {
                throw new ArgumentException(
                    "All but last character of 'csn' must a digit",
                    nameof(csn));
            }
            return int.Parse(rotationText);
        }

        public static BroadcastItem CreateMissingBroadcastItem(int rotation)
        {
            return new BroadcastItem
            {
                Status = BroadcastStatus.Missing,
                Csn = MakeCsn(rotation, Constant.VehicleRow1CsnSuffix),
                Sku = string.Empty,
                VehicleSku = string.Empty,
                Vin = string.Empty,
                PickMode = PickMode.BySku,
                PickModeKey = string.Empty,
                VehicleRowCount = 0,
                ReceivedOn = DateTime.Now
            };
        }

        public static BroadcastItem CreateSkipBroadcastItem(int rotation)
        {
            return new BroadcastItem
            {
                Status = BroadcastStatus.Skip,
                Csn = MakeCsn(rotation, Constant.VehicleRow1CsnSuffix),
                Sku = string.Empty,
                VehicleSku = string.Empty,
                Vin = string.Empty,
                PickMode = PickMode.BySku,
                PickModeKey = string.Empty,
                VehicleRowCount = 0,
                ReceivedOn = DateTime.Now
            };
        }

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
            _ = details.Append($"\r\n{spaces}Rotation:   {Rotation}");
            _ = details.Append($"\r\n{spaces}Vehicle SKU:   {VehicleSku}");
            _ = details.Append($"\r\n{spaces}ReceivedOn:   {ReceivedOnText}");
            _ = details.Append($"\r\n{spaces}Vehicle Row Count:   {VehicleRowCount}");
            _ = details.Append($"\r\n{spaces}Pick Mode:   {PickMode.ToText()}");
            _ = details.Append($"\r\n{spaces}Pick Mode Key:   {PickModeKey}");

            return details.ToString();
        }

    }
}
