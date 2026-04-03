using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DevExpress.Charts.Native;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LA = Mss.Data.LoadArchive;

namespace Mss.Data
{
    public class DataLayer : XDisposable
    {
        private Storage _storage;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private HoldCodes _holdCodes;
        private LowerRecirc _lowerRecirc;
        private UpperRecirc _upperRecirc;
        private SlugA _slugA;
        private SlugB _slugB;

        private SlugManager _slugManager;

        public static DataLayer Factory(
            out Storage storage,
            out LowerPit lowerPit,
            out UpperPit upperPit,
            out SystemSettings systemSettings,
            out Broadcast broadcast,
            out HoldCodes holdCodes,
            out LowerRecirc lowerRecirc,
            out UpperRecirc upperRecirc,
            out SlugA slugA,
            out SlugB slugB)
        {
            DataLayer dataLayer = new DataLayer();
            return dataLayer._Initialize(
                out storage,
                out lowerPit,
                out upperPit,
                out systemSettings,
                out broadcast,
                out holdCodes,
                out lowerRecirc,
                out upperRecirc,
                out slugA,
                out slugB);
        }

        private DataLayer _Initialize(
            out Storage storage,
            out LowerPit lowerPit,
            out UpperPit upperPit,
            out SystemSettings systemSettings,
            out Broadcast broadcast,
            out HoldCodes holdCodes,
            out LowerRecirc lowerRecirc,
            out UpperRecirc upperRecirc,
            out SlugA slugA,
            out SlugB slugB)
        {
            _ = XSharedCollection.Open(Constant.StorageName, out _storage);
            _ = XSharedCollection.Open(Constant.LowerPitName, out _lowerPit);
            _ = XSharedCollection.Open(Constant.UpperPitName, out _upperPit);
            _ = XSharedCollection.Open(Constant.SystemSettingsName, out _systemSettings);
            _ = XSharedCollection.Open(Constant.BroadcastName, out _broadcast);
            _ = XSharedCollection.Open(Constant.HoldCodesName, out _holdCodes);
            _ = XSharedCollection.Open(Constant.LowerRecircName, out _lowerRecirc);
            _ = XSharedCollection.Open(Constant.UpperRecircName, out _upperRecirc);
            _ = XSharedCollection.Open(Constant.SlugAName, out _slugA);
            _ = XSharedCollection.Open(Constant.SlugBName, out _slugB);
//             _ = XSharedCollection.Open(Constant.Name, out _);

            storage = _storage;
            lowerPit = _lowerPit;
            upperPit = _upperPit;
            systemSettings = _systemSettings;
            broadcast = _broadcast;
            holdCodes = _holdCodes;
            lowerRecirc = _lowerRecirc;
            upperRecirc = _upperRecirc;
            slugA = _slugA;
            slugB = _slugB;
            _slugManager = new SlugManager(_slugA, _slugB);

            return this;
        }

        protected override void DoDispose()
        {
            if (_storage != null)
            {
                _storage.Dispose();
                _storage = null;
            }
            if (_systemSettings != null)
            {
                _systemSettings.Dispose();
                _systemSettings = null;
            }
            if (_lowerPit != null)
            {
                _lowerPit.Dispose();
                _lowerPit = null;
            }
            if (_upperPit != null)
            {
                _upperPit.Dispose();
                _upperPit = null;
            }
            if (_broadcast != null)
            {
                _broadcast.Dispose();
                _broadcast = null;
            }
            if (_holdCodes != null)
            {
                _holdCodes.Dispose();
                _holdCodes = null;
            }
            if (_lowerRecirc != null)
            {
                _lowerRecirc.Dispose();
                _lowerRecirc = null;
            }
            if (_upperRecirc != null)
            {
                _upperRecirc.Dispose();
                _upperRecirc = null;
            }
            if (_slugA != null)
            {
                _slugA.Dispose();
                _slugA = null;
            }
            if (_slugB != null)
            {
                _slugB.Dispose();
                _slugB = null;
            }
        }

        private void _LockAll()
        {
            _ = _storage.Lock();
            _ = _lowerPit.Lock();
            _ = _upperPit.Lock();
            _ = _systemSettings.Lock();
            _ = _broadcast.Lock();
            _ = _holdCodes.Lock();
            _ = _lowerRecirc.Lock();
            _ = _upperRecirc.Lock();
            _slugManager.Lock();
        }

        private void _UnlockAll()
        {
            _slugManager.Unlock();
            _upperRecirc.Unlock();
            _lowerRecirc.Unlock();
            _holdCodes.Unlock();
            _broadcast.Unlock();
            _systemSettings.Unlock();
            _upperPit.Unlock();
            _lowerPit.Unlock();
            _storage.Unlock();
        }

        #region General

        public void ReceiveBroadcast(List<BroadcastItem> broadcastItems)
        {
            _LockAll();
            int largestRotationReceived = _systemSettings.LargestRotationReceived;
            bool touch = false;
            try
            {
                if (broadcastItems.Count() == 0)
                {
                    return;
                }
                _broadcast.PurgeOldBroadcast();
                foreach (BroadcastItem broadcastItem in broadcastItems)
                {
                    _ = _broadcast.Update(broadcastItem.Csn, broadcastItem, false);
                    if (broadcastItem.Rotation > largestRotationReceived)
                    {
                        largestRotationReceived = broadcastItem.Rotation;
                    }
                    touch = true;
                }
            }
            finally
            {
                if (touch)
                {
                    _systemSettings.LargestRotationReceived = largestRotationReceived;
                    _broadcast.Touch();
                }
                _UnlockAll();
            }
        }

        public void ReceiveHoldCodes (List<HoldCodeItem> holdCodes)
        {
            _LockAll();
            bool touch = false;
            bool setting = false;
            try
            {
                setting = _holdCodes.InhibitChangeNotifications;
                if (holdCodes.Count() == 0)
                {
                    return;
                }
                touch = true;
                _holdCodes.RemoveAll();
                foreach (HoldCodeItem holdCode in holdCodes)
                {
                    _holdCodes[holdCode.HoldCode] = holdCode;
                }
            }
            finally
            {
                _holdCodes.InhibitChangeNotifications = setting;
                if (touch)
                {
                    _holdCodes.Touch();
                }
                _UnlockAll();
            }
        }

        #endregion

        #region Upper and Lower Pit

        public bool TryGetPitItem(
            Levels level,
            string palletID,
            out PitItem pitItem)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof (level),
                new Levels[] { Levels.Lower, Levels.Upper });

            return (level == Levels.Upper)
                ? _upperPit.TryGetItem(palletID, out pitItem)
                : _lowerPit.TryGetItem(palletID, out pitItem);
        }

        public void RemovePitPallet(string palletID)
        {
            _LockAll();
            try
            {
                _ = _lowerPit.Remove(palletID);
                _ = _upperPit.Remove(palletID);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void SetPitPallet(
            Levels level,
            PalletItem palletItem,
            PitCode pitCode)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof(level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _LockAll();
            try
            {
                Pit pit = level == Levels.Upper
                    ? (Pit)_upperPit
                    : (Pit)_lowerPit;
                pit.Set(palletItem, level, pitCode);
            }
            finally
            {
                _UnlockAll();
            }
        }

//         public (int Lower, int Upper)[] AssignedCraneCounts
//         {
//             get
//             {
//                 _LockAll();
//                 try
//                 {
//                     (int, int)[] assignedCounts = new (int, int)[Constant.MaxCranes + 1];
//                     int[] lowerCounts = _lowerPit.AssignedCraneCounts;
//                     int[] upperCounts = _upperPit.AssignedCraneCounts;
//                     for (int index = 0; index < assignedCounts.Length; index++)
//                     {
//                         assignedCounts[index] = (lowerCounts[index], upperCounts[index]);
//                     }
//                     return assignedCounts;
//                 }
//                 finally
//                 {
//                     _UnlockAll();
//                 }
//             }
// 
//         }

        public int[] GetAssignedCraneCounts(Levels level) 
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof(level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _LockAll();
            try
            {
                return (level == Levels.Lower)
                    ? _lowerPit.AssignedCraneCounts
                    : _upperPit.AssignedCraneCounts;
            }
            finally
            {
                _UnlockAll();
            }
        }

        #endregion

        #region Storage
        #endregion

        #region System Settings
        #endregion

        #region Broadcast
        #endregion

        #region Load Manager

        private bool _MatchLoadItemStatus(
            Slug load,
            int loadIndex,
            LoadItemStatus statusesToCompare)
        {
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfLessThan(0, loadIndex, "loadIndex");

            return statusesToCompare.IsFlagSet(load[loadIndex].Status);
        }

        private bool _MatchPreviousLoadItemStatus(
            bool palletArrival,
            Slug slug,
            LoadItem loadItem,
            LoadItemStatus targetStatus,
            LoadItemStatus statusesToCompare,
            out string fault)
        {
            int previousIndexInLane = Constant.PreviousInLaneLoadIndex[loadItem.NodeIndex];
            if (previousIndexInLane == Constant.BeforeFirst
                || _MatchLoadItemStatus(
                    slug,
                    previousIndexInLane,
                    statusesToCompare))
            {
                fault = string.Empty;
                return true;
            }
            fault = palletArrival
                ? $"({loadItem.Coordinates}) Attempted to set a Load position to {targetStatus.ToText()} when the previous in lane was not set to one of the following: {statusesToCompare}."
                : $"({loadItem.Coordinates}) Attempted to set a Load position to {targetStatus.ToText()} when the previous in lane was not set to one of the following: {statusesToCompare}.";
            XSystemEvent.Publish(
                slug.CollectionConfiguration.Name,
                XSystemEventLevel.Error,
                fault + " Operation faulted.");
            return false;
        }

        public bool TrySetPicked(
            string palletID,
            out string fault)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either slug when attempting to set status to {LoadItemStatus.Picked}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Picking)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Slug position to {LoadItemStatus.Picked.ToText()} when it was not set to {LoadItemStatus.Picking.ToText()}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                if (!_MatchPreviousLoadItemStatus(
                    false,
                    slug,
                    loadItem,
                    LoadItemStatus.Picked,
                    LoadItemStatus.Presequenced | LoadItemStatus.Sequenced | LoadItemStatus.Done,
                    out fault))
                {
                    return false;
                }
                loadItem.Status = LoadItemStatus.Picked;
                loadItem.Shortage = false;
                slug[loadItem.NodeIndex] = loadItem;

                _slugManager.SetNextPickable(
                     _systemSettings.GetItem(),
                     slug,
                     loadItem.SlugLevel);
                fault = string.Empty;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TrySetPresequenced(
            string palletID,
            out string fault)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either slug when attempting to set status to {LoadItemStatus.Presequenced}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Picked)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Slug position to {LoadItemStatus.Presequenced.ToText()} when it was not set to {LoadItemStatus.Picked.ToText()}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                if (!_MatchPreviousLoadItemStatus(
                    false,
                    slug,
                    loadItem,
                    LoadItemStatus.Presequenced,
                    LoadItemStatus.Presequenced | LoadItemStatus.Sequenced | LoadItemStatus.Done,
                    out fault))
                {
                    return false;
                }
                loadItem.Status = LoadItemStatus.Presequenced;
                loadItem.Shortage = false;
                slug[loadItem.NodeIndex] = loadItem;

               _slugManager.SetNextPickable(
                    _systemSettings.GetItem(),
                    slug,
                    loadItem.SlugLevel);
                fault = string.Empty;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TrySetSequenced(
            string palletID,
            out string fault)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either slug when attempting to set status to {LoadItemStatus.Sequenced}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Presequenced)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Slug position to {LoadItemStatus.Sequenced.ToText()} when it was not set to {LoadItemStatus.Presequenced.ToText()}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                if (!_MatchPreviousLoadItemStatus(
                    false,
                    slug,
                    loadItem,
                    LoadItemStatus.Sequenced,
                    LoadItemStatus.Sequenced | LoadItemStatus.Done,
                    out fault))
                {
                    return false;
                }
                loadItem.Status = LoadItemStatus.Sequenced;
                loadItem.Shortage = false;
                slug[loadItem.NodeIndex] = loadItem;

               _slugManager.SetNextPickable(
                    _systemSettings.GetItem(),
                    slug,
                    loadItem.SlugLevel);
                fault = string.Empty;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TrySetDone(
            string palletID,
            out string fault)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either slug when attempting to set status to {LoadItemStatus.Done}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Sequenced)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Slug position to {LoadItemStatus.Done.ToText()} when it was not set to {LoadItemStatus.Sequenced.ToText()}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                int previousIndexInLane = Constant.PreviousInLaneLoadIndex[loadItem.NodeIndex];
                if (previousIndexInLane != Constant.BeforeFirst
                    && slug[previousIndexInLane].Status != LoadItemStatus.Done)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Load position to {LoadItemStatus.Done.ToText()} when the previous in lane was not set to {LoadItemStatus.Done.ToText()}.";
                    XSystemEvent.Publish(
                        slug.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                loadItem.Status = LoadItemStatus.Done;
                loadItem.Shortage = false;
                slug[loadItem.NodeIndex] = loadItem;

               _slugManager.SetNextPickable(
                    _systemSettings.GetItem(),
                    slug,
                    loadItem.SlugLevel);
                fault = string.Empty;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
//         private bool _SetNextPickable(Load load, Levels level)
//         {
//             bool isPrimaryLoad = true;
//             if (_loadManager.TryGetPrimaryLoad(
//                 _systemSettings.GetItem(),
//                 out Load primaryLoad))
//             {
//                 isPrimaryLoad = primaryLoad.LoadLetter == load.LoadLetter;
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
// 
// 
// 
// 
// 
// 
// 
// 
// 
//             LoadItemStatus allocatedStatuses = LoadItemStatus.Pickable
//                 | LoadItemStatus.Picking
//                 | LoadItemStatus.Picked
//                 | LoadItemStatus.Presequenced;
// 
//             int loadCount = load
//                 .Count(l => l.LoadLevel == level && allocatedStatuses.IsFlagSet(l.Status));
// 
//             Load otherLoad = _loadManager.GetOtherLoad(load);
//             int otherLoadCount = otherLoad
//                 .Count(l => l.LoadLevel == level && allocatedStatuses.IsFlagSet(l.Status));
// 
// 
// 
//             LoadItem loadItem = load
//                 .GetLoadInPickSearchOrder()
//                 .Where(l => l.LoadLevel == level)
// 
//         }


        #endregion

        #region Assignment
        #endregion

        #region Inbound Router

        public bool ProcessPalletAtRouter(
            OperationCode operationCode,
            CraneNumber craneNumber,
            Levels level,
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;

            if (!MesInterface.TryFetchPalletItem(
                operationCode,
                palletID,
                true,
                out PalletDestination destination,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                palletItem = null;
                return false;
            }

            _LockAll();
            try
            {
                if (TryGetPitItem(level, palletID, out PitItem pitItem))
                {
                    palletItem = pitItem.Pallet;
                    PitCode pitCode = pitItem.PitCode;
                    if (pitCode.IsAssigned())
                    {
                        CraneNumber assignedCrane = pitCode.AssignedCrane();
                        if (palletItem.Status == PalletStatus.OK
                            && !palletItem.IsStack
                            && _TryAssignHotJobAtRouter(
                                craneNumber,
                                level,
                                palletItem,
                                out LoadItem loadItem))
                        {
                            moveCommand = Constant.IRMoveCommandForward;
                            extendedState = $"({Constant.PalletTypeHotJob})  Assigned Hot Job Pallet {palletID} to Slug {loadItem.SlugLetter}";
                            RemovePitPallet(palletID);
                            return true;
                        }
                        if (assignedCrane == craneNumber
                            || craneNumber == CraneNumber.Crane4)
                        {
                            if (assignedCrane != craneNumber)
                            {
                                SetPitPallet(level, palletItem, PitCode.Assigned4);
                            }
                            moveCommand = Constant.IRMoveCommandToCrane;
                            extendedState = $"({Constant.PalletTypeStore})  Diverting Pallet {palletItem.PalletID} to Crane {(int)craneNumber}";
                            return true;
                        }
                        moveCommand = Constant.IRMoveCommandForward;
                        extendedState = $"({Constant.PalletTypeStore})  Moving Pallet {palletItem.PalletID} forward to Crane {(int)assignedCrane}";
                        return true;
                    }
                    else
                    {
                        string palletType = "???";
                        if (pitCode == PitCode.Unknown
                            || pitCode == PitCode.Purge)
                        {
                            palletItem.Status = PalletStatus.Purge;
                            SetPitPallet(level, palletItem, PitCode.Purge);
                            palletType = Constant.PalletTypePurge;
                        }
                        else if (pitCode == PitCode.Stack)
                        {
                            palletType = Constant.PalletTypeStack;
                        }
                        moveCommand = Constant.IRMoveCommandForward;
                        extendedState = $"({palletType})  Moving Pallet {palletItem.PalletID} forward";
                        return true;
                    }
                }
                else
                {
                    if (_slugManager.TryGetSlugByPalletID(
                        palletID,
                        out Slug slug,
                        out LoadItem loadItem))
                    {
                        if (loadItem.SlugLevel == level)
                        {
                            if (loadItem.Status != LoadItemStatus.Picked)
                            {
                                palletItem = loadItem.Pallet;
                                string error = $"Load Pallet {palletID} arrived at {level} Router {(int)craneNumber} with a Load Item Status of {loadItem.Status.ToText()}. It should be {LoadItemStatus.Picked.ToText()}. Operation faulted.";
                                XSystemEvent.Publish(
                                    $"{level} Router {(int)craneNumber}",
                                    XSystemEventLevel.Error,
                                    error);
                                fault = $"Load Pallet {palletID} arrived with a Load Item Status of {loadItem.Status.ToText()}. It should be {LoadItemStatus.Picked.ToText()}.";
                                return false;
                            }
                            palletItem = loadItem.Pallet;
                            moveCommand = Constant.IRMoveCommandForward;
                            extendedState = $"({Constant.PalletTypeLoad})  Moving Pallet {palletID} forward";
                            return true;
                        }
                        slug.RollbackPick(palletID);
                    }
                    palletItem = fetchedPalletItem;
                    if (palletItem.Status == PalletStatus.OK
                        || palletItem.Status == PalletStatus.Hold
                        || palletItem.Status == PalletStatus.Reserved)
                    {
                        if (!palletItem.IsStack
                            && palletItem.Status == PalletStatus.OK
                            && _TryAssignHotJobAtRouter(
                                craneNumber,
                                level,
                                palletItem,
                                out loadItem))
                        {
                            moveCommand = Constant.IRMoveCommandForward;
                            extendedState = $"({Constant.PalletTypeHotJob})  Assigned Hot Job Pallet {palletID} to Slug {loadItem.SlugLetter}";
                            return true;
                        }
                        return _AssignPalletToCraneAtRouter(
                            craneNumber,
                            level,
                            palletItem,
                            out moveCommand,
                            out extendedState,
                            out fault);
                    }
                    else
                    {
                        string palletType = "???";
                        PalletStatus palletStatus = palletItem.Status;
                        if (palletStatus == PalletStatus.Purge
                            || palletStatus == PalletStatus.Unknown
                            || palletStatus == PalletStatus.Invalid)
                        {
                            palletItem.Status = PalletStatus.Purge;
                            SetPitPallet(level, palletItem, PitCode.Purge);
                            palletType = Constant.PalletTypePurge;
                        }
//!!! USE DECOSTAR AS EXAMPLE TO RECOVER STACK PICK
//                         else if (palletItem.Status == PalletStatus.Stack)
//                         {
//                             palletType = Constant.PalletTypeStack;
//                         }
                        moveCommand = Constant.IRMoveCommandForward;
                        extendedState = $"({palletType})  Moving Pallet {palletItem.PalletID} forward";
                        return true;
                    }
                }
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _TryAssignHotJobAtRouter(
            CraneNumber craneNumber,
            Levels level,
            PalletItem palletItem,
            out LoadItem loadItem)
        {
            _LockAll();
            try
            {
                loadItem = null;
                return !_storage.AnyPickable(
                        CraneNumber.None,
                        palletItem.Sku)
                    && _TryAssignHotJob(
                        false,
                        level,
                        craneNumber,
                        palletItem,
                        out loadItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _TryAssignHotJob(
            bool calledByCrane,
            Levels levels,
            CraneNumber craneNumber,
            PalletItem palletItem,
            out LoadItem loadItem)
        {
            if (!_slugManager.TryGetPrimarySlug(
                _systemSettings.GetItem(),
                out Slug primarySlug,
                out Slug secondarySlug))
            {
                loadItem = null;
                return false;
            }
            loadItem = null;
            Slug currentSlug = null;
            if (_CanPickToSlug(primarySlug.SlugLetter))
            {
                currentSlug = primarySlug;
                loadItem = currentSlug
                    .GetLoadInPickSearchOrder()
                    .Where(l => levels.IsFlagSet(l.SlugLevel))
                    .FirstOrDefault(l =>
                        l.Status == LoadItemStatus.Pickable
                        && l.Broadcast.Sku == palletItem.Sku);
            }
            if (loadItem == null
                && _CanPickToSlug(secondarySlug.SlugLetter))
            {
                currentSlug = secondarySlug;
                loadItem = currentSlug
                    .GetLoadInPickSearchOrder()
                    .Where(l => levels.IsFlagSet(l.SlugLevel))
                    .FirstOrDefault(l =>
                        l.Status == LoadItemStatus.Pickable
                        && l.Broadcast.Sku == palletItem.Sku);
            }
            if (loadItem != null)
            {
                loadItem.Status = calledByCrane
                    ? LoadItemStatus.Picking
                    : LoadItemStatus.Picked;
                loadItem.Pallet = palletItem;
                loadItem.Crane = craneNumber;
                currentSlug[loadItem.NodeIndex] = loadItem;
                _ = _upperPit.Remove(palletItem.PalletID);
                _ = _lowerPit.Remove(palletItem.PalletID);
            }
            return loadItem != null;
        }

        // Must be called from within _LockAll()
        private bool _AssignPalletToCraneAtRouter(
            CraneNumber craneNumber,
            Levels level,
            PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            IEnumerable<AssignmentCandidate> candidates = _GetAssignmentCandidates(level, palletItem.Sku);
            foreach (AssignmentCandidate candidate in candidates)
            {
                CraneNumber targetCrane = candidate.TargetCrane;

                if (targetCrane >= craneNumber
                    && candidate.AvailableBins > 0
                    && candidate.CanAcceptPallet
                    && candidate.AssignedCounts < Constant.CraneInboundBufferSize)
                {
                    moveCommand = targetCrane == craneNumber
                        ? Constant.IRMoveCommandToCrane
                        : Constant.IRMoveCommandForward;
                    extendedState = moveCommand == Constant.IRMoveCommandToCrane
                        ? $"({Constant.PalletTypeStore})  Diverting Pallet {palletItem.PalletID} to Crane {(int)targetCrane}"
                        : $"({Constant.PalletTypeStore})  Moving Pallet {palletItem.PalletID} forward to Crane {(int)targetCrane}";
                    fault = string.Empty;
                    SetPitPallet(level, palletItem, targetCrane.AssignedPitCode());
                    return true;
                }
            }
            moveCommand = Constant.NoMoveCommand;
            extendedState = $"There is currently no destination available for Pallet {palletItem.PalletID}";
            fault = string.Empty;
            return false;
        }

        private class AssignmentCandidate
        {
            public CraneNumber TargetCrane { get; set; }
            public int SkuCount { get; set; }
            public bool CanAcceptPallet { get; set; }
            public int AvailableBins { get; set; }
            public int AssignedCounts { get; set; }
        }

        // Must be called from within _LockAll()
        private IEnumerable<AssignmentCandidate> _GetAssignmentCandidates(
            Levels level,
            string sku)
        {
            AssignmentCandidate[] candidates = new AssignmentCandidate[Constant.MaxCranes];
            int[] assignedCraneCounts = GetAssignedCraneCounts(level); // accessed by crane index
            (CraneNumber craneNumber, int skuCount)[] scs = _storage.GetPrioritizedSkuCountPerCrane(sku);
            foreach ((CraneNumber craneNumber, int skuCount) in scs)
            {
                if (craneNumber > CraneNumber.None)
                {
                    int emptyBins = _storage.GetEmptyBinCount(craneNumber);
                    candidates[craneNumber.Index() - 1] = new AssignmentCandidate
                    {
                        TargetCrane = craneNumber,
                        CanAcceptPallet = level == Levels.Lower
                            ? _systemSettings.CanRouteToLowerInbound(craneNumber)
                            : _systemSettings.CanRouteToUpperInbound(craneNumber),
                        SkuCount = skuCount,
                        AvailableBins = emptyBins - assignedCraneCounts[craneNumber.Index()],
                        AssignedCounts = assignedCraneCounts[craneNumber.Index()]
                    };
                }
            }
            return candidates.OrderBy(c => c.SkuCount);
        }

        #endregion

        //==================================================================================

        #region Crane

        public void FlagAsDuplicate(string palletID)
        {
            if (!palletID.ValidPalletID())
            {
                return;
            }
            _LockAll();
            try
            {
                IEnumerable<BinItem> duplicates = _storage
                    .Where(b => b.Pallet.PalletID == palletID);
                if (duplicates.Count() > 1)
                {
                    foreach (BinItem binItem in duplicates)
                    {
                        binItem.BinStatus = BinStatus.OfflineDuplicate;
                        _storage[binItem.NodeIndex] = binItem;
                    }
                }
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void CleanUpAllocatedBins(CraneNumber craneNumber)
        {
            _storage.CleanUpAllocatedBins(craneNumber);
        }

        public void ClearStorageBinByPalletID(string palletID)
        {
            ClearStorageBinByPalletID(palletID, false);
        }

        public void ClearStorageBinByPalletID(string palletID, bool audit)
        {
            _storage.ClearBinByPalletID(palletID, audit);
        }

        public void CompleteStorageGetByLocation(int location)
        {
            _LockAll();
            try
            {
                _storage.CompleteGetByLocation(location);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void CompleteStoragePutByLocation(int location)
        {
            _LockAll();
            try
            {
                _ = _storage.CompletePutByLocation(location);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool ProcessSemiAutoGet(
            int location,
            out PalletItem palletItem)
        {
            _LockAll();
            try
            {
                BinItem binItem = _storage.FirstOrDefault(b => b.Location == location);
                if (binItem == null)
                {
                    palletItem = null;
                    return false;
                }
                palletItem = binItem.Pallet;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void DoSemiAutoPutByLocation(PalletItem palletItem, int location)
        {
            _LockAll();
            try
            {
                BinItem binItem = _storage[BinItem.LocationToNodeIndex(location)];
                binItem.BinStatus = BinStatus.Pickable;
                binItem.StoredOn = DateTime.Now;
                binItem.Pallet = palletItem;
                binItem.Pallet = palletItem;
                _storage[binItem.NodeIndex] = binItem;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool CanDoStore(CraneNumber craneNumber)
        {
            _LockAll();
            try
            {
                return _systemSettings.CanDoStore(craneNumber)
                    && _storage.IsStorableBinAvailable(craneNumber);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void SetStorageLocationToDisabled(
            CraneNumber craneNumber,
            int location)
        {
            _LockAll();
            try
            {
                int nodeIndex = BinItem.LocationToNodeIndex(location);
                BinItem binItem = _storage[nodeIndex];
//                 binItem.Pallet = new PalletItem(); //???
                binItem.BinStatus = BinStatus.Offline;
                binItem.Disabled = true;
                _storage[nodeIndex] = binItem;
            }
            finally
            {
                _UnlockAll();
            }
            XSystemEvent.Publish(
                craneNumber.ToText(),
                XSystemEventLevel.Error,
                $"Crane {(int)craneNumber} received 'Invalid Location Error' for Location {location}. Bin was marked as Offline/Disabled.");
        }

        public bool TryAllocateStoragePut(
            CraneNumber craneNumber,
            PalletItem palletItem,
            out BinItem binItem)
        {
            _LockAll();
            try
            {
                return _storage.TryAllocatePut(
                    craneNumber,
                    palletItem,
                    out binItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryAllocateStorageStackPut(
            CraneNumber craneNumber,
            PalletItem palletItem,
            out BinItem binItem)
        {
            _LockAll();
            try
            {
                return _storage.TryAllocateStackPut(
                    craneNumber,
                    palletItem,
                    out binItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryAuditPick(
            CraneNumber craneNumber,
            out PalletItem palletItem,
            out int getCommand,
            out string extendedState)
        {
            _LockAll();
            try
            {
                palletItem = null;
                extendedState = string.Empty;
                getCommand = Constant.NoCraneCommand;
                if (!_systemSettings.CanDoAuditPick(craneNumber))
                {
                    return false;
                }
                if (!_storage.TryAllocateAuditPick(craneNumber, out BinItem binItem))
                {
                    return false;
                }
                palletItem = binItem.Pallet;
                getCommand = binItem.Location;
                extendedState = $"({Constant.PalletTypeAudit})  Getting Pallet {palletItem.PalletID} from {getCommand}";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryPurgePick(
            CraneNumber craneNumber,
            bool lowerOutboundClear,
            bool upperOutboundClear,
            out Levels purgeLevel,
            out PalletItem palletItem,
            out int getCommand,
            out string extendedState)
        {
            _LockAll();
            try
            {
                purgeLevel = Levels.None;
                palletItem = null;
                getCommand = Constant.NoCraneCommand;
                extendedState = string.Empty;

                if (lowerOutboundClear
                    && _systemSettings.CanDoPurgePicks(craneNumber, Levels.Lower))
                {
                    purgeLevel = Levels.Lower;
                }
                else if (upperOutboundClear
                    && _systemSettings.CanDoPurgePicks(craneNumber, Levels.Upper))
                {
                    purgeLevel = Levels.Upper;
                }
                else
                {
                    return false;
                }

                if (!_storage.TryAllocatePurgePick(craneNumber, out BinItem binItem))
                {
                    purgeLevel = Levels.None;
                    return false;
                }
                palletItem = binItem.Pallet;
                getCommand = binItem.Location;
                extendedState = $"({Constant.PalletTypePurge})  Getting Pallet {palletItem.PalletID} from {getCommand}";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryStackPick(
            CraneNumber craneNumber,
            bool lowerOutboundClear,
            bool upperOutboundClear,
            out Levels stackPickLevel,
            out PalletItem palletItem,
            out int getCommand,
            out string extendedState)
        {
            _LockAll();
            try
            {
                stackPickLevel = Levels.None;
                palletItem = null;
                getCommand = Constant.NoCraneCommand;
                extendedState = string.Empty;

                if (lowerOutboundClear
                    && _systemSettings.CanDoStackPicks(craneNumber, Levels.Lower))
                {
                    stackPickLevel = Levels.Lower;
                }
                else if (upperOutboundClear
                    && _systemSettings.CanDoStackPicks(craneNumber, Levels.Upper))
                {
                    stackPickLevel = Levels.Upper;
                }
                else
                {
                    return false;
                }

                if (!_storage.TryAllocateStackPick(craneNumber, out BinItem binItem))
                {
                    stackPickLevel = Levels.None;
                    return false;
                }
                palletItem = binItem.Pallet;
                getCommand = binItem.Location;
                extendedState = $"({Constant.PalletTypeStack})  Getting Pallet {palletItem.PalletID} from {getCommand}";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryLoadPick(
            CraneNumber craneNumber,
            bool lowerOutboundClear,
            bool upperOutboundClear,
            out LoadItem loadItem,
            out int getCommand,
            out string extendedState)
        {
            _LockAll();
            try
            {
                loadItem = null;
                getCommand = Constant.NoCraneCommand;
                extendedState = string.Empty;
                if (!lowerOutboundClear && !upperOutboundClear)
                {
                    return false;
                }
                if (!_slugManager.TryGetPrimarySlug(
                    _systemSettings.GetItem(),
                    out Slug primaryLoad,
                    out Slug secondaryLoad))
                {
                    return false;
                }

                _slugManager.GetPickableLoadItems(
                    primaryLoad,
                    secondaryLoad,
                    out List<LoadItem> primaryPickableItems,
                    out List<LoadItem> secondaryPickableItems);

                if (!_TryLoadPick(
                    craneNumber,
                    lowerOutboundClear,
                    upperOutboundClear,
                    primaryPickableItems.Concat(secondaryPickableItems),
                    out loadItem,
                    out getCommand))
                {
                    return false;
                }
                extendedState = $"({Constant.PalletTypeLoad})  Getting Load Pallet {loadItem.Pallet.PalletID} from {getCommand} to Slug {loadItem.SlugLetter}.";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called within _LockAll()
        private bool _TryLoadPick(
            CraneNumber craneNumber,
            bool lowerOutboundClear,
            bool upperOutboundClear,
            IEnumerable<LoadItem> pickableItems,
            out LoadItem loadItem,
            out int getCommand)
        {
            foreach (LoadItem pickableItem in pickableItems)
            {
                if (!_systemSettings.CanDoLoadPick(craneNumber, pickableItem.SlugLevel))
                {
                    continue;
                }
                if (pickableItem.SlugLevel == Levels.Lower && !lowerOutboundClear)
                {
                    continue;
                }
                if (pickableItem.SlugLevel == Levels.Upper && !upperOutboundClear)
                {
                    continue;
                }
                if (!_slugManager.TryGetSlugByLetter(pickableItem.SlugLetter, out Slug load))
                {
                    XSystemEvent.Publish(
                        nameof(_TryLoadPick),
                        XSystemEventLevel.Error,
                        $"Unknown Slug Letter {pickableItem.SlugLetter} referenced in LoadItem with NodeIndex {pickableItem.NodeIndex}.");
                    continue;
                }
                if (!_CanPickToSlug(load.SlugLetter))
                {
                    continue;
                }
                if (_storage.TryAllocateLoadPick(
                    craneNumber,
                    _systemSettings.FifoMode,
                    pickableItem.Broadcast.Sku,
                    out BinItem binItem))
                {
                    loadItem = pickableItem;
                    loadItem.Pallet = binItem.Pallet;
                    loadItem.Status = LoadItemStatus.Picking;
                    loadItem.Crane = craneNumber;
                    load[loadItem.NodeIndex] = loadItem;

                    getCommand = binItem.Location;
                    return true;
                }
            }
            loadItem = null;
            getCommand = Constant.NoCraneCommand;
            return false;
        }

        public bool TryAssignHotJobAtCrane(
            CraneNumber craneNumber,
            bool lowerOutboundClear,
            bool upperOutboundClear,
            PalletItem palletItem,
            out LoadItem loadItem)
        {
            Levels levels = Levels.None;
            if (lowerOutboundClear)
            {
                levels |= Levels.Lower;
            }
            if (upperOutboundClear)
            {
                levels |= Levels.Upper;
            }
            if (levels == Levels.None)
            {
                loadItem = null;
                return false;
            }
            return _TryAssignHotJobAtCrane(
                craneNumber,
                levels,
                palletItem,
                out loadItem);
        }

        // Must be called from within _LockAll()
        private bool _TryAssignHotJobAtCrane(
            CraneNumber craneNumber,
            Levels level,
            PalletItem palletItem,
            out LoadItem loadItem)
        {
            _LockAll();
            try
            {
                loadItem = null;
                return _storage.AnyPickable(
                        CraneNumber.None,
                        palletItem.Sku)
                    && _TryAssignHotJob(
                        true,
                        level,
                        craneNumber,
                        palletItem,
                        out loadItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        //         private int _GetCraneHotJobPutCommand(
        //             CraneNumber craneNumber,
        //             Levels level)
        //         {
        //             int putCommand = Constant.NoCraneCommand;
        //             switch (craneNumber)
        //             {
        //                 case CraneNumber.Crane1:
        //                     putCommand = level == Levels.Upper
        //                         ? Constant.Crane1UpperOutboundLocation
        //                         : Constant.Crane1LowerOutboundLocation;
        //                     break;
        //                 case CraneNumber.Crane2:
        //                     putCommand = level == Levels.Upper
        //                         ? Constant.Crane2UpperOutboundLocation
        //                         : Constant.Crane2LowerOutboundLocation;
        //                     break;
        //                 case CraneNumber.Crane3:
        //                     putCommand = level == Levels.Upper
        //                         ? Constant.Crane3UpperOutboundLocation
        //                         : Constant.Crane3LowerOutboundLocation;
        //                     break;
        //                 case CraneNumber.Crane4:
        //                     putCommand = level == Levels.Upper
        //                         ? Constant.Crane4UpperOutboundLocation
        //                         : Constant.Crane4LowerOutboundLocation;
        //                     break;
        //             }
        //             return putCommand;
        //         }

        public bool TryGetRecoveryLoadItem(
            string palletID,
            out LoadItem loadItem)
        {
            _LockAll();
            try
            {
                loadItem = null;
                return _slugManager.TryGetSlugByPalletID(
                    palletID,
                    out _,
                    out loadItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryCleanUpPitPalletOnCrane(
            string palletID,
            out PalletItem palletItem,
            out BinItem binItem,
            out CraneFunction craneFunction)
        {
            _LockAll();
            try
            {
                if (_storage.TryFindBinByPalletID(
                    palletID,
                    out binItem))
                {
                    palletItem = binItem.Pallet;
                    if (binItem.BinStatus == BinStatus.PutAllocated)
                    {
                        craneFunction = CraneFunction.Store;
                        return true;
                    }
                    switch (palletItem.Status)
                    {
                        case PalletStatus.Purge:
                            craneFunction = CraneFunction.PurgePick;
                            return true;
                        case PalletStatus.Hold:
                            craneFunction = CraneFunction.Audit;
                            return true;
//                         case PalletStatus.Stack:
//                             craneFunction = CraneFunction.StackPick;
//                             return true;
                        default:
                            craneFunction = CraneFunction.Audit;
                            return true;
                    }
                }

                if (!_upperPit.TryGetItem(palletID, out PitItem pitItem)
                    && !_lowerPit.TryGetItem(palletID, out pitItem))
                {
                    craneFunction = CraneFunction.None;
                    palletItem = null;
                    return false;
                }
                palletItem = pitItem.Pallet;
                switch (pitItem.PitCode)
                {
                    case PitCode.Assigned1:
                    case PitCode.Assigned2:
                    case PitCode.Assigned3:
                    case PitCode.Assigned4:
                        craneFunction = CraneFunction.Store;
                        return true;
                    default:
                        craneFunction = CraneFunction.Audit;
                        return true;
                }
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _IsSlugEnabled(SlugLetter slugLetter)
        {
            return slugLetter != SlugLetter.None
                && (slugLetter == SlugLetter.A
                    ? _systemSettings.SlugAEnabled
                    : _systemSettings.SlugBEnabled);
        }

        private bool _CanPickToSlug(SlugLetter slugLetter)
        {
            if (slugLetter == SlugLetter.None)
            {
                return false;
            }
            bool onlyA = _systemSettings.SlugPickPriority == SlugPickPriority.SlugAOnly;
            bool onlyB = _systemSettings.SlugPickPriority == SlugPickPriority.SlugBOnly;
            return _IsSlugEnabled(slugLetter)
                && ((!onlyA && slugLetter == SlugLetter.B)
                    || (!onlyB && slugLetter == SlugLetter.A));
        }

        public void ClearStorageBinByLocation(int location)
        {
            ClearStorageBinByLocation(location, false);
        }

        public void ClearStorageBinByLocation(int location, bool flagBinForAudit)
        {
            _LockAll();
            try
            {
                _storage.ClearBinByLocation(location, flagBinForAudit);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void RollBackLoadPick(
            string palletID,
            bool clearBin)
        {
            _LockAll();
            try
            {
                _slugManager.RollbackPick(palletID);
                if (clearBin)
                {
                    _storage.ClearBinByPalletID(palletID, false);
                }
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryCompactStorage(CraneNumber craneNumber)
        {
            _LockAll();
            try
            {
                if (!_systemSettings.CanAutoCompactStorage(craneNumber))
                {
                    return false;
                }
                // Don't set Compact Audit if any existing Audits
                if (_storage.Any(b =>
                    b.CraneNumber == craneNumber
                    && b.Audit
                    && !b.Disabled
                    && !b.NotUsable))
                {
                    return false;
                }
                return _TryCompactStack(craneNumber)
                    || _TryCompactPallet(craneNumber);
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called within _LockAll()
        private bool _TryCompactPallet(CraneNumber craneNumber)
        {
            BinItem sourceBin = _storage
                .Reverse()
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && !b.Pallet.IsStack
                    && b.Pallet.Status == PalletStatus.OK
                    && b.BinStatus == BinStatus.Pickable
                    && !b.Audit
                    && !b.Disabled
                    && !b.NotUsable);
            if (sourceBin == null)
            {
                return false;
            }
            BinItem emptyBins = _storage
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && b.NodeIndex < sourceBin.NodeIndex
                    && b.BinStatus == BinStatus.Empty
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);
            if (emptyBins == null)
            {
                return false;
            }
            sourceBin.Audit = true;
            _storage[sourceBin.NodeIndex] = sourceBin;
            return true;
        }

        // Must be called within _LockAll()
        private bool _TryCompactStack(CraneNumber craneNumber)
        {
            BinItem sourceBin = _storage
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && b.Pallet.IsStack
                    && b.Pallet.Status == PalletStatus.OK
                    && b.BinStatus == BinStatus.Pickable
                    && !b.Audit
                    && !b.Disabled
                    && !b.NotUsable);
            if (sourceBin == null)
            {
                return false;
            }
            BinItem emptyBins = _storage
                .Reverse()
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && b.NodeIndex > sourceBin.NodeIndex
                    && b.BinStatus == BinStatus.Empty
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);
            if (emptyBins == null)
            {
                return false;
            }
            sourceBin.Audit = true;
            _storage[sourceBin.NodeIndex] = sourceBin;
            return true;
        }

        #endregion

        //==================================================================================

        #region Load Director
        #endregion

        //==================================================================================

        #region Recirc In
        #endregion

        //==================================================================================

        #region Recirc Out
        #endregion

        //==================================================================================

        #region Purge
        #endregion

        //==================================================================================

        #region  Transfer

        public bool ProcessPalletAtTransfer(
            Levels level,
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            extendedState = string.Empty;
            moveCommand = Constant.NoMoveCommand;
            fault = string.Empty;
            palletItem = null;
            _LockAll();
            try
            {

                if (!_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    if (TryGetPitItem(level, palletID, out PitItem pitItem)
                        && level == Levels.Upper
                        && pitItem.PitCode == PitCode.Stack
                        && pitItem.Pallet.IsStack)
                    {
                        palletItem = pitItem.Pallet;
                        moveCommand = Constant.TFStackMoveCommand;
                        extendedState = $"(STACK) Moving Stack {palletID} to Empty Pallet Lane.";
                        return true;
                    }
                    moveCommand = Constant.TFFinalPurgeMoveCommand;
                    extendedState = $"(UNEXPECTED) Moving unexpected Pallet {palletID} to Final Purge.";
                    return true;
                }
                if (loadItem.SlugLevel != level)
                {
                    fault = $"Pallet {palletID} on the wrong level";
                    return false;
                }
                palletItem = loadItem.Pallet;
                moveCommand = loadItem.TransferMoveCommand;
                extendedState = $"Transferring Pallet {palletID} to {slug.CollectionConfiguration.FriendlyName}, Lane {loadItem.SlugLane}";

                if (XConfiguration.TryGetAlias(Constant.TransferTelemetryEnabledName, out string setting)
                    && XConfigurationPropertyValueParser.IsTrue(setting, out _))
                {
                    _PublishTransferTelemetry(
                        level,
                        loadItem,
                        moveCommand);
                }

                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private void _PublishTransferTelemetry(
            Levels level,
            LoadItem loadItem,
            int moveCommand)
        {
            string opName = $"{level.ToText()}Transfer";
            int loadNumber = loadItem.SlugLetter == SlugLetter.A
                ? _systemSettings.SlugALoadNumber
                : _systemSettings.SlugBLoadNumber;
            string csn = loadItem.Broadcast.Csn;
            PalletItem palletItem = loadItem.Pallet;
            string palletID = palletItem.PalletID;
            string jobID = palletItem.JobID;

            XSystemEvent.Publish(
                opName,
                XSystemEventLevel.Telemetry,
                $"({loadItem.Coordinates}) LoadNumber:{loadNumber}|CSN:{csn}|PalletID:{palletID}|JobID:{jobID}|MoveCommand:{moveCommand}");
        }

        public bool IsLoadLevelDone(
            SlugLetter letter,
            Levels level)
        {
            _LockAll();
            try
            {
                return _slugManager.TryGetSlugByLetter(letter, out Slug load)
                    && load.LevelDone(level);
            }
            finally
            {
                _UnlockAll();
            }
        }

        #endregion

        //==================================================================================

        #region  Trailer Load

        public bool TryAutoReleaseBroadcast(out string error)
        {
            error = string.Empty;

            _LockAll();
            try
            {
//                 if (!_systemSettings.AutoReleaseBroadcastEnabled)
//                 {
//                     return false;
//                 }
// 
//                 if (!_loadManager.GetTargetLoadForBroadcastRelease(
//                     _systemSettings.GetItem(),
//                     out Load targetLoad))
//                 {
//                     error = null;
//                     return false;
//                 }
//                 int maxCountToRelease = targetLoad.IsInvalid
//                     ? Constant.LoadSize
//                     : targetLoad.WaitingCount;
//                 _broadcast.GetCurrentBroadcast()
// 
// 
// 
                return false;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool IsLoadLoadable(SlugLetter slugLetter)
        {
            _LockAll();
            try
            {
                return _slugManager.TryGetSlugByLetter(slugLetter, out Slug load)
                    && load.LoadLoadable;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool FinalizeLoad(
            SlugLetter slugLetter,
            string trailerID,
            out string error)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                slugLetter,
                nameof(slugLetter),
                new[] { SlugLetter.A, SlugLetter.B });

            int loadNumber;
            Slug load;
            _LockAll();
            try
            {
                loadNumber = slugLetter == SlugLetter.A
                    ? _systemSettings.SlugALoadNumber
                    : _systemSettings.SlugBLoadNumber;
//             }
//             finally
//             {
//                 _UnlockAll();
//             }
// 
// 
//             _LockAll();
//             try
//             {
                if (slugLetter == SlugLetter.A)
                {
                    _systemSettings.SlugALoadNumber = Constant.NoLoadNumber;
                    _systemSettings.SlugALoadStartedOn = Constant.BeforeBeginningOfTime;
                    _systemSettings.SlugALoadCompletedOn = Constant.BeforeBeginningOfTime;
                }
                else
                {
                    _systemSettings.SlugBLoadNumber = Constant.NoLoadNumber;
                    _systemSettings.SlugBLoadStartedOn = Constant.BeforeBeginningOfTime;
                    _systemSettings.SlugBLoadCompletedOn = Constant.BeforeBeginningOfTime;
                }

                _ = _slugManager.TryGetSlugByLetter(slugLetter, out load);
                load.SafeClear(true);
                load.Touch();
            }
            finally
            {
                _UnlockAll();
            }
            IEnumerable<LoadItem> loadItems = load
                .Where(item => item.Status == LoadItemStatus.Done)
                .OrderBy(item => item.Broadcast.Csn);

            if (!_SendLoadDataToMes(
                loadNumber,
                loadItems,
                DateTime.Now,
                trailerID,
                out error))
            {
                return false;
            }
            _ = load.Lock();
            load.SafeClear(true);
            load.Touch();
            load.Unlock();
            return true;
        }

        private bool _SendLoadDataToMes(
            int loadNumber,
            IEnumerable<LoadItem> loadItems,
            DateTime now,
            string trailerID,
            out string error)
        {
            string connectionString = XConfiguration.GetConnectionString(Constant.MesConnectionStringName);
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            try
            {
                string sql = string.Format(
                    @"INSERT INTO SHIP_ManifestHdr (ManifestID, Trailer_Nbr, Ship_DT) VALUES ('{0}','{1}','{2}'); SELECT Convert(BigInt, SCOPE_IDENTITY());",
                    loadNumber,
                    trailerID,
                    now.ToString(Constant.DateTimeFormat));

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    int Hdr_Data_ID = (int)command.ExecuteScalar();

                    //                     IEnumerable<LoadItem> loadItems = load
                    //                         .Where(item => item.Status == LoadItemStatus.Done)
                    //                         .OrderBy(item => item.Broadcast.Csn);

                    foreach (LoadItem loadItem in loadItems)
                    {
                        BroadcastItem broadcast = loadItem.Broadcast;
                        PalletItem pallet = loadItem.Pallet;

                        if (pallet.Sku != Constant.Row1EmptyPalletSku
                            && pallet.Sku != Constant.Row2EmptyPalletSku)
                        {
                            sql = string.Format(
                                @"INSERT INTO SHIP_ManifestDtl (ShipHdrID, PalletNbr, Job_ID, SKU, Vin_Ref_Nbr, Brdcst_Nbr) VALUES ({0},'{1}','{2}','{3}','{4}',{5});",
                                Hdr_Data_ID,
                                pallet.PalletID,
                                pallet.JobID,
                                pallet.Sku,
                                broadcast.Vin,
                                broadcast.Rotation);
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }
                    }

                    sql = $"INSERT INTO SHIP_ManifestQueue (ShipHdrID) VALUES ({Hdr_Data_ID});";
                    command.CommandText = sql;
                    _ = command.ExecuteNonQuery();
                }
                error = null;
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(_SendLoadDataToMes));
                error = x.Message;
                return false;
            }
        }

        public bool TryAutoAcceptLoad(SlugLetter slugLetter)
        {
            _LockAll();
            try
            {
                if (!_systemSettings.AutoAcceptLoadsEnabled)
                {
                    return false;
                }

                if (!_slugManager.TryGetSlugByLetter(slugLetter, out Slug load)
                    || !load.LoadDone)
                {
                    return false;
                }

                if (!_TryAcceptLoad(load, out string error))
                {
                    if (!error.IsNullOrEmpty())
                    {
                        XSystemEvent.Publish(
                            "AutoAcceptLoad",
                            XSystemEventLevel.Error,
                            $" {slugLetter.SlugDisplayName()} Auto Accept Error: {error}.");
                    }
                    return false;
                }
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        #endregion

        //==================================================================================

        #region  Command Service

        public bool TryAcceptLoad(SlugLetter slugLetter, out string error)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByLetter(slugLetter, out Slug load))
                {
                    error = $"Slug letter '{slugLetter.ToText()}' not found!";
                    XSystemEvent.Publish(
                        "AcceptLoad",
                        XSystemEventLevel.Error,
                        error);
                    return false;
                }

                if (!load.LoadDone)
                {
                    error = $"Cannot ACCEPT {slugLetter.SlugDisplayName()} when it is not Done.";
                    XSystemEvent.Publish(
                        slugLetter.SlugDisplayName(),
                        XSystemEventLevel.Error,
                        error);
                    return false;
                }

                return _TryAcceptLoad(load, out error);
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _TryAcceptLoad(Slug load, out string errorMessage)
        {
            DateTime now = DateTime.Now;
            DateTime firstPalletTimestamp = now;
            DateTime lastPalletTimestamp = now;
            DateTime loadDoneTimestamp = now;
            int loadNumber;
            if (load.SlugLetter == SlugLetter.A)
            {
                loadNumber = _systemSettings.SlugALoadNumber;
//                 firstPalletTimestamp = _systemSettings.LoadAFirstPalletTimestamp;
//                 lastPalletTimestamp = _systemSettings.LoadALastPalletTimestamp;
//                 loadDoneTimestamp = _systemSettings.LoadADoneTimestamp;
                firstPalletTimestamp = now;
                lastPalletTimestamp = now;
                loadDoneTimestamp = now;
            }
            else
            {
                loadNumber = _systemSettings.SlugBLoadNumber;
//                 firstPalletTimestamp = _systemSettings.LoadBFirstPalletTimestamp;
//                 lastPalletTimestamp = _systemSettings.LoadBLastPalletTimestamp;
//                 loadDoneTimestamp = _systemSettings.LoadBDoneTimestamp;
                firstPalletTimestamp = now;
                lastPalletTimestamp = now;
                loadDoneTimestamp = now;
            }

//             int smallestRotation = load.SmallestRotationOnDoneLoad;
//             int largestRotation = load.LargestRotationOnDoneLoad;
//             int previousRotation = _systemSettings.LargestRotationLoadedOnTrailer;
            int smallestRotation = 0;
            int largestRotation = 0;
            int previousRotation = 0;

            if (!LA.LoadArchive.BuildLoadArchive(
                load,
                loadNumber,
                Constant.NoTrailerID,
                firstPalletTimestamp,
                lastPalletTimestamp,
                loadDoneTimestamp,
                now,
                previousRotation,
                out LA.LoadArchive loadArchive,
                out errorMessage))
            {
                return false;
            }

            if (!LA.LoadArchive.ArchiveLoadData(
                loadArchive,
                out errorMessage))
            {
                return false;
            }

            load.SetLoadable();
            return true;
        }

        public bool TryReleaseBroadcast(
            SlugLetter slugLetter,
            int countToRelease,
            out string error)
        {
            _LockAll();
            try
            {




                if (_ReleaseBroadcast(false, slugLetter, countToRelease, out error))
                {
                    XSystemEvent.Publish(
                        "ReleaseBroadcast",
                        XSystemEventLevel.Information,
                        $"{countToRelease} Broadcast Records Released.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        XSystemEvent.Publish(
                            "ReleaseBroadcast",
                            XSystemEventLevel.Error,
                            "Failed to Release Broadcast: " + error);
                    }
                }
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        private bool _ReleaseBroadcast(
            bool autoRelease,
            SlugLetter slugLetter,
            int countToRelease,
            out string error)
        {
            bool touchBroadcast = false;
            Slug targetLoad = null;

            _LockAll();
            try
            {
                error = string.Empty;
                return false;

//                 string loadName = targetLoad.CollectionConfiguration.Name;
// 
// //                 int loadSize = targetLoad.ItemCount;
//                 List<BroadcastItem> broadcastItems = _broadcast.GetReleasableItems(
// //                     countToRelease,
//                     _systemSettings.LastCsnReleased,
//                     _systemSettings.LargestRotationReceived)
//                     .Take(countToRelease)
//                     .ToList();
//                 if (broadcastItems.Count < countToRelease)
//                 {
//                     if (autoRelease)
//                     {
//                         error = null;
//                     }
//                     else
//                     {
//                         error = $"{countToRelease} Broadcast records were requested for release. Only {broadcastItems.Count} are available.";
//                     }
//                     return false;
//                 }
//                 bool broadcastInhibit = _broadcast.InhibitChangeNotifications;
//                 _broadcast.InhibitChangeNotifications = true;
// 
//                 bool loadInhibit = targetLoad.InhibitChangeNotifications;
//                 targetLoad.InhibitChangeNotifications = true;
// 
//                 int lastBroadcastReleased = 0;
//                 int broadcastIndex = 0;
//                 int loadIndex = 0;
// 
//                 for (broadcastIndex = 0; broadcastIndex < countToRelease; broadcastIndex++)
//                 {
//                     if (broadcastIndex / 10 == 1)
//                     {
//                         loadIndex = broadcastIndex + 10;
//                     }
//                     else if (broadcastIndex / 10 == 2)
//                     {
//                         loadIndex = broadcastIndex - 10;
//                     }
//                     else
//                     {
//                         loadIndex = broadcastIndex;
//                     }
//                     BroadcastItem broadcastItem = broadcastItems[broadcastIndex];
//                     LoadItem loadItem = targetLoad[loadIndex];
//                     loadItem.Broadcast = broadcastItem;
//                     loadItem.Status = LoadItemStatus.Pending;
//                     targetLoad[loadIndex] = loadItem;
// 
//                     broadcastItem.Status = BroadcastStatus.Shipped;
//                     _broadcast[broadcastItem.Csn] = broadcastItem;
// 
//                     lastCsnReleased = broadcastItem.Csn;
//                 }
//                 Slug otherLoad = _slugManager.GetOtherSlug(targetLoad);
//                 if (otherLoad.PresequencedOrGreater)
//                 {
//                     targetLoad.ApplyInitialPickableStatuses();
//                 }
// 
//                 _broadcast.InhibitChangeNotifications = broadcastInhibit;
//                 targetLoad.InhibitChangeNotifications = loadInhibit;
//                 _systemSettings.LastBroadcastReleased = lastBroadcastReleased;
// 
//                 if (loadName == Constant.SlugAName)
//                 {
//                     _systemSettings.LoadAFirstPalletTimestamp = Constant.BeginningOfTime;
//                     _systemSettings.LoadALastPalletTimestamp = Constant.BeginningOfTime;
//                     _systemSettings.SlugALoadNumber = _systemSettings.NextLoadNumber;
//                 }
//                 else // has to be Load B
//                 {
//                     _systemSettings.LoadBFirstPalletTimestamp = Constant.BeginningOfTime;
//                     _systemSettings.LoadBLastPalletTimestamp = Constant.BeginningOfTime;
//                     _systemSettings.SlugBLoadNumber = _systemSettings.NextLoadNumber;
//                 }
// 
//                 error = string.Empty;
//                 XMessaging.Publish(
//                     Constant.CalculateShortagesMessageTopicName,
//                     XMessageScopes.All);
//                 touchBroadcast = true;
//                 return true;
            }
            finally
            {
                _UnlockAll();

                if (touchBroadcast)
                {
                    _broadcast.Touch();
                }
                if (targetLoad != null)
                {
                    targetLoad.Touch();
                }
            }
        }

//         public bool TryAbortLoad(
//             LoadLetter loadLetter,
//             bool recoverBroadcast,
//             out string error)
//         {
//             _LockAll();
//             try
//             {
//                 if (!_loadManager.TryGetLoadByLetter(loadLetter, out Load load))
//                 {
//                     error = $"Load {loadLetter.ToText()} is not valid.";
//                     return false;
//                 }
//                 int loadNumber = _systemSettings.GetLoadNumber(loadLetter);
//                 if (loadNumber <= Constant.NoLoadNumber)
//                 {
//                     error = $"Load {loadLetter.ToText()} has an invalid Load Number in the System Settings";
//                     return false;
//                 }
// 
//                 if (!MesQuery.TryAbortLoad(
//                     loadNumber,
//                     out error))
//                 {
//                     return false;
//                 }
//                 if (loadLetter == LoadLetter.A)
//                 {
//                     _systemSettings.LoadANumber = Constant.NoLoadNumber;
//                     _systemSettings.LoadALoadStartedOn = Constant.BeforeBeginningOfTime;
//                     _systemSettings.LoadALoadCompletedOn = Constant.BeforeBeginningOfTime;
//                 }
//                 else // B
//                 {
//                     _systemSettings.LoadBNumber = Constant.NoLoadNumber;
//                     _systemSettings.LoadBLoadStartedOn = Constant.BeforeBeginningOfTime;
//                     _systemSettings.LoadBLoadCompletedOn = Constant.BeforeBeginningOfTime;
//                 }
//                 load.Clear(true);
//                 foreach (LoadItem loadItem in load.ToList())
//                 {
//                     if (loadItem.LoadLetter == LoadLetter.None)
//                     {
//                         loadItem.LoadLetter = loadLetter;
//                     }
//                     load.SetAt(loadItem.NodeIndex, loadItem, true);
//                 }
//                 load.Touch();
//                 return true;
//             }
//             finally
//             {
//                 _UnlockAll();
//             }
//         }

        public bool TryAbortLoad(
            SlugLetter slugLetter,
            bool recoverBroadcast,
            out string errorMessage)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByLetter(slugLetter, out Slug load))
                {
                    errorMessage = $"Slug '{slugLetter.ToText()}' not found!";
                    XSystemEvent.Publish(
                        nameof (TryAbortLoad),
                        XSystemEventLevel.Error,
                        errorMessage);
                    return false;
                }
                return _TryAbortLoad(load, recoverBroadcast, out errorMessage);
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _TryAbortLoad(
            Slug load,
            bool recoverBroadcast,
            out string errorMessage)
        {
            errorMessage = null;
            List<BroadcastItem> broadcastToRecover = load
                .Where(l => l.Status > LoadItemStatus.Invalid)
                .Select(l => l.Broadcast)
                .OrderBy(b => b.Csn)
                .ToList();
            if (broadcastToRecover.Count == 0)
            {
                return true;
            }
            string largestCsn = broadcastToRecover.Last().Csn;
            string smallestCsn = broadcastToRecover.First().Csn;
            if (_systemSettings.LastCsnReleased == largestCsn)
            {
                _systemSettings.LastCsnReleased = smallestCsn.Right(1) == Constant.VehicleRow1CsnSuffix
                    ? $"{broadcastToRecover.First().Rotation}{Constant.VehicleRow2CsnSuffix}"
                    : $"{broadcastToRecover.First().Rotation - 1}{Constant.VehicleRow1CsnSuffix}";
            }
            else
            {
                errorMessage = $"You are attempting to abort a load which was not the last load released. You must first abort the last load released.";
                return false;
            }
            if (load.SlugLetter == SlugLetter.A)
            {
                _systemSettings.SlugALoadNumber--;
            }
            else // has to be Load B
            {
                _systemSettings.SlugBLoadNumber--;
            }
            load.Clear(true);
            if (recoverBroadcast)
            {
                List<string> csnListToRecover = _broadcast.Values
                    .Where(b =>
                        b.Csn.IsGreaterThan(_systemSettings.LastCsnReleased, true)
                        && b.Status == BroadcastStatus.Shipped)
                    .Select(b => b.Csn)
                    .ToList();
                _RecoverShippedBroadcast(csnListToRecover);
            }
            load.Touch();
            XSystemEvent.Publish(
                load.CollectionName,
                XSystemEventLevel.Notification,
                "Load was aborted.");
            return true;
        }

        public void RecoverShippedBroadcast(
            RecoverShippedBroadcastMessageData messageData,
            out string errorMessage)
        {
            _LockAll();
            try
            {
                errorMessage = null;

                List<string> csnListToRecover = _broadcast.Values
                    .Where(b =>
                        b.Csn.IsGreaterThan(_systemSettings.LastCsnReleased, true)
                        && b.Status == BroadcastStatus.Shipped)
                    .Select(b => b.Csn)
                    .ToList();
                _RecoverShippedBroadcast(csnListToRecover);
            }
            finally
            {
                _UnlockAll();
            }
        }

        private void _RecoverShippedBroadcast(List<string> csnListToRecover)
        {
            foreach (string csn in csnListToRecover)
            {
                if (_broadcast.TryGetItem(csn, out BroadcastItem broadcastItem))
                {
                    broadcastItem.Status = BroadcastStatus.OK;
                    _ = _broadcast.Update(csn, broadcastItem, true);
                }
            }
            _broadcast.Touch();
        }

        public void BulkConvertPalletStatus(
            BulkPalletStatusConversionMessageData messageData,
            out List<BinItem> failedConversionItems)
        {
            PalletStatus newPalletStatus = messageData.NewPalletStatus;
            string newComment = messageData.NewComment;
            failedConversionItems = new List<BinItem>();
            _LockAll();
            try
            {
                foreach (BinItem binItem in messageData.BinItems)
                {
                    BinItem newBinItem = XDataItem.Clone(binItem);
                    PalletItem palletItem = newBinItem.Pallet;
                    if (newPalletStatus != PalletStatus.Invalid)
                    {
                        palletItem.Status = newPalletStatus;
                    }
                    else if (messageData.MarkAudit)
                    {
                        newBinItem.Audit = true;
                    }
                    else if (messageData.UnmarkAudit)
                    {
                        newBinItem.Audit = false;
                    }
                    if (!string.IsNullOrWhiteSpace(newComment)
                        && (messageData.OverwriteComment
                            || string.IsNullOrWhiteSpace(palletItem.Comment)))
                    {
                        palletItem.Comment = newComment;
                    }
                    newBinItem.Pallet = palletItem;
                    if (!_storage.SafeSetAt(binItem.NodeIndex, binItem, ref newBinItem))
                    {
                        failedConversionItems.Add(binItem);
                    }
                }
            }
            finally
            {
                _UnlockAll();
            }
        }


        #endregion









    }
}
