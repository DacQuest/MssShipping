using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public class RollbackLoadItemMessageData : XMessageData
    {
        public const string RollbackRequest = "RollbackRequest";

        public RollbackLoadItemMessageData(LoadLetter loadLetter, int loadIndex)
        {
            XArgumentChecker.ThrowIfLessThanZero(loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");

            LoadLetter = loadLetter;
            LoadIndex = loadIndex;
        }

        public LoadLetter LoadLetter { get; } = LoadLetter.None;

        public int LoadIndex { get; } = -1;

    }
}
