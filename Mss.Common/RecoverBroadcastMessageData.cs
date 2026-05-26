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
    public class RecoverBroadcastMessageData : XMessageData
    {
        public static readonly string RecoverBroadcastMessageTopicName = "RecoverBroadcastMessageTopic";
        public static readonly int RecoverBroadcastTimeoutMilliseconds = 20000;

        public RecoverBroadcastMessageData(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public RecoverBroadcastMessageData(XSystemEvent systemEvent)
        {
            SystemEvent = systemEvent;
        }

        public XSystemEvent SystemEvent { get; } = null;

        public string ErrorMessage { get; set; }

    }
}
