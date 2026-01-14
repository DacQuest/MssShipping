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
    public partial class BinItem : XDataItem
    {
        public CraneNumber CraneNumber => GetCraneNumber(NodeIndex);

        public string BinStatusText => BinStatus.ToText();

        public string SearchResultsHoldCodeText { get; set; } = string.Empty;

        public int BinNumber => NodeIndex + 1;

        public int Location => NodeIndexToLocation(NodeIndex);

        public string BinLocation => LocationText;

        public string LocationText => Location.ToString();

        public int BinRowNumber => GetBinRowNumber(NodeIndex);

        public int BinHorizontalNumber => GetBinHorizontalNumber(NodeIndex);
        public string BinHorizontalNumberText => $"{BinHorizontalNumber:00}";

        public int BinVerticalNumber => GetBinVerticalNumber(NodeIndex);
        public string BinVerticalNumberText => $"{BinHorizontalNumber:00}";

        public static int GetBinRowNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.StorageSize, nodeIndex, nameof(nodeIndex));

            return _GetBinRowNumber(nodeIndex);
        }

        public static int GetBinHorizontalNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.StorageSize, nodeIndex, nameof(nodeIndex));

            return _GetBinHorizontalNumber(nodeIndex);
        }

        public static int GetBinVerticalNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.StorageSize, nodeIndex, nameof(nodeIndex));

            return _GetBinVerticalNumber(nodeIndex);
        }

        //         private static CraneNumber _GetCraneNumber(int nodeIndex)
        //         {
        //             return (CraneNumber)(nodeIndex / (Constant.MaxHorizontal * Constant.MaxVertical * 2) + 1);
        //
        //         }

        public static CraneNumber GetCraneNumber(int nodeIndex)
        {
            return (CraneNumber)(nodeIndex / (Constant.MaxHorizontal * Constant.MaxVertical * 2) + 1);

        }

        private static CraneNumber _LocationToCraneNumber(int location)
        {
            return GetCraneNumber(LocationToNodeIndex(location));
        }

        private static int _GetBinVerticalNumber(int nodeIndex)
        {
            return ((nodeIndex / Constant.CraneSides) % Constant.MaxVertical) + 1;
        }

        private static int _GetBinHorizontalNumber(int nodeIndex)
        {
            return ((nodeIndex / (Constant.CraneSides * Constant.MaxVertical) % Constant.MaxHorizontal) + 1);
        }

        private static int _GetBinRowNumber(int nodeIndex)
        {
            int group = nodeIndex / (Constant.MaxHorizontal * Constant.MaxVertical * Constant.CraneSides) + 1;
            int row = group * Constant.CraneSides;
            if (nodeIndex % Constant.CraneSides == 0)
            {
                row -= 1;
            }
            return row;
        }

        public static int NodeIndexToLocation(int nodeIndex)
        {
            int group = nodeIndex / (Constant.MaxHorizontal * Constant.MaxVertical * Constant.CraneSides) + 1;
            int row = group * Constant.CraneSides;
            if (nodeIndex % Constant.CraneSides == 0)
            {
                row -= 1;
            }
            row = row * 10000;
            int horizontal = ((nodeIndex / (Constant.CraneSides * Constant.MaxVertical) % Constant.MaxHorizontal) + 1) * 100;
            int vertical = ((nodeIndex / Constant.CraneSides) % Constant.MaxVertical) + 1;
            return row + horizontal + vertical;
        }

        public static int LocationToNodeIndex(int location)
        {
            int row = location / 10000;
            int horizontal = (location - row * 10000) / 100;
            int vertical = (location - row * 10000) - (horizontal * 100);

            return LocationToNodeIndex(row, horizontal, vertical);
        }

        public static int LocationToNodeIndex(
            int row,
            int horizontal,
            int vertical)
        {
            int index = ((((row + 1) / Constant.CraneSides) - 1) * Constant.StorageSize / Constant.MaxCranes) + Constant.CraneSides * (vertical - 1) + Constant.MaxVertical * Constant.CraneSides * (horizontal - 1);
            int rowOffset = 0;
            if (row % Constant.CraneSides == 0)
            {
                rowOffset = 1;
            }
            index += rowOffset;
            return index;
        }

        public static bool LocationTextToNodeIndex(string locationText, out int nodeIndex)
        {
            nodeIndex = -1;
            if (!int.TryParse(locationText, out int location))
            {
                return false;
            }
            if (location < 0 || !IsValidStorageLocation(location))
            {
                return false;
            }
            nodeIndex = LocationToNodeIndex(location);
            return true;
        }

        public static bool IsValidStorageLocation(int location)
        {
            int row = location / 10000;
            int horizontal = (location - row * 10000) / 100;
            int vertical = (location - row * 10000) - (horizontal * 100);

            return row >= 1 && row <= (Constant.MaxCranes * Constant.CraneSides)
                && horizontal >= 1 && horizontal <= Constant.MaxHorizontal
                && vertical >= 1 && vertical <= Constant.MaxVertical;
        }

        public void SetDuplicatePalletIDHold()
        {
            PalletItem pallet = Pallet;
            pallet.Status = PalletStatus.Hold;
            //pallet.HoldCode = Constant.DuplicatePalletIDHoldCode;
            pallet.Comment = Constant.AutoDetectedDuplicatePalletIDComment;
            Pallet = pallet;
        }

        public void SetDuplicateJobIDHold()
        {
            PalletItem pallet = Pallet;
            pallet.Status = PalletStatus.Hold;
            //pallet.HoldCode = Constant.DuplicateJobIDHoldCode;
            pallet.Comment = Constant.AutoDetectedDuplicateJobIDComment;
            Pallet = pallet;
        }

        public void MarkAsDuplicateForAudit()
        {
            Clear(BinStatus.Offline);
            Audit = true;
        }

        public void ClearPallet()
        {
            Pallet = new PalletItem();
        }

        public void Clear()
        {
            Clear(BinStatus.Empty, false);
        }

        public void Clear(BinStatus newStatus)
        {
            Clear(newStatus, false);
        }

        public void Clear(BinStatus newStatus, bool markForAudit)
        {
            ClearPallet();
            Audit = markForAudit;
            BinStatus = newStatus;
            AuditAttempts = 0;
            StoredOn = Constant.BeginningOfTime;
        }

        public string ReportBinStatusText
        {
            get
            {
                if (NotUsable)
                {
                    return "[NOT USABLE]";
                }
                string binStatusText = BinStatusText;
                if (Audit)
                {
                    binStatusText += " [A]";
                }
                if (PickOnly)
                {
                    binStatusText += " [P]";
                }
                if (Disabled)
                {
                    binStatusText += " [D]";
                }
                return binStatusText;
            }
        }
    }
}
