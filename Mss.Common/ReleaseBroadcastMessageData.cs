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

        public ReleaseBroadcastMessageData()
        {
        }

        public ReleaseBroadcastMessageData(string error)
        {
            Error = error;
        }

        public ReleaseBroadcastMessageData(
            XSystemEvent systemEvent)
        {
            SystemEvent = systemEvent;
        }


        public ReleaseBroadcastMessageData(
            SlugLetter slugLetter,
            int countToRelease,
            XSystemEvent systemEvent)
        {
            SlugLetter = slugLetter;
            CountToRelease = countToRelease;
            SystemEvent = systemEvent;
        }

        public SlugLetter SlugLetter { get; } = SlugLetter.None;

        public int CountToRelease { get; } = 0;

        public XSystemEvent SystemEvent { get; } = null;

        public string Error { get; } = string.Empty;

    }
}
