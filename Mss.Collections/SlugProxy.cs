using DacQuest.DFX.Core.DataItems.Proxy;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Collections
{
    public class SlugProxy : XSharedArrayProxy<LoadItem>
    {

        public List<LoadItem> Items => ProxyList.ToList();

        public bool Cleared => Items.All(l => l.IsInvalid);

        public bool Completed
        {
            get
            {
                List<LoadItem> load = Items;
                return !load.All(l => l.Status == LoadItemStatus.Invalid)
                    && load.All(m => m.Status == LoadItemStatus.Invalid || m.Status == LoadItemStatus.Done);
            }
        }

        public SlugLetter SlugLetter
        {
            get
            {
                if (CollectionName == Constant.SlugAName)
                {
                    return SlugLetter.A;
                }
                else if (CollectionName == Constant.SlugBName)
                {
                    return SlugLetter.B;
                }
                return SlugLetter.None;
            }
        }

        public List<LoadItem> GetSlugInPickSearchOrder()
        {
            List<LoadItem> allItems = Items;
            List<LoadItem> orderedLoadItems = new List<LoadItem>();
            for (int index = 0; index < CollectionConfiguration.ItemCount; index++)
            {
                orderedLoadItems.Add(allItems[Constant.PickSearchOrder[index]]);
            }
            return orderedLoadItems;
        }

        public IEnumerable<LoadItem> GetPickableLoadItems()
        {
            return GetSlugInPickSearchOrder()
                .Where(l => l.Status == LoadItemStatus.Pickable);
        }

        public int WaitingCount => Cleared
            ? Constant.LoadSize
            : Items.Count(l => l.Status == LoadItemStatus.Waiting);

        public bool HasOpenLoad => WaitingCount > 0 && WaitingCount < Constant.LoadSize;

        public bool IsInvalid => Items.All(l => l.IsInvalid);

    }
}
