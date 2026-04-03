using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;

namespace Mss.Common
{
    [Serializable]
    public class AcceptLoadMessageData : XMessageData
    {
        public static readonly string AcceptLoadMessageTopic = "AcceptLoadMessageTopic";

        public AcceptLoadMessageData(SlugLetter slugLetter)
        {
            SlugLetter = slugLetter;
        }

        public AcceptLoadMessageData(
            SlugLetter slugLetter, 
            string error)
        {
            SlugLetter = slugLetter;
            Error = error;
        }

        public AcceptLoadMessageData(
            SlugLetter slugLetter,
            XSystemEvent systemEvent)
        {
            SlugLetter = slugLetter;
            SystemEvent = systemEvent;
        }

        public SlugLetter SlugLetter { get; } = SlugLetter.None;

        public XSystemEvent SystemEvent { get; } = null;

        public string Error { get; } = string.Empty;

    }
}
