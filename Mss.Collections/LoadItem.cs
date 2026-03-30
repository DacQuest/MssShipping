using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;

namespace Mss.Collections
{
    [Serializable]
    public partial class LoadItem : XDataItem
    {
        //!!! THIS VALUE SHOULD BE EITHER 4 OR 5
        public static int RowsTowardDoors = 4;

        public int LoadCommandOffset => LoadLetter == LoadLetter.A ? 0 : 6;

        public string Coordinates
        {
            get
            {
                return $"{LoadLetter}/Level:{LoadLevel}/Lane:{LoadLane}/Row:{LoadRow}";
            }
        }

        public static int GridRowIndexFromNodeIndex(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, nodeIndex, nameof(nodeIndex));

            int rowIndex = (nodeIndex % (Constant.LoadSize / 2) / 3) + 1;

            //!!! USE THIS IF THERE ARE 9 ROWS ON THE SLUG/LOAD!
            int rowCount = 10;
            //!!! USE THIS IF THERE ARE 10 ROWS ON THE SLUG/LOAD!
//             int rowCount = 11;
            //!!! USE THIS IF THERE ARE 11 ROWS ON THE SLUG/LOAD!
//             int rowCount = 12;

            rowIndex = rowCount - rowIndex;
            return rowIndex;
        }

        public static Levels LevelFromNodeIndex(int nodeIndex)
            => nodeIndex < Constant.LoadSize / 2
                ? Levels.Upper
                : Levels.Lower;

        public static int LaneFromNodeIndex(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, nodeIndex, nameof(nodeIndex));

            int lane = (nodeIndex % (Constant.LoadSize / 2) % 3) + 1;

            //!!! USE THIS IF NodeIndex==0 IS IN THE LOWER RIGHT CORNER OF THE GRID INSTEAD OF LOWER LEFT
//             lane = 4 - lane;

            return lane;
        }

        public static int RowNumberFromNodeIndex(int nodeIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(nodeIndex, nameof(nodeIndex));
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, nodeIndex, nameof(nodeIndex));

            int row = GridRowIndexFromNodeIndex(nodeIndex);

            //!!! USE THIS IF ROWS SHOULD BE NUMBERED 9-1 FROM THE DOORS!
            row = 10 - row;
            //!!! USE THIS IF ROWS SHOULD BE NUMBERED 10-1 FROM THE DOORS!
//             row = 11 - row;
            //!!! USE THIS IF ROWS SHOULD BE NUMBERED 11-1 FROM THE DOORS!
//             row = 12 - row;

            return row;
        }

        public static int TransferMoveCommandFromNodeIndex(int loadCommandOffset, int nodeIndex)
        {
            int firstNodeIndexTowardDoors = (Constant.LoadSize / 2) - (RowsTowardDoors * 3);
            int lane = LaneFromNodeIndex(nodeIndex);

            return nodeIndex % (Constant.LoadSize / 2) < firstNodeIndexTowardDoors
                        ? lane + loadCommandOffset
                        : lane + 3 + loadCommandOffset;
        }

        public Levels LoadLevel => LevelFromNodeIndex(NodeIndex);

        public int LoadLane => LaneFromNodeIndex(NodeIndex);

        public int LoadRow => RowNumberFromNodeIndex(NodeIndex);

        public int LoadGridRowIndex => GridRowIndexFromNodeIndex(NodeIndex);

        public int TransferMoveCommand
            => TransferMoveCommandFromNodeIndex(LoadCommandOffset, NodeIndex);

        public static (Color, Color) GetLoadStatusColors(
            LoadItemStatus status,
            bool transferring,
            bool flashInverted)
        {
            // Tuple is (ForeColor,BackColor)
            if (flashInverted)
            {
                return (Color.Black, Color.Yellow);
            }
            switch (status)
            {
                case LoadItemStatus.Invalid:
                    return (Color.White, Color.Black);
                case LoadItemStatus.Pending:
                    return (Color.White, Color.DarkCyan);
                case LoadItemStatus.Pickable:
                    return (Color.Black, Color.Cyan);
                case LoadItemStatus.Picking:
                    return (Color.White, Color.Blue);
                case LoadItemStatus.Picked:
                    return (Color.White, Color.Navy);
//                 case LoadItemStatus.Sequencing:
//                     return (Color.Black, Color.Lavender);
                case LoadItemStatus.Presequenced:
                    return (Color.White, Color.DarkMagenta);
                case LoadItemStatus.Sequenced:
                    return transferring
                        ? (Color.Black, Color.Yellow)
                        : (Color.White, Color.Magenta);
                case LoadItemStatus.Done:
                    return (Color.White, Color.DarkGreen);
//                 case LoadItemStatus.Loadable:
//                     return (Color.Black, Color.Pink);
                default:
                    return (Color.Yellow, Color.Red);
            }
        }

        public int ReportPosition
        {
            get
            {
                return NodeIndex + 1;
            }
        }

//         public int TransferMoveCommand
//         {
//             get
//             {
//                 if (LoadLetter == LoadLetter.A)
//                 {
//                     switch (GetLaneFromIndex(NodeIndex))
//                     {
//                         case 1:
//                             return Constant.TFMoveCommandToSlugLaneA1;
//                         case 2:
//                             return Constant.TFMoveCommandToSlugLaneA2;
//                         case 3:
//                             return Constant.TFMoveCommandToSlugLaneA3;
//                     }
// 
//                 }
//                 else if (LoadLetter == LoadLetter.B)
//                 {
//                     switch (GetLaneFromIndex(NodeIndex))
//                     {
//                         case 1:
//                             return Constant.TFMoveCommandToSlugLaneB1;
//                         case 2:
//                             return Constant.TFMoveCommandToSlugLaneB2;
//                         case 3:
//                             return Constant.TFMoveCommandToSlugLaneB3;
//                     }
//                 }
//                 //TODO: Publish System Event
//                 return 0; // this cannot happen
//             }
//         }

        public static (int Position, int Lane, Levels Level) ConvertIndexToPositionLaneLevel(int loadIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");

            int position = -1;
            int lane = -1;
            Levels level = Levels.Lower;

            lane = GetLaneFromIndex(loadIndex);
            if ((loadIndex >= 0) && (loadIndex < Constant.LoadSize / 2))
            {
                level = Levels.Lower;
                position = (loadIndex / 3) + 1;
            }
            else if ((loadIndex >= Constant.LoadSize / 2) && (loadIndex < Constant.LoadSize))
            {
                level = Levels.Upper;
                position = ((loadIndex - (Constant.LoadSize / 2)) / 3) + 1;
            }

            return (position, lane, level);
        }

        public static int GetLaneFromIndex(Int32 loadIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");

            return (loadIndex % 3) + 1;
            //             switch ((loadIndex % 3) + 1)
            //             {
            //             case 1:
            //                 return 3;
            //             case 3:
            //                 return 1;
            //             default:
            //                 return 2;
            //             }
        }

        //        public void SetNodeIndex(Int32 nodeIndex)
        //        {
        //            NodeIndex = nodeIndex;
        //        }

        public string PickedOnText => PickedOn > Constant.BeginningOfTime
            ? PickedOn.ToString(Constant.DateTimeFormat)
            : string.Empty;

        public string GetStateDetails(int leadingSpaceCount)
        {
            string spaces = string.Concat(Enumerable.Repeat(' ', leadingSpaceCount));

            StringBuilder details = new StringBuilder();
            _ = details.Append($"\r\n{spaces}Status:   {Status.ToText()}");
            _ = details.Append($"\r\n{spaces}Crane:   {Crane.ToText()}");
            _ = details.Append($"\r\n{spaces}Picked On:   {PickedOnText}");
            _ = details.Append($"\r\n{spaces}Load Letter:   {LoadLetter.ToText()}");
            _ = details.Append($"\r\n{spaces}Pick Mode:   {PickMode.ToText()}");
            _ = details.Append($"\r\n{spaces}Pick Mode Pallet ID:   {PickModePalletID}");
            _ = details.Append($"\r\n{spaces}Pick Mode Job ID:   {PickModeJobID}");

            _ = details.Append($"\r\n{spaces}Pallet:");
            _ = details.Append(Pallet.GetStateDetails(leadingSpaceCount * 2));

            _ = details.Append($"\r\n{spaces}Broadcast:");
            _ = details.Append(Broadcast.GetStateDetails(leadingSpaceCount * 2));

            return details.ToString();
        }

    }

}

