using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.Strings;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public string RotationText => RotationTextFromCsn(Csn);

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
            string rotationText = RotationTextFromCsn(csn);
            if (!rotationText.All(c => char.IsDigit(c)))
            {
                throw new ArgumentException(
                    "All but last character of 'csn' must a digit",
                    nameof(csn));
            }
            return int.Parse(rotationText);
        }

        public static string RotationTextFromCsn(string csn)
        {
            return csn.Left(csn.Length - 1);
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
//             ? ReceivedOn.ToString(Constant.DateTimeFormat)
            ? ReceivedOn.ToString("G")
            : string.Empty;

        public string StatusText => Status.ToText();

        public Image StatusImage
        {
            get
            {
                Image image = Properties.Resources.RoundRedBang16;
                switch (Status)
                {
                    case BroadcastStatus.Invalid:
                    case BroadcastStatus.Missing:
                        image = Properties.Resources.RoundRedBang16;
                        break;
                    case BroadcastStatus.Shipped:
                    case BroadcastStatus.Skip:
                        image = Properties.Resources.RoundBlackDash16;
                        break;
                    case BroadcastStatus.OK:
                        image = Shortage ? Properties.Resources.RoundYellowBangBorder16 : Properties.Resources.RoundGreenCheck16;
                        break;
                }
                return image;
            }
        }
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
