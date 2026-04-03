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
    public abstract class Slug : XSharedArray<LoadItem>
    {
        public abstract SlugLetter SlugLetter { get; }

        public void SafeClear()
        {
            SafeClear(false);
        }

        public void SafeClear(bool inhibitChangeNotifications)
        {
            _ = Lock();
            try
            {
                Clear(inhibitChangeNotifications);
                for (int nodeIndex = 0; nodeIndex < CollectionConfiguration.ItemCount; nodeIndex++)
                {
                    LoadItem loadItem = this[nodeIndex];
                    if (loadItem.SlugLetter != SlugLetter)
                    {
                        loadItem.SlugLetter = SlugLetter;
                        _ = SetAt(loadItem.NodeIndex, loadItem, inhibitChangeNotifications);
                    }
                }
            }
            finally
            {
                Unlock();
            }
        }

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
                            && status >= LoadItemStatus.Waiting
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

        public bool IsInvalid
        {
            get
            {
                _ = Lock();
                try
                {
                    return this.All(l => l.IsInvalid);
                }
                finally
                {
                    Unlock();
                }
            }
        }

        public int WaitingCount
        {
            get
            {
                _ = Lock();
                try
                {
                    return this.Count(l => l.Status == LoadItemStatus.Waiting);
                }
                finally
                {
                    Unlock();
                }
            }
        }

        public bool LoadDone
        {
            get
            {
                _ = Lock();
                try
                {
                    return LevelDone(Levels.Upper)
                        && LevelDone(Levels.Lower);
                }
                finally
                {
                    Unlock();
                }
            }
        }

        public bool LevelDone(Levels level)
        {
            _ = Lock();
            try
            {
                return level != Levels.None
                    && this
                        .Where(l => level == l.SlugLevel)
                        .All(l => l.IsDoneOrInvalid)
                    && !this
                        .All(l => l.IsInvalid);
            }
            finally
            {
                Unlock();
            }
        }

        public void SetLoadable()
        {
            _ = Lock();
            try
            {
                if (LoadDone)
                {
                    foreach (LoadItem loadItem in this)
                    {
                        if (loadItem.Status == LoadItemStatus.Done)
                        {
                            loadItem.Status = LoadItemStatus.Loadable;
                            _ = SetAt(loadItem.NodeIndex, loadItem, true);
                        }
                    }
                    Touch();
                }
            }
            finally
            {
                Unlock();
            }
        }

        public bool LoadLoadable
        {
            get
            {
                _ = Lock();
                try
                {
                    return this.All(l => l.Status == LoadItemStatus.Loadable || l.IsInvalid)
                        && !this.All(l => l.IsInvalid);
                }
                finally
                {
                    Unlock();
                }
            }
        }

    }
}
