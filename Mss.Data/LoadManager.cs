using DacQuest.DFX.Core;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data
{
    internal class LoadManager
    {
        private LoadA _loadA;
        private LoadB _loadB;

        internal LoadManager(LoadA loadA, LoadB loadB)
        {
            XArgumentChecker.ThrowIfNull(loadA, nameof(loadA));
            XArgumentChecker.ThrowIfNull(loadB, nameof(loadB));

            _loadA = loadA;
            _loadB = loadB;
        }

        internal void Lock()
        {
            _loadA.Lock();
            _loadB.Lock();
        }

        internal void Unlock()
        {
            _loadB.Unlock();
            _loadA.Unlock();
        }

        internal bool TryGetLoadByPalletID(
            string palletID,
            out Load load,
            out LoadItem loadItem)
        {
            try
            {
                Lock();
                if (_loadA.TryFindByPalletID(palletID, out loadItem))
                {
                    load = _loadA;
                    return true;
                }
                else if (_loadB.TryFindByPalletID(palletID, out loadItem))
                {
                    load = _loadB;
                    return true;
                }
                load = null;
                loadItem = null;
                return false;
            }
            finally
            {
                Unlock();
            }
        }

        internal bool TryGetLoadByLetter(LoadLetter letter, out Load load)
        {
            load = null;
//             if (letter == LoadLetter.None)
//             {
//                 return false;
//             }
            if (letter == LoadLetter.A)
            {
                load = _loadA;
            }
            else if (letter == LoadLetter.B)
            {
                load = _loadB;
            }
            return load != null;
        }

        internal bool TryGetPrimaryLoad(
            SystemSettingsItem systemSettingsItem,
            out Load primaryLoad,
            out Load secondaryLoad)
        {
            if (!TryGetPrimaryLoad(systemSettingsItem, out primaryLoad))
            {
                secondaryLoad = null;
                return false;
            }
            secondaryLoad = GetOtherLoad(primaryLoad);
            return true;
        }

        internal bool TryGetPrimaryLoad(
            SystemSettingsItem systemSettingsItem,
            out Load primaryLoad)
        {
            Lock();
            try
            {
                LoadPickPriority loadPickPriority = systemSettingsItem.LoadPickPriority;
                if (loadPickPriority == LoadPickPriority.Balanced)
                {
                    return _GetNoPriorityPrimaryLoad(out primaryLoad);
                }
                bool slugAPicksToDo = _loadA.Any(l =>
                    (l.Status == LoadItemStatus.Pending
                    || l.Status == LoadItemStatus.Pickable)
                    && (loadPickPriority == LoadPickPriority.SmallerLoadID
                        || loadPickPriority == LoadPickPriority.LoadA
                        || loadPickPriority == LoadPickPriority.LoadAOnly));
                bool slugBPicksToDo = _loadB.Any(l =>
                    (l.Status == LoadItemStatus.Pending
                    || l.Status == LoadItemStatus.Pickable)
                    && (loadPickPriority == LoadPickPriority.SmallerLoadID
                        || loadPickPriority == LoadPickPriority.LoadB
                        || loadPickPriority == LoadPickPriority.LoadBOnly));

                primaryLoad = null;
                if (!slugAPicksToDo && !slugBPicksToDo)
                {
                    primaryLoad = null;
                }
                else if (slugAPicksToDo && slugBPicksToDo)
                {
                    if (loadPickPriority == LoadPickPriority.LoadA
                        || loadPickPriority == LoadPickPriority.LoadAOnly)
                    {
                        primaryLoad = _loadA;
                    }
                    else if (loadPickPriority == LoadPickPriority.LoadB
                        || loadPickPriority == LoadPickPriority.LoadBOnly)
                    {
                        primaryLoad = _loadB;
                    }
                    else if (loadPickPriority == LoadPickPriority.SmallerLoadID)
                    {
                        int loadIDA = systemSettingsItem.LoadALoadID;
                        int loadIDB = systemSettingsItem.LoadBLoadID;
                        primaryLoad = loadIDA < loadIDB ? _loadA : (Load)_loadB;
                    }
                }
                else if (slugAPicksToDo
                    && !slugBPicksToDo
                    && loadPickPriority != LoadPickPriority.LoadBOnly)
                {
                    primaryLoad = _loadA;
                }
                else if (!slugAPicksToDo
                    && loadPickPriority != LoadPickPriority.LoadAOnly
                    && slugBPicksToDo)
                {
                    primaryLoad = _loadB;
                }
                return primaryLoad != null;
            }
            finally
            {
                Unlock();
            }
        }

        internal Load GetOtherLoad(Load load) => load == _loadA ? _loadB : (Load)_loadA;

        private bool _GetNoPriorityPrimaryLoad(out Load primaryLoad)
        {
            bool excludeLoadA = _loadA.AllPickedOrGreater;
            bool excludeLoadB = _loadB.AllPickedOrGreater;

            if (excludeLoadA && excludeLoadB)
            {
                primaryLoad = null;
            }
            else if (excludeLoadA)
            {
                primaryLoad = _loadB;
            }
            else if (excludeLoadB)
            {
                primaryLoad = _loadA;
            }
            else
            {
                int workToDoA = _loadA.WorkToDoCount;
                int workToDoB = _loadB.WorkToDoCount;
                primaryLoad = workToDoA < workToDoB ? _loadB : (Load)_loadA;
            }
            return primaryLoad != null;
        }

        internal void GetPickableLoadItems(
            Load primaryLoad,
            Load secondaryLoad,
            out List<LoadItem> primaryPickableItems,
            out List<LoadItem> secondaryPickableItems)
        {
            Lock();
            try
            {
                primaryPickableItems = primaryLoad
                    .GetLoadInPickSearchOrder()
                    .Where(l => l.Status == LoadItemStatus.Pickable)
                    .ToList();
                secondaryPickableItems = secondaryLoad
                    .GetLoadInPickSearchOrder()
                    .Where(l => l.Status == LoadItemStatus.Pickable)
                    .ToList();
            }
            finally
            {
                Unlock();
            }
        }

        internal void RollbackPick(string palletID)
        {
            try
            {
                Lock();
                if (TryGetLoadByPalletID(palletID, out Load load, out _))
                {
                    load.RollbackPick(palletID);
                }
            }
            finally
            {
                Unlock();
            }
        }

    }
}
