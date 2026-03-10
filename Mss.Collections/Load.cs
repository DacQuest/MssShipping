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
    public abstract class Load : XSharedArray<LoadItem>
    {
        public abstract LoadLetter LoadLetter { get; }

        public bool TryFindByPalletID(
            string palletID,
            out LoadItem loadItem)
        {
            XArgumentChecker.ThrowIfNullOrWhitespace(palletID, nameof(palletID));

            _ = Lock();
            try
            {
                loadItem = this.FirstOrDefault(l => l.Pallet.PalletID == palletID);
                return loadItem != null;
            }
            finally
            {
                Unlock();
            }
        }

        public bool AllPickedOrGreater
        {
            get
            {
                _ = Lock();
                try
                {
                    return this.All(l =>
                    {
                        LoadItemStatus status = l.Status;
                        return status == LoadItemStatus.Invalid
                            || status >= LoadItemStatus.Picked;
                    });
                }
                finally
                {
                    Unlock();
                }
            }
        }

        public int WorkToDoCount
        {
            get
            {
                _ = Lock();
                try
                {
                    return this.Count(l =>
                    {
                        LoadItemStatus status = l.Status;
                        return status != LoadItemStatus.Invalid
                            && status <= LoadItemStatus.Pickable;
                    });
                }
                finally
                {
                    Unlock();
                }
            }
        }





        public void RollbackPick(string palletID)
        {
            _ = Lock();
            try
            {
                if (!palletID.ValidPalletID()
                    || !TryFindByPalletID(palletID, out LoadItem loadItem))
                {
                    return;
                }
                loadItem.Status = LoadItemStatus.Pickable;
                loadItem.Pallet = new PalletItem();
                loadItem.Crane = CraneNumber.None;
                loadItem.PickedOn = Constant.BeginningOfTime;

                this[loadItem.NodeIndex] = loadItem;
            }
            finally
            {
                Unlock();
            }
        }

        public void SetNextInLanePickable(int currentLoadItemIndex)
        {
            int nextIndex = Constant.NextInLaneLoadIndex[currentLoadItemIndex];
            if (nextIndex != Constant.AfterLast)
            {
                _ = Lock();
                LoadItem nextLoadItem = this[nextIndex];
                if (nextLoadItem.Status == LoadItemStatus.Pending)
                {
                    nextLoadItem.Status = LoadItemStatus.Pickable;
                    this[nextLoadItem.NodeIndex] = nextLoadItem;
                }
                Unlock();
            }
        }

        public List<LoadItem> GetLoadInPickSearchOrder()
        {
            _ = Lock();
            try
            {
                List<LoadItem> loadItems = new List<LoadItem>();
                for (int index = 0; index < CollectionConfiguration.ItemCount; index++)
                {
                    loadItems.Add(this[Constant.PickSearchOrder[index]]);
                }
                return loadItems;
            }
            finally
            {
                Unlock();
            }
        }

//         private static readonly int[] _nextInLaneLoadIndex = new int[]
//         {
//             //Lower Level
//             03, 04, 05,
//             06, 07, 08,
//             09, 10, 11,
//             12, 13, 14,
//             Constant.AfterLast, Constant.AfterLast, Constant.AfterLast,
// 
//             Constant.AfterLast, Constant.AfterLast, Constant.AfterLast,
//             15, 16, 17,
//             18, 19, 20,
//             21, 22, 23,
//             24, 25, 26,
// 
//             //Upper Level
//             33, 34, 35,
//             36, 37, 38,
//             39, 40, 41,
//             42, 43, 44,
//             Constant.AfterLast, Constant.AfterLast, Constant.AfterLast,
// 
//             Constant.AfterLast, Constant.AfterLast, Constant.AfterLast,
//             45, 46, 47,
//             48, 49, 50,
//             51, 52, 53,
//             54, 55, 56
//         };
// 
//         private static readonly int[] _previousInLaneLoadIndex = new int[]
//         {
//             //Lower
//             Constant.BeforeFirst, Constant.BeforeFirst, Constant.BeforeFirst,
//             00, 01, 02,
//             03, 04, 05,
//             06, 07, 08,
//             09, 10, 11,
// 
//             18, 19, 20,
//             21, 22, 23,
//             24, 25, 26,
//             27, 28, 29,
//             Constant.BeforeFirst, Constant.BeforeFirst, Constant.BeforeFirst,
// 
//             // Upper
//             Constant.BeforeFirst, Constant.BeforeFirst, Constant.BeforeFirst,
//             30, 31, 32,
//             33, 34, 35,
//             36, 37, 38,
//             39, 40, 41,
// 
//             48, 49, 50,
//             51, 52, 53,
//             54, 55, 56,
//             57, 58, 59,
//             Constant.BeforeFirst, Constant.BeforeFirst, Constant.BeforeFirst,
//         };
// 
    }
}
