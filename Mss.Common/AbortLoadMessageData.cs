using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;

namespace Mss.Common
{
    [Serializable]
    public class AbortLoadMessageData : XMessageData
    {
        public static readonly string AbortLoadMessageTopic = "AbortLoadMessageTopic";

        public AbortLoadMessageData(SlugLetter slugLetter)
        {
            SlugLetter = slugLetter;
        }

        public AbortLoadMessageData(SlugLetter slugLetter, string error)
        {
            SlugLetter = slugLetter;
            Error = error;
        }

        public AbortLoadMessageData(
            SlugLetter slugLetter,
            bool autoRecoverBroadcast,
            XSystemEvent systemEvent)
        {
            SlugLetter = slugLetter;
            AutoRecoverBroadcast = autoRecoverBroadcast;
            SystemEvent = systemEvent;
        }

        public SlugLetter SlugLetter { get; } = SlugLetter.None;

        public bool AutoRecoverBroadcast { get; } = false;

        public XSystemEvent SystemEvent { get; } = null;

        public string Error { get; } = string.Empty;

    }
}
