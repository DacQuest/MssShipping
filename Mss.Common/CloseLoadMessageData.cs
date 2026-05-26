using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;

namespace Mss.Common
{
    [Serializable]
    public class CloseLoadMessageData : XMessageData
    {
        public static readonly string CloseLoadMessageTopic = "CloseLoadMessageTopic";

        public CloseLoadMessageData(
            SlugLetter slugLetter,
            string error)
        {
            SlugLetter = slugLetter;
            Error = error;
        }
        public CloseLoadMessageData(
            SlugLetter slugLetter,
            bool reopenLoad,
            XSystemEvent systemEvent)
        {
            SlugLetter = slugLetter;
            ReopenLoad = reopenLoad;
            SystemEvent = systemEvent;
        }

        public SlugLetter SlugLetter { get; } = SlugLetter.None;

        public bool ReopenLoad { get; } = false;

        public XSystemEvent SystemEvent { get; } = null;

        public string Error { get; set; }

    }
}
