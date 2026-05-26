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
    internal class SlugManager
    {
        private SlugA _slugA;
        private SlugB _slugB;

        internal SlugManager(SlugA slugA, SlugB slugB)
        {
            XArgumentChecker.ThrowIfNull(slugA, nameof(slugA));
            XArgumentChecker.ThrowIfNull(slugB, nameof(slugB));

            _slugA = slugA;
            _slugB = slugB;
        }

        internal void Lock()
        {
            _slugA.Lock();
            _slugB.Lock();
        }

        internal void Unlock()
        {
            _slugB.Unlock();
            _slugA.Unlock();
        }

        public bool GetTargetSlugForBroadcastRelease(
            SystemSettingsItem systemSettingsItem,
            out Slug targetSlug)
        {
            Lock();
            try
            {
                targetSlug = null;
                bool slugAReady = _slugA.Cleared && systemSettingsItem.SlugAEnabled;
                bool slugBReady = _slugB.Cleared && systemSettingsItem.SlugBEnabled;

                if (_slugA.HasOpenLoad)
                {
                    if (!systemSettingsItem.SlugAEnabled)
                    {
                        return false;
                    }
                    targetSlug = _slugA;
                }
                else if (_slugB.HasOpenLoad)
                {
                    if (!systemSettingsItem.SlugBEnabled)
                    {
                        return false;
                    }
                    targetSlug = _slugB;
                }
                else if (slugAReady && slugBReady)
                {
                    targetSlug = systemSettingsItem.PreferredSlug == SlugLetter.A
                        ? _slugA
                        : (Slug)_slugB;
                }
                else if (slugAReady)
                {
                    targetSlug = _slugA;
                }
                else if (slugBReady)
                {
                    targetSlug = _slugB;
                }
                return targetSlug != null;
            }
            finally
            {
                Unlock();
            }
        }

        internal bool TryGetSlugByPalletID(
            string palletID,
            out Slug slug,
            out LoadItem loadItem)
        {
            try
            {
                Lock();
                if (_slugA.TryFindByPalletID(palletID, out loadItem))
                {
                    slug = _slugA;
                    return true;
                }
                else if (_slugB.TryFindByPalletID(palletID, out loadItem))
                {
                    slug = _slugB;
                    return true;
                }
                slug = null;
                loadItem = null;
                return false;
            }
            finally
            {
                Unlock();
            }
        }

        internal bool TryGetSlugByLetter(SlugLetter letter, out Slug slug)
        {
            slug = null;
            if (letter == SlugLetter.None)
            {
                return false;
            }
            if (letter == SlugLetter.A)
            {
                slug = _slugA;
            }
            else if (letter == SlugLetter.B)
            {
                slug = _slugB;
            }
            return slug != null;
        }

        internal bool TryGetPrimarySlug(
            SystemSettingsItem systemSettingsItem,
            out Slug primarySlug,
            out Slug secondarySlug)
        {
            if (!TryGetPrimarySlug(systemSettingsItem, out primarySlug))
            {
                secondarySlug = null;
                return false;
            }
            secondarySlug = GetOtherSlug(primarySlug);
            return true;
        }

        internal bool TryGetPrimarySlug(
            SystemSettingsItem systemSettingsItem,
            out Slug primarySlug)
        {
            Lock();
            try
            {
                SlugPickPriority slugPickPriority = systemSettingsItem.SlugPickPriority;
                if (slugPickPriority == SlugPickPriority.Balanced)
                {
                    return _GetBalancedPrimarySlug(out primarySlug);
                }
                bool slugAPicksToDo = _slugA.Any(l => l.Status == LoadItemStatus.Pickable)
                    && (slugPickPriority == SlugPickPriority.SmallerLoadNumber
                        || slugPickPriority == SlugPickPriority.SlugA
                        || slugPickPriority == SlugPickPriority.SlugAOnly);
                bool slugBPicksToDo = _slugB.Any(l => l.Status == LoadItemStatus.Pickable)
                    && (slugPickPriority == SlugPickPriority.SmallerLoadNumber
                        || slugPickPriority == SlugPickPriority.SlugB
                        || slugPickPriority == SlugPickPriority.SlugBOnly);

                primarySlug = null;
                if (!slugAPicksToDo && !slugBPicksToDo)
                {
                    primarySlug = null;
                }
                else if (slugAPicksToDo && slugBPicksToDo)
                {
                    if (slugPickPriority == SlugPickPriority.SlugA
                        || slugPickPriority == SlugPickPriority.SlugAOnly)
                    {
                        primarySlug = _slugA;
                    }
                    else if (slugPickPriority == SlugPickPriority.SlugB
                        || slugPickPriority == SlugPickPriority.SlugBOnly)
                    {
                        primarySlug = _slugB;
                    }
                    else if (slugPickPriority == SlugPickPriority.SmallerLoadNumber)
                    {
                        int loadIDA = systemSettingsItem.SlugALoadNumber;
                        int loadIDB = systemSettingsItem.SlugBLoadNumber;
                        primarySlug = loadIDA < loadIDB ? _slugA : (Slug)_slugB;
                    }
                }
                else if (slugAPicksToDo
                    && !slugBPicksToDo
                    && slugPickPriority != SlugPickPriority.SlugBOnly)
                {
                    primarySlug = _slugA;
                }
                else if (!slugAPicksToDo
                    && slugPickPriority != SlugPickPriority.SlugAOnly
                    && slugBPicksToDo)
                {
                    primarySlug = _slugB;
                }
                return primarySlug != null;
            }
            finally
            {
                Unlock();
            }
        }

        internal Slug GetOtherSlug(Slug slug) => slug == _slugA ? _slugB : (Slug)_slugA;

        private bool _GetBalancedPrimarySlug(out Slug primarySlug)
        {
            bool excludeSlugA = _slugA.AllPickingOrGreater;
            bool excludeSlugB = _slugB.AllPickingOrGreater;

            if (excludeSlugA && excludeSlugB)
            {
                primarySlug = null;
            }
            else if (excludeSlugA)
            {
                primarySlug = _slugB;
            }
            else if (excludeSlugB)
            {
                primarySlug = _slugA;
            }
            else
            {
                int workToDoA = _slugA.WorkToDoCount;
                int workToDoB = _slugB.WorkToDoCount;
                primarySlug = workToDoA < workToDoB ? _slugB : (Slug)_slugA;
            }
            return primarySlug != null;
        }

        internal void SetNextPickable(
            SystemSettingsItem systemSettingsItem,
            Slug thisSlug,
            Levels level)
        {
//             Lock();
//             try
//             {
//                 Load otherLoad;
//                 bool isPrimaryLoad = true;
//                 if (TryGetPrimaryLoad(
//                     systemSettingsItem,
//                     out Load primaryLoad,
//                     out Load secondaryLoad))
//                 {
//                     isPrimaryLoad = primaryLoad.LoadLetter == thisLoad.LoadLetter;
//                     otherLoad = isPrimaryLoad ? secondaryLoad : primaryLoad;
//                 }
//                 else
//                 {
//                     otherLoad = GetOtherLoad(thisLoad);
//                 }
//                 int activeLaneCount = thisLoad.GetActiveLanes(level);
//                 int activePalletCount = thisLoad.GetActivePallets(level);
//                 int otherActivePalletCount = otherLoad.GetActivePallets(level);
//                 int totalActivePalletCount = activePalletCount + otherActivePalletCount;
// 
//                 if (totalActivePalletCount >= Constant.MaxActivePalletsPerLevel + Constant.MaxPalletsPerRecirc)
//                 {
//                     // Nothing can be done
//                     return false;
//                 }
//                 else if (activePalletCount >= Constant.MaxActivePalletsPerLoadLevel + Constant.MaxPalletsPerRecirc)
//                 {
//                     // Nothing can be done
//                     return false;
//                 }
// 
// 
// 
// 
//             }
//             finally
//             {
//                 Unlock();
//             }
// 
//             if (isPrimaryLoad)
//             {
// 
//             }
//             else
//             {
// 
//             }
// 
// 
//             int eligibleLaneCount;
//             for (int index = 0; index < upperLevelIndexes.Length; index++)
//             {
//                 int loadIndex = upperLevelIndexes[index];
//                 LoadItemStatus status = load[loadIndex].Status;
//                 while (status >= LoadItemStatus.Pickable)
//                 {
//                     loadIndex = Constant.NextInLaneLoadIndex[loadIndex];
// 
//                 }
//             }
        }

//         private int GetActiveCounts(
//             Load load,
//             Levels level,
//             out)
//         {
// 
//         }


        internal void GetPickableLoadItems(
            Slug primarySlug,
            Slug secondarySlug,
            out List<LoadItem> primaryPickableItems,
            out List<LoadItem> secondaryPickableItems)
        {
            Lock();
            try
            {
                primaryPickableItems = primarySlug
                    .GetLoadInPickSearchOrder()
                    .Where(l => l.Status == LoadItemStatus.Pickable)
                    .ToList();
                secondaryPickableItems = secondarySlug
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
                if (TryGetSlugByPalletID(palletID, out Slug slug, out _))
                {
                    slug.RollbackPick(palletID);
                }
            }
            finally
            {
                Unlock();
            }
        }

    }
}
