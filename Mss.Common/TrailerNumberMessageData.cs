using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;

namespace Mss.Common
{
    [Serializable]
    public class TrailerNumberMessageData : XMessageData
    {
        public static readonly string TrailerNumberRequestMessageTopicName = "TrailerNumberRequestMessageTopic";
        public static readonly string TrailerNumberMessageTopicName = "TrailerNumberMessageTopic";

        public TrailerNumberMessageData(SlugLetter slugLetter, string trailerNumber)
        {
            SlugLetter = slugLetter;
            TrailerNumber = trailerNumber;
        }

        public SlugLetter SlugLetter { get; } = SlugLetter.None;
        public string TrailerNumber { get; } = string.Empty;

    }
}
