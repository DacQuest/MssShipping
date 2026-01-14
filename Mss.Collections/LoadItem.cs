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
    public partial class LoadItem : XDataItem
    {
        public Boolean Shortage
        {
            get;
            set;
        }

        public Int32 ReportPosition
        {
            get
            {
                return NodeIndex + 1;
            }
        }

        public OutboundLevels Level
        {
            get
            {
                if (NodeIndex >= 0 && NodeIndex < Constant.LoadSize / 2)
                {
                    return OutboundLevels.Lower;
                }
                else
                {
                    return OutboundLevels.Upper;
                }
            }
        }

        public Int32 TransferMoveCommand
        {
            get
            {
                switch (GetLaneFromIndex(NodeIndex))
                {
                    case 1:
                        return Constant.CommandMoveToSlugLane1;
                    case 2:
                        return Constant.CommandMoveToSlugLane2;
                    case 3:
                        return Constant.CommandMoveToSlugLane3;
                }
                //TODO: Publish System Event
                return 0; // this cannot happen
            }
        }

        public static (int Position, int Lane, OutboundLevels Level) ConvertIndexToPositionLaneLevel(int loadIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");

            Int32 position = -1;
            Int32 lane = -1;
            OutboundLevels level = OutboundLevels.Lower;

            lane = GetLaneFromIndex(loadIndex);
            if ((loadIndex >= 0) && (loadIndex < Constant.LoadSize / 2))
            {
                level = OutboundLevels.Lower;
                position = (loadIndex / 3) + 1;
            }
            else if ((loadIndex >= Constant.LoadSize / 2) && (loadIndex < Constant.LoadSize))
            {
                level = OutboundLevels.Upper;
                position = ((loadIndex - (Constant.LoadSize / 2)) / 3) + 1;
            }

            return (position, lane, level);
        }

        public static Int32 GetLaneFromIndex(Int32 loadIndex)
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
    }

}

