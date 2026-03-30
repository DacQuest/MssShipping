using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        private LoadA _loadA;
        private LoadB _loadB;

        private LoadManager _loadManager;

        public static DataLayer Factory(
            out Storage storage,
            out LowerPit lowerPit,
            out UpperPit upperPit,
            out SystemSettings systemSettings,
            out Broadcast broadcast,
            out HoldCodes holdCodes,
            out LowerRecirc lowerRecirc,
            out UpperRecirc upperRecirc,
            out LoadA loadA,
            out LoadB loadB)
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
                out loadA,
                out loadB);
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
            out LoadA loadA,
            out LoadB loadB)
        {
            _ = XSharedCollection.Open(Constant.StorageName, out _storage);
            _ = XSharedCollection.Open(Constant.LowerPitName, out _lowerPit);
            _ = XSharedCollection.Open(Constant.UpperPitName, out _upperPit);
            _ = XSharedCollection.Open(Constant.SystemSettingsName, out _systemSettings);
            _ = XSharedCollection.Open(Constant.BroadcastName, out _broadcast);
            _ = XSharedCollection.Open(Constant.HoldCodesName, out _holdCodes);
            _ = XSharedCollection.Open(Constant.LowerRecircName, out _lowerRecirc);
            _ = XSharedCollection.Open(Constant.UpperRecircName, out _upperRecirc);
            _ = XSharedCollection.Open(Constant.LoadAName, out _loadA);
            _ = XSharedCollection.Open(Constant.LoadBName, out _loadB);
//             _ = XSharedCollection.Open(Constant.Name, out _);

            storage = _storage;
            lowerPit = _lowerPit;
            upperPit = _upperPit;
            systemSettings = _systemSettings;
            broadcast = _broadcast;
            holdCodes = _holdCodes;
            lowerRecirc = _lowerRecirc;
            upperRecirc = _upperRecirc;
            loadA = _loadA;
            loadB = _loadB;
            _loadManager = new LoadManager(_loadA, _loadB);

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
            if (_loadA != null)
            {
                _loadA.Dispose();
                _loadA = null;
            }
            if (_loadB != null)
            {
                _loadB.Dispose();
                _loadB = null;
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
            _loadManager.Lock();
        }

        private void _UnlockAll()
        {
            _loadManager.Unlock();
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
                    if (broadcastItem.RotationNumber > largestRotationReceived)
                    {
                        largestRotationReceived = broadcastItem.RotationNumber;
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
            Load load,
            int loadIndex,
            LoadItemStatus statusesToCompare)
        {
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfLessThan(0, loadIndex, "loadIndex");

            return statusesToCompare.IsFlagSet(load[loadIndex].Status);
        }

        private bool _MatchPreviousLoadItemStatus(
            bool palletArrival,
            Load load,
            LoadItem loadItem,
            LoadItemStatus targetStatus,
            LoadItemStatus statusesToCompare,
            out string fault)
        {
            int previousIndexInLane = Constant.PreviousInLaneLoadIndex[loadItem.NodeIndex];
            if (previousIndexInLane == Constant.BeforeFirst
                || _MatchLoadItemStatus(
                    load,
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
                load.CollectionConfiguration.Name,
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
                if (!_loadManager.TryGetLoadByPalletID(
                    palletID,
                    out Load load,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either load when attempting to set status to {LoadItemStatus.Picked}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Picking)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Load position to {LoadItemStatus.Picked.ToText()} when it was not set to {LoadItemStatus.Picking.ToText()}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                if (!_MatchPreviousLoadItemStatus(
                    false,
                    load,
                    loadItem,
                    LoadItemStatus.Picked,
                    LoadItemStatus.Presequenced | LoadItemStatus.Sequenced | LoadItemStatus.Done,
                    out fault))
                {
                    return false;
                }
                loadItem.Status = LoadItemStatus.Picked;
                loadItem.Shortage = false;
                load[loadItem.NodeIndex] = loadItem;

                _loadManager.SetNextPickable(
                     _systemSettings.GetItem(),
                     load,
                     loadItem.LoadLevel);
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
                if (!_loadManager.TryGetLoadByPalletID(
                    palletID,
                    out Load load,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either load when attempting to set status to {LoadItemStatus.Presequenced}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Picked)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Load position to {LoadItemStatus.Presequenced.ToText()} when it was not set to {LoadItemStatus.Picked.ToText()}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                if (!_MatchPreviousLoadItemStatus(
                    false,
                    load,
                    loadItem,
                    LoadItemStatus.Presequenced,
                    LoadItemStatus.Presequenced | LoadItemStatus.Sequenced | LoadItemStatus.Done,
                    out fault))
                {
                    return false;
                }
                loadItem.Status = LoadItemStatus.Presequenced;
                loadItem.Shortage = false;
                load[loadItem.NodeIndex] = loadItem;

               _loadManager.SetNextPickable(
                    _systemSettings.GetItem(),
                    load,
                    loadItem.LoadLevel);
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
                if (!_loadManager.TryGetLoadByPalletID(
                    palletID,
                    out Load load,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either load when attempting to set status to {LoadItemStatus.Sequenced}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Presequenced)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Load position to {LoadItemStatus.Sequenced.ToText()} when it was not set to {LoadItemStatus.Presequenced.ToText()}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                if (!_MatchPreviousLoadItemStatus(
                    false,
                    load,
                    loadItem,
                    LoadItemStatus.Sequenced,
                    LoadItemStatus.Sequenced | LoadItemStatus.Done,
                    out fault))
                {
                    return false;
                }
                loadItem.Status = LoadItemStatus.Sequenced;
                loadItem.Shortage = false;
                load[loadItem.NodeIndex] = loadItem;

               _loadManager.SetNextPickable(
                    _systemSettings.GetItem(),
                    load,
                    loadItem.LoadLevel);
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
                if (!_loadManager.TryGetLoadByPalletID(
                    palletID,
                    out Load load,
                    out LoadItem loadItem))
                {
                    fault = $"Pallet ID {palletID} not found on either load when attempting to set status to {LoadItemStatus.Done}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                if (loadItem.Status != LoadItemStatus.Sequenced)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Load position to {LoadItemStatus.Done.ToText()} when it was not set to {LoadItemStatus.Sequenced.ToText()}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Warning,
                        fault);
                    return false;
                }
                int previousIndexInLane = Constant.PreviousInLaneLoadIndex[loadItem.NodeIndex];
                if (previousIndexInLane != Constant.BeforeFirst
                    && load[previousIndexInLane].Status != LoadItemStatus.Done)
                {
                    fault = $"({loadItem.Coordinates}) Attempted to set a Load position to {LoadItemStatus.Done.ToText()} when the previous in lane was not set to {LoadItemStatus.Done.ToText()}.";
                    XSystemEvent.Publish(
                        load.CollectionConfiguration.Name,
                        XSystemEventLevel.Error,
                        fault);
                    return false;
                }
                loadItem.Status = LoadItemStatus.Done;
                loadItem.Shortage = false;
                load[loadItem.NodeIndex] = loadItem;

               _loadManager.SetNextPickable(
                    _systemSettings.GetItem(),
                    load,
                    loadItem.LoadLevel);
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
                            && palletItem.Sku != Constant.StackSku
                            && _TryAssignHotJobAtRouter(
                                craneNumber,
                                level,
                                palletItem,
                                out LoadItem loadItem))
                        {
                            moveCommand = Constant.IRMoveCommandForward;
                            extendedState = $"({Constant.PalletTypeHotJob})  Assigned Hot Job Pallet {palletID} to Load {loadItem.LoadLetter}";
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
                    if (_loadManager.TryGetLoadByPalletID(
                        palletID,
                        out Load load,
                        out LoadItem loadItem))
                    {
                        if (loadItem.LoadLevel == level)
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
                        load.RollbackPick(palletID);
                    }
                    palletItem = fetchedPalletItem;
                    if (palletItem.Status == PalletStatus.OK
                        || palletItem.Status == PalletStatus.Hold
                        || palletItem.Status == PalletStatus.Reserved)
                    {
                        if (palletItem.Status == PalletStatus.OK
                            && palletItem.Sku != Constant.StackSku
                            && _TryAssignHotJobAtRouter(
                                craneNumber,
                                level,
                                palletItem,
                                out loadItem))
                        {
                            moveCommand = Constant.IRMoveCommandForward;
                            extendedState = $"({Constant.PalletTypeHotJob})  Assigned Hot Job Pallet {palletID} to Load {loadItem.LoadLetter}";
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
                        else if (palletItem.Status == PalletStatus.Stack)
                        {
                            palletType = Constant.PalletTypeStack;
                        }
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
            if (!_loadManager.TryGetPrimaryLoad(
                _systemSettings.GetItem(),
                out Load primaryLoad,
                out Load secondaryLoad))
            {
                loadItem = null;
                return false;
            }
            loadItem = null;
            Load currentLoad = null;
            if (_CanPickToLoad(primaryLoad.LoadLetter))
            {
                currentLoad = primaryLoad;
                loadItem = currentLoad
                    .GetLoadInPickSearchOrder()
                    .Where(l => levels.IsFlagSet(l.LoadLevel))
                    .FirstOrDefault(l =>
                        l.Status == LoadItemStatus.Pickable
                        && l.Broadcast.Sku == palletItem.Sku);
            }
            if (loadItem == null
                && _CanPickToLoad(secondaryLoad.LoadLetter))
            {
                currentLoad = secondaryLoad;
                loadItem = currentLoad
                    .GetLoadInPickSearchOrder()
                    .Where(l => levels.IsFlagSet(l.LoadLevel))
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
                currentLoad[loadItem.NodeIndex] = loadItem;
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
                    if (moveCommand == Constant.IRMoveCommandToCrane)
                    {
                        extendedState = $"({Constant.PalletTypeStore})  Moving Pallet {palletItem.PalletID} to Crane {(int)targetCrane}";
                    }
                    else
                    {
                        extendedState = $"({Constant.PalletTypeStore})  Moving Pallet {palletItem.PalletID} forward to Crane {(int)targetCrane}";
                    }
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
                if (!_loadManager.TryGetPrimaryLoad(
                    _systemSettings.GetItem(),
                    out Load primaryLoad,
                    out Load secondaryLoad))
                {
                    return false;
                }

                _loadManager.GetPickableLoadItems(
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
                extendedState = $"({Constant.PalletTypeLoad})  Getting Load Pallet {loadItem.Pallet.PalletID} from {getCommand} to Load {loadItem.LoadLetter}.";
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
                if (!_systemSettings.CanDoLoadPick(craneNumber, pickableItem.LoadLevel))
                {
                    continue;
                }
                if (pickableItem.LoadLevel == Levels.Lower && !lowerOutboundClear)
                {
                    continue;
                }
                if (pickableItem.LoadLevel == Levels.Upper && !upperOutboundClear)
                {
                    continue;
                }
                if (!_loadManager.TryGetLoadByLetter(pickableItem.LoadLetter, out Load load))
                {
                    XSystemEvent.Publish(
                        nameof(_TryLoadPick),
                        XSystemEventLevel.Error,
                        $"Unknown Load Letter {pickableItem.LoadLetter} referenced in LoadItem with NodeIndex {pickableItem.NodeIndex}.");
                    continue;
                }
                if (!_CanPickToLoad(load.LoadLetter))
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
                return _loadManager.TryGetLoadByPalletID(
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
                        case PalletStatus.Stack:
                            craneFunction = CraneFunction.StackPick;
                            return true;
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

        private bool _IsLoadEnabled(LoadLetter loadLetter)
        {
            return loadLetter == LoadLetter.None
                ? false
                : loadLetter == LoadLetter.A
                    ? _systemSettings.LoadAEnabled
                    : _systemSettings.LoadBEnabled;
        }

        private bool _CanPickToLoad(LoadLetter loadLetter)
        {
            if (loadLetter == LoadLetter.None)
            {
                return false;
            }
            bool onlyA = _systemSettings.LoadPickPriority == LoadPickPriority.LoadAOnly;
            bool onlyB = _systemSettings.LoadPickPriority == LoadPickPriority.LoadBOnly;
            return _IsLoadEnabled(loadLetter)
                && ((!onlyA && loadLetter == LoadLetter.B)
                    || (!onlyB && loadLetter == LoadLetter.A));
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
                _loadManager.RollbackPick(palletID);
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
                if (_storage
                        .Any(b => b.CraneNumber == craneNumber
                            && b.Audit
                            && !b.Disabled
                            && !b.NotUsable))
                {
                    return false;
                }
                if (!_TryCompactStack(craneNumber))
                {
                    return _TryCompactPallet(craneNumber);
                }
                return true;
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
        #endregion

        //==================================================================================

        #region  Command Service

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
