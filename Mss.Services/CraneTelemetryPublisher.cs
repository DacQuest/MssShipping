using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core.SystemEvents.Processors;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Services
{
    public class CraneTelemetryPublisher : XSystemEventProcessor
    {
        public override void ProcessEvent(XSystemEvent systemEvent)
        {
            string messageTopic = string.Format(
                "{0}{1}{2}",
                Constant.CraneTelemetryTopicNameBase,
                XMessaging.MessageTopicDelimiter,
                systemEvent.Context);
            XStringMessageData messageData = new XStringMessageData(
                string.Format(
                    "{0}    {1}",
                    systemEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    systemEvent.Message));
            XMessaging.Publish(
                messageTopic,
                messageData,
                XMessageScopes.All,
                this);
        }
    }
}
