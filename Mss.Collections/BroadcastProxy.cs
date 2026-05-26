using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Proxy;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    public class BroadcastProxy : XSharedDictionaryProxy<string, BroadcastItem>
    {

        public int ReleasableBroadcastItemCount => GetReleasableItems().Count;

        public List<BroadcastItem> GetReleasableItems()
        {
            List<BroadcastItem> releasableItems = new List<BroadcastItem>();
            IEnumerable<BroadcastItem> currentItems = Values.OrderBy(b => b.Csn);

            foreach (BroadcastItem item in currentItems)
            {
                BroadcastStatus status = item.Status;
                if (status == BroadcastStatus.OK)
                {
                    releasableItems.Add(item);
                }
                else if (status != BroadcastStatus.Skip)
                {
                    break;
                }
            }
            return releasableItems;
        }

    }
}
