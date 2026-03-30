using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    public class Broadcast : XSharedDictionary<string, BroadcastItem>
    {
        public void PurgeOldBroadcast()
        {
            _ = Lock();
            bool oldSetting = InhibitChangeNotifications;
            InhibitChangeNotifications = true;
            bool touch = false;
            try
            {
                int activeCount = this.Where(b => b.Value.Active).Count();
                if (activeCount >= (int)(ItemCount * 0.95F))
                {
                    int countToPurge = activeCount - (int)(ItemCount * 0.8F);
//                     countToPurge -= countToPurge % 2 == 1 ? 1 : 0;
                    IEnumerable<BroadcastItem> listToPurge = Values
                        .Where(b =>
                        {
                            return b.Status == BroadcastStatus.Shipped
                                || b.Status == BroadcastStatus.Skip;
                        })
                        .OrderBy(b => b.ReceivedOn)
                        .Take(countToPurge);
                    foreach (BroadcastItem broadcastItem in listToPurge)
                    {
                        _ = Remove(broadcastItem.Csn);
                        touch = true;
                    }
                }
            }
            catch (Exception x)
            {
                x.PublishSystemEvent("Broadcast.PurgeOldBroadcasts()");
            }
            finally
            {
                InhibitChangeNotifications = oldSetting;
                if (touch)
                {
                    Touch();
                }
                Unlock();
            }
        }
    }
}
