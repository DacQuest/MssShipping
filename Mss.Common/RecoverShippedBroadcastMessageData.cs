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
    public class RecoverShippedBroadcastMessageData : XMessageData
    {
        public static readonly string RecoverShippedBroadcastMessageTopicName = "RecoverShippedBroadcastMessageTopic";

        public RecoverShippedBroadcastMessageData(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public RecoverShippedBroadcastMessageData(XSystemEvent systemEvent)
        {
            SystemEvent = systemEvent;
        }

        public XSystemEvent SystemEvent { get; } = null;

        public string ErrorMessage { get; set; }

    }
}
