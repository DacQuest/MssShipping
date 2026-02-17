using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Mss.Common;

namespace Mss.Collections
{
    [Serializable]
    public partial class BinItem : XDataItem
    {
        public void Clear()
        {
            Pallet = new PalletItem();
            Audit = false;
            BinStatus = BinStatus.Empty;
            StoredOn = Constant.BeginningOfTime;
        }
        public void ClearPallet()
        {
            Pallet = new PalletItem();
        }


        public void FlagAsDuplicateForAudit()
        {
            Clear();
            Audit = true;
            BinStatus = BinStatus.OfflineDuplicatePalletID;
        }

        public void MarkAsDuplicateForAudit()
        {
            Clear(BinStatus.Offline);
            Audit = true;
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
            //             AuditAttempts = 0;
            StoredOn = Constant.BeginningOfTime;
        }
        public void SetDuplicateJobIDHold()
        {
            PalletItem pallet = Pallet;
            pallet.Status = PalletStatus.Hold;
            pallet.Comment = Constant.AutoDetectedDuplicateJobIDComment;
            Pallet = pallet;
        }
        public void SetDuplicatePalletIDHold()
        {
            PalletItem pallet = Pallet;
            pallet.Status = PalletStatus.Hold;
            pallet.Comment = Constant.AutoDetectedDuplicatePalletIDComment;
            Pallet = pallet;
        }






        public CraneNumber CraneNumber => GetCraneNumber(NodeIndex);
        public int BinNumber => NodeIndex + 1;
        public string LocationText => NodeIndexToLocationText(NodeIndex);
        public int Location => int.Parse(LocationText);
        public int BinVerticalNumber => GetBinVerticalNumber(NodeIndex);
        public int BinHorizontalNumber => GetBinHorizontalNumber(NodeIndex);
        public int BinSideNumber => GetBinSideNumber(NodeIndex);
//         public bool IsBinEmpty => BinStatus == BinStatus.Empty;

        public static int LocationTextToNodeIndex(string locationText)
            => LocationToNodeIndex(int.Parse(locationText));

        public static int LocationToNodeIndex(int location)
        {
            int c = location / 10000;
            int wl = location - (c * 10000);
            int s = wl / 1000 % 2 == 0 ? 2 : 1;
            wl -= (s * 1000);
            int h = wl / 10;
            int v = wl - (h * 10);
            return ((h - 1) * Constant.MaxVertical * Constant.CraneSides)
                + ((v - 1) * Constant.CraneSides)
                + s - 1
                + ((c - 1) * Constant.StoragePerCrane);
        }

        public static int LocationCoordinatesToNodeIndex(
            int crane,
            int side,
            int horizontal,
            int vertical)
        {
            return LocationTextToNodeIndex($"{crane}{side}{horizontal:00}{vertical}");
        }

        public static CraneNumber GetCraneNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.StorageSize, nodeIndex, nameof(nodeIndex));

            return (CraneNumber)((nodeIndex / Constant.StoragePerCrane) + 1);

        }

        public static int GetBinSideNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThan(Constant.StorageSize - 1, nodeIndex, nameof(nodeIndex));

            return (nodeIndex % 2) + 1;
        }

        public static int GetBinHorizontalNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.StorageSize, nodeIndex, nameof(nodeIndex));

            return (nodeIndex / (Constant.CraneSides * Constant.MaxVertical) % Constant.MaxHorizontal) + 1;
        }

        public static int GetBinVerticalNumber(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThan(Constant.StorageSize - 1, nodeIndex, nameof(nodeIndex));

            return (nodeIndex / Constant.CraneSides % Constant.MaxVertical) + 1;
        }

        public static string NodeIndexToLocationText(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThan(Constant.StorageSize - 1, nodeIndex, nameof(nodeIndex));

            return $"{(int)GetCraneNumber(nodeIndex)}{GetBinSideNumber(nodeIndex)}{GetBinHorizontalNumber(nodeIndex):00}{GetBinVerticalNumber(nodeIndex)}";
        }

    }
}
