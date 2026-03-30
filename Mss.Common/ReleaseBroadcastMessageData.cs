using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Serializable]
    public class ReleaseBroadcastMessageData : XMessageData
    {
        public static readonly string ReleaseBroadcastMessageTopicName = "ReleaseBroadcastMessageTopic";
        public static readonly int BroadcastReleaseTimeoutMilliseconds = 20000;

        public ReleaseBroadcastMessageData(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ReleaseBroadcastMessageData(
            XSystemEvent systemEvent)
        {
            SystemEvent = systemEvent;
        }


        public ReleaseBroadcastMessageData(
            LoadLetter loadLetter,
            int countToRelease,
            XSystemEvent systemEvent)
        {
            LoadLetter = loadLetter;
            CountToRelease = countToRelease;
            SystemEvent = systemEvent;
        }

        public LoadLetter LoadLetter { get; } = LoadLetter.None;

        public int CountToRelease { get; } = 0;

        public XSystemEvent SystemEvent { get; } = null;

        public string ErrorMessage { get; set; } = string.Empty;

    }
}
