using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DevExpress.CodeParser;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraReports.UI;
using Mss.Collections;
using Mss.Common;
using Mss.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using LA = Mss.Data.LoadArchive;

namespace Mss.Data
{
    public class DataLayer : XDisposable
    {
        private Storage _storage;
        private AssignmentPit _assignmentPit;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private HoldCodes _holdCodes;
        private LowerRecircBuffer _lowerRecirc;
        private UpperRecircBuffer _upperRecirc;
        private SlugA _slugA;
        private SlugB _slugB;

        private SlugManager _slugManager;

        public static DataLayer Create(
            out Storage storage,
            out AssignmentPit assignmentPit,
            out LowerPit lowerPit,
            out UpperPit upperPit,
            out SystemSettings systemSettings,
            out Broadcast broadcast,
            out HoldCodes holdCodes,
            out LowerRecircBuffer lowerRecirc,
            out UpperRecircBuffer upperRecirc,
            out SlugA slugA,
            out SlugB slugB)
        {
            DataLayer dataLayer = new DataLayer();
            return dataLayer._Initialize(
                out storage,
                out assignmentPit,
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
            out AssignmentPit assignmentPit,
            out LowerPit lowerPit,
            out UpperPit upperPit,
            out SystemSettings systemSettings,
            out Broadcast broadcast,
            out HoldCodes holdCodes,
            out LowerRecircBuffer lowerRecirc,
            out UpperRecircBuffer upperRecirc,
            out SlugA slugA,
            out SlugB slugB)
        {
            _ = XSharedCollection.Open(Constant.StorageName, out _storage);
            _ = XSharedCollection.Open(Constant.AssignmentPitName, out _assignmentPit);
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
            assignmentPit = _assignmentPit;
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
            if (_assignmentPit != null)
            {
                _assignmentPit.Dispose();
                _assignmentPit = null;
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
            _ = _assignmentPit.Lock();
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
            _assignmentPit.Unlock();
            _storage.Unlock();
        }

        #region General

        public void ReceiveBroadcast(List<BroadcastItem> broadcastItems)
        {
            if (broadcastItems.Count() == 0)
            {
                return;
            }

            _LockAll();
            int largestRotationReceived = _systemSettings.LargestRotationReceived;
            bool touch = false;
            try
            {
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

        #region Assignment, Upper, and Lower Pit

        // Must be called from within _LockAll()
        private bool _GetAvailableLevel(Levels preferredLevel, out Levels availableLevel)
        {
            availableLevel = Levels.None;

            int upperAvailable = _systemSettings.UpperLevelInboundEnabled
                ? Constant.UpperAssignmentBufferSize - _assignmentPit.Values.Count(p => p.PitCode == PitCode.Upper)
                : 0;
            int lowerAvailable = _systemSettings.LowerLevelInboundEnabled
                ? Constant.LowerAssignmentBufferSize - _assignmentPit.Values.Count(p => p.PitCode == PitCode.Lower)
                : 0;

            if (upperAvailable > lowerAvailable)
            {
                availableLevel =  upperAvailable > 0
                    ? Levels.Upper
                    : Levels.None;
            }
            else if (lowerAvailable > upperAvailable)
            {
                availableLevel = lowerAvailable > 0
                    ? Levels.Lower
                    : Levels.None;
            }
            else if (upperAvailable > 0) // same available on each level so test one for zero
            {
                availableLevel = preferredLevel;
            }
            return availableLevel != Levels.None;
        }

        public bool TryFindAssignmentPitPalletByJobID(
            string jobID,
            out PitItem pitItem)
        {
            _LockAll();
            try
            {
                pitItem = _assignmentPit
                    .Values
                    .FirstOrDefault(p => p.Pallet.JobID == jobID);
                return pitItem != null;
            }
            finally
            {
                _UnlockAll();
            }
        }

        private int _GetAssignmentPitCodeCount(PitCode pitCode)
        {
            _LockAll();
            try
            {
                return _assignmentPit
                    .Values
                    .Count(p => p.PitCode == pitCode);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void SetAssignmentPitPallet(
            PalletItem palletItem,
            PitCode pitCode)
        {
            _LockAll();
            try
            {
                _assignmentPit.Set(Levels.None, palletItem, pitCode);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryGetAssignmentPitItem(
            string palletID,
            out PitItem pitItem)
        {
            _LockAll();
            try
            {
                return _assignmentPit.TryGetItem(palletID, out pitItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void RemoveAssignmentPitPallet(string palletID)
        {
            _LockAll();
            try
            {
                _ = _assignmentPit.Remove(palletID);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryGetPitItem(
            string palletID,
            out PitItem pitItem)
        {
            _LockAll();
            try
            {
                return _upperPit.TryGetItem(palletID, out pitItem)
                    || _lowerPit.TryGetItem(palletID, out pitItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryGetPitItem(
            Levels level,
            string palletID,
            out PitItem pitItem)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof (level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _LockAll();
            try
            {
                return (level == Levels.Upper)
                    ? _upperPit.TryGetItem(palletID, out pitItem)
                    : _lowerPit.TryGetItem(palletID, out pitItem);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryFindPitPalletByJobID(
            Levels level,
            string jobID,
            out PitItem pitItem)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof (level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _LockAll();
            try
            {
                Pit pit = level == Levels.Upper
                    ? _upperPit
                    : (Pit)_lowerPit;


                pitItem = pit
                    .Values
                    .FirstOrDefault(p => p.Pallet.JobID == jobID);
                return pitItem != null;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void RemovePitPallet(string palletID)
        {
            _LockAll();
            try
            {
                _ = _assignmentPit.Remove(palletID);
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
            _LockAll();
            try
            {
                Pit pit = level == Levels.Upper
                    ? _upperPit
                    : level == Levels.Lower
                        ? _lowerPit
                        : (Pit)_assignmentPit;
                pit.Set(level, palletItem, pitCode);
            }
            finally
            {
                _UnlockAll();
            }
        }

        #endregion

        #region Recirc Buffer

        public void RemoveRecircBufferPallet(string palletID)
        {
            _LockAll();
            try
            {
                _ = _lowerRecirc.Remove(palletID);
                _ = _upperRecirc.Remove(palletID);
            }
            finally
            {
                _UnlockAll();
            }
        }

        public void SetRecircBufferPallet(
            Levels level,
            PalletItem palletItem)
        {
            _LockAll();
            try
            {
                XArgumentChecker.ThrowIfNotContainedIn(
                    level,
                    nameof(level),
                    new Levels[] { Levels.Lower, Levels.Upper });

                _LockAll();
                try
                {
                    RecircBuffer recirc = level == Levels.Upper
                        ? (RecircBuffer)_upperRecirc
                        : (RecircBuffer)_lowerRecirc;
                    _ = recirc.Update(palletItem.PalletID, palletItem);
                }
                finally
                {
                    _UnlockAll();
                }
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

        // Must be called from within _LockAll()
        private bool _MatchLoadItemStatus(
            Slug load,
            int loadIndex,
            LoadItemStatus statusesToCompare)
        {
            XArgumentChecker.ThrowIfGreaterThanOrEqualTo(Constant.LoadSize, loadIndex, "loadIndex");
            XArgumentChecker.ThrowIfLessThan(0, loadIndex, "loadIndex");

            return statusesToCompare.IsFlagSet(load[loadIndex].Status);
        }

        // Must be called from within _LockAll()
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
                BroadcastItem broadcast = loadItem.Broadcast;
                broadcast.Shortage = false;
                loadItem.Broadcast = broadcast;
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
                BroadcastItem broadcast = loadItem.Broadcast;
                broadcast.Shortage = false;
                loadItem.Broadcast = broadcast;
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
                BroadcastItem broadcast = loadItem.Broadcast;
                broadcast.Shortage = false;
                loadItem.Broadcast = broadcast;
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
                BroadcastItem broadcast = loadItem.Broadcast;
                broadcast.Shortage = false;
                loadItem.Broadcast = broadcast;
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

        public bool ProcessPalletAtAssignment(
                OperationCode operationCode,
                string palletID,
                out PalletItem palletItem,
                out int moveCommand,
                out string extendedState,
                out string fault)
        {
            switch (operationCode)
            {
                case OperationCode.AS1:
                    return _ProcessPalletAtAssignment1(
//                         operationCode,
                        palletID,
                        out palletItem,
                        out moveCommand,
                        out extendedState,
                        out fault);
                case OperationCode.AS2:
                    return _ProcessPalletAtAssignment2(
//                         operationCode,
                        palletID,
                        out palletItem,
                        out moveCommand,
                        out extendedState,
                        out fault);
                case OperationCode.AS3:
                    return _ProcessPalletAtAssignment3(
//                         operationCode,
                        palletID,
                        out palletItem,
                        out moveCommand,
                        out extendedState,
                        out fault);
                default:
                    palletItem = null;
                    moveCommand = Constant.NoMoveCommand;
                    extendedState = string.Empty;
                    fault = $"Unknown Assignment Operation Code {operationCode}({operationCode.ToText()}).";
                    return false;
            }
        }

        // Must be called from within _LockAll()
        private bool _ProcessPalletAtAssignment1(
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            if (!MesInterface.TryFetchPalletItem(
                OperationCode.AS1,
                palletID,
                out bool sendToTwentyPercentArea,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                palletItem = null;
                moveCommand = Constant.NoMoveCommand;
                extendedState = string.Empty;
                return false;
            }

            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;
            fault = string.Empty;

            _LockAll();
            try
            {
                palletItem = fetchedPalletItem;
                Levels availableLevel;
                if (sendToTwentyPercentArea)
                {
                    if (_assignmentPit.Values.Count(p => p.PitCode == PitCode.Twenty) < Constant.TwentyAssignmentBufferSize)
                    {
                        SetPitPallet(Levels.None, palletItem, PitCode.Twenty);
                        moveCommand = Constant.Assignment1MoveCommandForward;
                        extendedState = $"Routing Pallet {palletID} to the 20% Area.";
                        return true;
                    }
                    moveCommand = Constant.NoMoveCommand;
                    extendedState = $"Pallet {palletID} does not currently have an available Destination.";
                    fault = string.Empty;
                    return false;
                }
                else if (palletItem.Status == PalletStatus.Purge
                    || palletItem.Status == PalletStatus.Unknown)
                {
                    palletItem.Status = PalletStatus.Purge;
                    _ = _assignmentPit.Remove(palletID);
                    if (_GetAvailableLevel(Levels.Lower, out availableLevel))
                    {
                        if (availableLevel == Levels.Lower)
                        {
                            moveCommand = Constant.Assignment1MoveCommandLower;
                            SetPitPallet(Levels.None, palletItem, PitCode.Lower);
                            extendedState = $"Routing Pallet {palletID} to Purge via Lower Level.";
                        }
                        else // availableLevel == Levels.Upper
                        {
                            moveCommand = Constant.Assignment1MoveCommandForward;
                            SetPitPallet(Levels.None, palletItem, PitCode.Upper);
                            extendedState = $"Routing Pallet {palletID} to Purge via Upper Level.";
                        }
                        string comment = $"Pallet {palletItem.PalletID} received at Assignment1 with no data";
                        palletItem.Comment = comment;
                        XSystemEvent.Publish(
                            "Assignment 1",
                            XSystemEventLevel.Warning,
                            comment);
                        return true;
                    }
                    moveCommand = Constant.NoMoveCommand;
                    extendedState = $"Pallet {palletID} does not currently have an available Destination.";
                    fault = string.Empty;
                    return false;
                }
                if (_GetAvailableLevel(Levels.Lower, out availableLevel))
                {
                    if (availableLevel == Levels.Lower)
                    {
                        moveCommand = Constant.Assignment1MoveCommandLower;
                        SetPitPallet(Levels.None, palletItem, PitCode.Lower);
                        extendedState = $"Routing Pallet {palletID} to Lower Level.";
                    }
                    else // availableLevel == Levels.Upper
                    {
                        moveCommand = Constant.Assignment1MoveCommandForward;
                        SetPitPallet(Levels.None, palletItem, PitCode.Upper);
                        extendedState = $"Routing Pallet {palletID} Forward.";
                    }
                    return true;
                }
                moveCommand = Constant.NoMoveCommand;
                extendedState = $"Pallet {palletID} does not currently have an available Destination.";
                fault = string.Empty;
                return false;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _ProcessPalletAtAssignment2(
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            if (!MesInterface.TryFetchPalletItem(
                OperationCode.AS2,
                palletID,
                out bool sendToTwentyPercentArea,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                palletItem = null;
                moveCommand = Constant.NoMoveCommand;
                extendedState = string.Empty;
                return false;
            }

            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;
            fault = string.Empty;

            _LockAll();
            try
            {
                palletItem = fetchedPalletItem;
                Levels availableLevel;
                if (sendToTwentyPercentArea)
                {
                    if (_assignmentPit.Values.Count(p => p.PitCode == PitCode.Twenty) <= Constant.TwentyAssignmentBufferSize)
                    {
                        SetPitPallet(Levels.None, palletItem, PitCode.Twenty);
                        moveCommand = Constant.Assignment1MoveCommandForward;
                        extendedState = $"Routing Pallet {palletID} to the 20% Area.";
                        return true;
                    }
                    moveCommand = Constant.NoMoveCommand;
                    extendedState = $"Pallet {palletID} does not currently have an available Destination.";
                    fault = string.Empty;
                    return false;
                }
                else if (palletItem.Status == PalletStatus.Purge
                    || palletItem.Status == PalletStatus.Unknown)
                {
                    palletItem.Status = PalletStatus.Purge;
                    _ = _assignmentPit.Remove(palletID);
                    if (_GetAvailableLevel(Levels.Lower, out availableLevel))
                    {
                        if (availableLevel == Levels.Lower)
                        {
                            moveCommand = Constant.Assignment2MoveCommandLower;
                            SetPitPallet(Levels.None, palletItem, PitCode.Lower);
                            extendedState = $"Routing Pallet {palletID} to Purge via Lower Level.";
                        }
                        else // availableLevel == Levels.Upper
                        {
                            moveCommand = Constant.Assignment2MoveCommandUpper;
                            SetPitPallet(Levels.None, palletItem, PitCode.Upper);
                            extendedState = $"Routing Pallet {palletID} to Purge via Upper Level.";
                        }
                        string comment = $"Pallet {palletItem.PalletID} received at Assignment2 with no data";
                        palletItem.Comment = comment;
                        XSystemEvent.Publish(
                            "Assignment 1",
                            XSystemEventLevel.Warning,
                            comment);
                        return true;
                    }
                    moveCommand = Constant.NoMoveCommand;
                    extendedState = $"Pallet {palletID} does not currently have an available Destination.";
                    fault = string.Empty;
                    return false;
                }
                if (_GetAvailableLevel(Levels.Lower, out availableLevel))
                {
                    if (availableLevel == Levels.Lower)
                    {
                        moveCommand = Constant.Assignment2MoveCommandLower;
                        SetPitPallet(Levels.None, palletItem, PitCode.Lower);
                        extendedState = $"Routing Pallet {palletID} to Lower Level.";
                    }
                    else // availableLevel == Levels.Upper
                    {
                        moveCommand = Constant.Assignment2MoveCommandUpper;
                        SetPitPallet(Levels.None, palletItem, PitCode.Upper);
                        extendedState = $"Routing Pallet {palletID} to Upper Level.";
                    }
                    return true;
                }
                moveCommand = Constant.NoMoveCommand;
                extendedState = $"Pallet {palletID} does not currently have a Destination.";
                fault = string.Empty;
                return false;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _ProcessPalletAtAssignment3(
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            if (!MesInterface.TryFetchPalletItem(
                OperationCode.AS3,
                palletID,
                out bool sendToTwentyPercentArea,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                palletItem = null;
                moveCommand = Constant.NoMoveCommand;
                extendedState = string.Empty;
                return false;
            }

            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;

            _LockAll();
            try
            {
                palletItem = fetchedPalletItem;
                Levels availableLevel;
                if (sendToTwentyPercentArea)
                {
                    SetPitPallet(Levels.None, palletItem, PitCode.Twenty);
                    moveCommand = Constant.Assignment3MoveCommandTwenty;
                    extendedState = $"Routing Pallet {palletID} to the 20% Area.";
                    return true;
                }
                else if (palletItem.Status == PalletStatus.Purge
                    || palletItem.Status == PalletStatus.Unknown)
                {
                    palletItem.Status = PalletStatus.Purge;
                    _ = _assignmentPit.Remove(palletID);
                    if (_GetAvailableLevel(Levels.Lower, out availableLevel))
                    {
                        if (availableLevel == Levels.Lower)
                        {
                            moveCommand = Constant.Assignment3MoveCommandLower;
                            SetPitPallet(Levels.None, palletItem, PitCode.Lower);
                            extendedState = $"Routing Pallet {palletID} to Purge via Lower Level.";
                        }
                        else // availableLevel == Levels.Upper
                        {
                            moveCommand = Constant.Assignment3MoveCommandUpper;
                            SetPitPallet(Levels.None, palletItem, PitCode.Upper);
                            extendedState = $"Routing Pallet {palletID} to Purge via Upper Level.";
                        }
                        string comment = $"Pallet {palletItem.PalletID} received at Assignment3 with no data";
                        palletItem.Comment = comment;
                        XSystemEvent.Publish(
                            "Assignment 1",
                            XSystemEventLevel.Warning,
                            comment);
                        return true;
                    }
                    moveCommand = Constant.NoMoveCommand;
                    extendedState = $"Pallet {palletID} does not currently have an available Destination.";
                    fault = string.Empty;
                    return false;
                }
                if (_GetAvailableLevel(Levels.Lower, out availableLevel))
                {
                    if (availableLevel == Levels.Lower)
                    {
                        moveCommand = Constant.Assignment3MoveCommandLower;
                        SetPitPallet(Levels.None, palletItem, PitCode.Lower);
                        extendedState = $"Routing Pallet {palletID} to Lower Level.";
                    }
                    else // availableLevel == Levels.Upper
                    {
                        moveCommand = Constant.Assignment3MoveCommandUpper;
                        SetPitPallet(Levels.None, palletItem, PitCode.Upper);
                        extendedState = $"Routing Pallet {palletID} to Upper Level.";
                    }
                    return true;
                }
                moveCommand = Constant.NoMoveCommand;
                extendedState = $"Pallet {palletID} does not currently have a Destination.";
                fault = string.Empty;
                return false;
            }
            finally
            {
                _UnlockAll();
            }
        }

        #endregion

        #region Router

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
                out PalletItem fetchedPalletItem,
                out fault))
            {
                palletItem = null;
                return false;
            }

            _LockAll();
            try
            {
                if  (craneNumber != CraneNumber.Crane1
                    && TryGetPitItem(level, palletID, out PitItem pitItem))
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
                            moveCommand = Constant.RouterMoveCommandForward;
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
                            moveCommand = Constant.RouterMoveCommandToCrane;
                            extendedState = $"({Constant.PalletTypeStore})  Diverting Pallet {palletItem.PalletID} to Crane {(int)craneNumber}";
                            return true;
                        }
                        moveCommand = Constant.RouterMoveCommandForward;
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
                            palletType = palletItem.IsFrontStack
                                ? Constant.PalletTypeStack1
                                : Constant.PalletTypeStack2;
                        }
                        moveCommand = Constant.RouterMoveCommandForward;
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
                                string error = $"({loadItem.Coordinates})  Load Pallet {palletID} arrived at {level} Router {(int)craneNumber} with a Load Item Status of {loadItem.Status.ToText()}. It should be {LoadItemStatus.Picked.ToText()}. Operation faulted.";
                                XSystemEvent.Publish(
                                    $"{level} Router {(int)craneNumber}",
                                    XSystemEventLevel.Error,
                                    error);
                                fault = $"Load Pallet {palletID} arrived with a Load Item Status of {loadItem.Status.ToText()}. It should be {LoadItemStatus.Picked.ToText()}.";
                                return false;
                            }
                            palletItem = loadItem.Pallet;
                            moveCommand = Constant.RouterMoveCommandForward;
                            extendedState = $"({Constant.PalletTypeLoad})  Moving Pallet {palletID} forward";
                            return true;
                        }
                        slug.RollbackPick(palletID);
                    }
                    palletItem = fetchedPalletItem;
                    PalletStatus palletStatus = palletItem.Status;
                    if (palletStatus == PalletStatus.OK
                        || palletStatus == PalletStatus.Hold
                        || palletStatus == PalletStatus.Reserved)
                    {
                        if (!palletItem.IsStack
                            && palletItem.Status == PalletStatus.OK
                            && _TryAssignHotJobAtRouter(
                                craneNumber,
                                level,
                                palletItem,
                                out loadItem))
                        {
                            moveCommand = Constant.RouterMoveCommandForward;
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
//                         palletStatus = palletItem.Status;
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
                        moveCommand = Constant.RouterMoveCommandForward;
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
            IEnumerable<AssignmentCandidate> candidates = _GetAssignmentCandidates(level, palletItem);
            foreach (AssignmentCandidate candidate in candidates)
            {
                CraneNumber targetCrane = candidate.TargetCrane;

                if (targetCrane >= craneNumber
                    && candidate.AvailableBins > 0
                    && candidate.CanAcceptPallet
                    && candidate.AssignedCounts < Constant.CraneInboundBufferSize)
                {
                    moveCommand = targetCrane == craneNumber
                        ? Constant.RouterMoveCommandToCrane
                        : Constant.RouterMoveCommandForward;
                    extendedState = moveCommand == Constant.RouterMoveCommandToCrane
                        ? $"({Constant.PalletTypeStore})  Diverting Pallet {palletItem.PalletID} to Crane {(int)targetCrane}"
                        : $"({Constant.PalletTypeStore})  Assigned Pallet {palletItem.PalletID} to Crane {(int)targetCrane}";
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
            PalletItem palletItem)
        {
            BinSize binSize = palletItem.BinSize;
            AssignmentCandidate[] candidates = new AssignmentCandidate[Constant.MaxCranes];
            int[] assignedCraneCounts = GetAssignedCraneCounts(level); // accessed by crane index
            (CraneNumber craneNumber, int skuCount)[] scs = _storage.GetPrioritizedSkuCountPerCrane(palletItem.Sku);
            foreach ((CraneNumber craneNumber, int skuCount) in scs)
            {
                if (craneNumber > CraneNumber.None)
                {
                    int emptyBins = _storage.GetEmptyBinCount(binSize, craneNumber);
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

//         public bool CanDoStore(CraneNumber craneNumber)
//         {
//             _LockAll();
//             try
//             {
//                 return _systemSettings.CanDoStore(craneNumber)
//                     && _storage.IsStorableBinAvailable(craneNumber, binSize);
//             }
//             finally
//             {
//                 _UnlockAll();
//             }
//         }

        public void SetStorageLocationToOfflineDisabled(
            CraneNumber craneNumber,
            int location)
        {
            _LockAll();
            try
            {
                int nodeIndex = BinItem.LocationToNodeIndex(location);
                BinItem binItem = _storage[nodeIndex];
                binItem.Pallet = new PalletItem();
                binItem.Audit = false;
                binItem.StoredOn = Constant.BeginningOfTime;
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

        public bool TryFrontStackPick(
            CraneNumber craneNumber,
            bool upperOutboundClear,
            out PalletItem palletItem,
            out int getCommand,
            out string extendedState)
        {
            _LockAll();
            try
            {
                palletItem = null;
                getCommand = Constant.NoCraneCommand;
                extendedState = string.Empty;

                if (!upperOutboundClear
                    || !_systemSettings.CanDoStackPicks(craneNumber))
                {
                    return false;
                }
                if (!_storage.TryAllocateStack1Pick(craneNumber, out BinItem binItem))
                {
                    return false;
                }
                palletItem = binItem.Pallet;
                getCommand = binItem.Location;
                extendedState = $"({Constant.PalletTypeStack1})  Getting Pallet {palletItem.PalletID} from {getCommand}";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryRearStackPick(
            CraneNumber craneNumber,
            bool upperOutboundClear,
            out PalletItem palletItem,
            out int getCommand,
            out string extendedState)
        {
            _LockAll();
            try
            {
                palletItem = null;
                getCommand = Constant.NoCraneCommand;
                extendedState = string.Empty;

                if (!upperOutboundClear
                    || !_systemSettings.CanDoStackPicks(craneNumber))
                {
                    return false;
                }
                if (!_storage.TryAllocateStack2Pick(craneNumber, out BinItem binItem))
                {
                    return false;
                }
                palletItem = binItem.Pallet;
                getCommand = binItem.Location;
                extendedState = $"({Constant.PalletTypeStack2})  Getting Pallet {palletItem.PalletID} from {getCommand}";
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
                extendedState = $"({Constant.PalletTypeLoad})  Getting Load Pallet {loadItem.Pallet.PalletID} from {getCommand} for Slug {loadItem.SlugLetter}.";
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
                if (!_systemSettings.CanDoLoadPick(craneNumber, pickableItem.SlugLevel)
                    || (pickableItem.SlugLevel == Levels.Lower && !lowerOutboundClear)
                    || (pickableItem.SlugLevel == Levels.Upper && !upperOutboundClear)
                    || !_CanPickToSlug(pickableItem.SlugLetter))
                {
                    continue;
                }
//                 if (pickableItem.SlugLevel == Levels.Lower && !lowerOutboundClear)
//                 {
//                     continue;
//                 }
//                 if (pickableItem.SlugLevel == Levels.Upper && !upperOutboundClear)
//                 {
//                     continue;
//                 }
                if (!_slugManager.TryGetSlugByLetter(pickableItem.SlugLetter, out Slug slug))
                {
                    XSystemEvent.Publish(
                        nameof(_TryLoadPick),
                        XSystemEventLevel.Error,
                        $"Unknown Slug Letter {pickableItem.SlugLetter} referenced in LoadItem with NodeIndex {pickableItem.NodeIndex}.");
                    continue;
                }
//                 if (!_CanPickToSlug(slug.SlugLetter))
//                 {
//                     continue;
//                 }
                if (_storage.TryAllocateLoadPick(
                    craneNumber,
                    _systemSettings.FifoMode,
                    pickableItem,
                    out BinItem binItem))
                {
                    loadItem = pickableItem;
                    loadItem.Pallet = binItem.Pallet;
                    loadItem.Status = LoadItemStatus.Picking;
                    loadItem.Crane = craneNumber;
                    slug[loadItem.NodeIndex] = loadItem;

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
                return !_storage.AnyPickable(
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

        // Must be called from within _LockAll()
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
                return _systemSettings.CanAutoCompactStorage(craneNumber)
                    && !_storage.Any(b => b.Audit && b.CraneNumber == craneNumber)
                    && !_upperPit.Values
                        .Concat(_lowerPit.Values)
                        .Any(p => p.PitCode == craneNumber.AssignedPitCode())
                    && (_TryCompactMismatchSizes(craneNumber)
                        || _TryCompactFarthestPallet(craneNumber)
                        || _TryCompactClosestEmptyBin(craneNumber));
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called within _LockAll()
        private bool _TryCompactMismatchSizes(CraneNumber craneNumber)
        {
            BinItem sourceBin = _storage
                .Reverse()
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && !b.Pallet.IsStack
                    && b.Pallet.Status == PalletStatus.OK
                    && b.Pallet.BinSize == BinSize.Small
                    && b.BinSize == BinSize.Large
                    && b.BinStatus == BinStatus.Pickable
                    && !b.Audit
                    && !b.Disabled
                    && !b.NotUsable);
            if (sourceBin == null)
            {
                return false;
            }
            BinItem emptyBin = _storage
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && b.BinSize == BinSize.Small
                    && b.BinStatus == BinStatus.Empty
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);
            if (emptyBin == null)
            {
                return false;
            }
            sourceBin.Audit = true;
            _storage[sourceBin.NodeIndex] = sourceBin;
            return true;
        }

        // Must be called within _LockAll()
        private bool _TryCompactFarthestPallet(CraneNumber craneNumber)
        {
            BinItem sourceBin = _storage
                .Reverse()
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && !b.Pallet.IsStack
                    && b.Pallet.Status == PalletStatus.OK
                    && b.BinStatus == BinStatus.Pickable
                    && b.BinSize == b.Pallet.BinSize
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
                    && b.BinSize == sourceBin.BinSize
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
        private bool _TryCompactClosestEmptyBin(CraneNumber craneNumber)
        {
            BinItem emptyBin = _storage
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && b.BinStatus == BinStatus.Empty
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);
            if (emptyBin == null)
            {
                return false;
            }
            BinItem sourceBin = _storage
                .Reverse()
                .FirstOrDefault(b => b.CraneNumber == craneNumber
                    && !b.Pallet.IsStack
                    && b.Pallet.Status == PalletStatus.OK
                    && b.NodeIndex > emptyBin.NodeIndex
                    && b.BinStatus == BinStatus.Pickable
                    && b.Pallet.BinSize == emptyBin.BinSize
                    && !b.Audit
                    && !b.PickOnly
                    && !b.Disabled
                    && !b.NotUsable);
            if (sourceBin == null)
            {
                return false;
            }
            sourceBin.Audit = true;
            _storage[sourceBin.NodeIndex] = sourceBin;
            return true;
        }

        public bool CanDoStore(
            OperationCode operationCode,
            CraneNumber craneNumber,
            Levels level,
            string inboundPalletID,
            out string fault)
        {
            if (!MesInterface.TryFetchPalletItem(
                operationCode,
                inboundPalletID,
                out PalletItem palletItem,
                out fault))
            {
                fault = $"Unknown Pallet {inboundPalletID} at {level.ToText()} Inbound of Crane {(int)craneNumber}";
                return false;
            }

            _LockAll();
            try
            {
                if (!inboundPalletID.ValidPalletID()
                    || !_systemSettings.CanDoStore(craneNumber))
                {
                    return false;
                }
                if (TryGetPitItem(level, inboundPalletID, out PitItem pitItem))
                {
                    palletItem = pitItem.Pallet;
                }
                fault = string.Empty;
                BinSize binSize = palletItem.BinSize;
                return palletItem.IsStack
                    ? _storage.IsStorableStackBinAvailable(craneNumber)
                    : _storage.IsStorableBinAvailable(craneNumber, binSize);
            }
            finally
            {
                _UnlockAll();
            }
        }






        //         public bool TryCompactStorage(CraneNumber craneNumber)
        //         {
        //             _LockAll();
        //             try
        //             {
        //                 if (!_systemSettings.CanAutoCompactStorage(craneNumber))
        //                 {
        //                     return false;
        //                 }
        //                 // Don't set Compact Audit if any existing Audits
        //                 if (_storage.Any(b =>
        //                     b.CraneNumber == craneNumber
        //                     && b.Audit
        //                     && !b.Disabled
        //                     && !b.NotUsable))
        //                 {
        //                     return false;
        //                 }
        //                 return _TryCompactStack(craneNumber)
        //                     || _TryCompactPallet(craneNumber);
        //             }
        //             finally
        //             {
        //                 _UnlockAll();
        //             }
        //         }
        // 
        //         // Must be called within _LockAll()
        //         private bool _TryCompactPallet(CraneNumber craneNumber)
        //         {
        //             BinItem sourceBin = _storage
        //                 .Reverse()
        //                 .FirstOrDefault(b => b.CraneNumber == craneNumber
        //                     && !b.Pallet.IsStack
        //                     && b.Pallet.Status == PalletStatus.OK
        //                     && b.BinStatus == BinStatus.Pickable
        //                     && !b.Audit
        //                     && !b.Disabled
        //                     && !b.NotUsable);
        //             if (sourceBin == null)
        //             {
        //                 return false;
        //             }
        //             BinItem emptyBins = _storage
        //                 .FirstOrDefault(b => b.CraneNumber == craneNumber
        //                     && b.NodeIndex < sourceBin.NodeIndex
        //                     && b.BinStatus == BinStatus.Empty
        //                     && !b.Audit
        //                     && !b.PickOnly
        //                     && !b.Disabled
        //                     && !b.NotUsable);
        //             if (emptyBins == null)
        //             {
        //                 return false;
        //             }
        //             sourceBin.Audit = true;
        //             _storage[sourceBin.NodeIndex] = sourceBin;
        //             return true;
        //         }
        // 
        //         // Must be called within _LockAll()
        //         private bool _TryCompactStack(CraneNumber craneNumber)
        //         {
        //             BinItem sourceBin = _storage
        //                 .FirstOrDefault(b => b.CraneNumber == craneNumber
        //                     && b.Pallet.IsStack
        //                     && b.Pallet.Status == PalletStatus.OK
        //                     && b.BinStatus == BinStatus.Pickable
        //                     && !b.Audit
        //                     && !b.Disabled
        //                     && !b.NotUsable);
        //             if (sourceBin == null)
        //             {
        //                 return false;
        //             }
        //             BinItem emptyBins = _storage
        //                 .Reverse()
        //                 .FirstOrDefault(b => b.CraneNumber == craneNumber
        //                     && b.NodeIndex > sourceBin.NodeIndex
        //                     && b.BinStatus == BinStatus.Empty
        //                     && !b.Audit
        //                     && !b.PickOnly
        //                     && !b.Disabled
        //                     && !b.NotUsable);
        //             if (emptyBins == null)
        //             {
        //                 return false;
        //             }
        //             sourceBin.Audit = true;
        //             _storage[sourceBin.NodeIndex] = sourceBin;
        //             return true;
        //         }

        #endregion

        //==================================================================================

        #region Load Director

        public bool ProcessPalletAtLoadDirector(
            OperationCode operationCode,
            Levels level,
            string palletID,
            out PalletItem palletItem,
            out LoadItem loadItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            throw new NotImplementedException("DataLayer.ProcessPalletAtLoadDirector");
        }

        #endregion

        //==================================================================================

        #region Recirc Router

        public bool ProcessPalletAtRecircRouter(
            OperationCode operationCode,
            Levels level,
            string palletID,
            out bool isLoadPallet,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            isLoadPallet = false;
            palletItem = null;
            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;

            if (!MesInterface.TryFetchPalletItem(
                operationCode,
                palletID,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                return false;
            }

            _LockAll();
            try
            {
                if (_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    isLoadPallet = true;
                    if (loadItem.Status != LoadItemStatus.Presequenced)
                    {
                        fault = $"({loadItem.Coordinates})  The Status of the Load Item for Pallet {loadItem.Pallet.PalletID} is {loadItem.Status.ToText()}. It should be {LoadItemStatus.Presequenced.ToText()}.";
                        return false;
                    }
                    if (slug.IsNextInLaneToSequence(loadItem.NodeIndex))
                    {
                        palletItem = loadItem.Pallet;
                        moveCommand = Constant.RecircRouterMoveCommandForward;
                        extendedState = $"({Constant.PalletTypeLoad})  Moving Pallet {palletItem.PalletID} forward to {loadItem.SlugLetter.SlugDisplayName()}";
                        return true;
                    }
                    else
                    {
                        palletItem = loadItem.Pallet;
                        moveCommand = Constant.RecircRouterMoveCommandToRecircBuffer;
                        extendedState = $"({Constant.PalletTypeLoad})  Diverting Pallet {palletItem.PalletID} to Recirc Buffer";
                        return true;
                    }
                }
                else if (TryGetPitItem(level, palletID, out PitItem pitItem))
                {
                    PitCode pitCode = pitItem.PitCode;
                    palletItem = pitItem.Pallet;
                    if (pitCode == PitCode.Purge)
                    {
                        moveCommand = Constant.RecircRouterMoveCommandForward;
                        extendedState = $"(PURGE) Moving {palletItem.PalletID} forward to Purge Operation.";
                        return true;
                    }
                    else if (pitCode == PitCode.Stack
                        && palletItem.IsStack)
                    {
                        moveCommand = Constant.RecircRouterMoveCommandForward;
                        string palletType = palletItem.Sku == Constant.StackSku1
                            ? Constant.PalletTypeStack1
                            : Constant.PalletTypeStack2;
                        string toPlace = level == Levels.Upper
                            ? "Pallet Stack Lane"
                            : "Purge Operation";
                        extendedState = $"({palletType}) Moving Stack {palletItem.PalletID} forward to {toPlace}.";
                        return true;
                    }
                    SetPitPallet(level, palletItem, PitCode.Purge);
                    moveCommand = Constant.RecircRouterMoveCommandForward;
                    extendedState = $"({Constant.PalletTypePurge}) Moving Pallet {palletItem.PalletID} to forward";
                    return true;
                }
                palletItem = fetchedPalletItem;
                SetPitPallet(level, palletItem, PitCode.Purge);
                moveCommand = Constant.RecircRouterMoveCommandForward;
                extendedState = $"(UNKNOWN) Moving Unknown Pallet {palletItem.PalletID} forward to Purge Operation.";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        #endregion

        //==================================================================================

        #region Recirc Buffer

        public bool ProcessPalletAtRecircBuffer(
            OperationCode operationCode,
            Levels level,
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            palletItem = null;
            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;

            if (!MesInterface.TryFetchPalletItem(
                operationCode,
                palletID,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                return false;
            }

            _LockAll();
            try
            {
                if (!_TryGetRecircPallet(
                    level,
                    palletID,
                    out palletItem))
                {
                    palletItem = fetchedPalletItem;
                    SetPitPallet(level, palletItem, PitCode.Purge);
                    moveCommand = Constant.RecircBufferMoveCommandRelease;
                    extendedState = $"(UNKNOWN) Removing Unknown Pallet {palletID} from Recirc Buffer.";
                    return true;
                }

                // palletItem is the pallet at RecircBuffer operation
                IEnumerable<PalletItem> recircPallets = level == Levels.Upper
                    ? _upperRecirc.Values
                    : _lowerRecirc.Values;
                foreach (PalletItem recircPallet in recircPallets)
                {
                    if (!_slugManager.TryGetSlugByPalletID(
                        recircPallet.PalletID,
                        out Slug slug,
                        out LoadItem loadItem))
                    {
                        SetPitPallet(level, recircPallet, PitCode.Purge);
                        moveCommand = Constant.RecircBufferMoveCommandRelease;
                        extendedState = palletItem.PalletID != recircPallet.PalletID
                            ? $"(RECIRC) Recirculating Pallet {palletID} to access UNKNOWN Pallet {recircPallet.PalletID} in Recirc Buffer."
                            : $"(UNKNOWN) Releasing Unknown Pallet {palletID}.";
                        return true;
                    }
                    if (loadItem.SlugLevel != level)
                    {
                        RollBackLoadPick(recircPallet.PalletID, false);
                        SetPitPallet(level, recircPallet, PitCode.Purge);
                        moveCommand = Constant.RecircBufferMoveCommandRelease;
                        extendedState = palletItem.PalletID != recircPallet.PalletID
                            ? $"(RECIRC) Recirculating Pallet {palletID} to access Pallet {recircPallet.PalletID} on wrong level in Recirc Buffer."
                            : $"(WRONG LEVEL) Releasing Wrong-Level Pallet {palletID}.";
                        return true;
                    }
                    if (slug.IsNextInLaneToSequence(loadItem.NextNodeIndex))
                    {
                        moveCommand = Constant.RecircBufferMoveCommandRelease;
                        extendedState = palletItem.PalletID != recircPallet.PalletID
                            ? $"(RECIRC) Recirculating Pallet {palletID} to access Next-in-Lane Pallet {recircPallet.PalletID}."
                            : $"(NEXT-IN-LANE) Releasing Next-in-Lane Pallet {palletID}.";
                        return true;
                    }
                }
                palletItem = null;
                moveCommand = Constant.NoMoveCommand;
                extendedState = string.Empty;
                return false;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _TryGetRecircPallet(
            Levels level,
            string palletID,
            out PalletItem palletItem)
        {
            return level == Levels.Upper
                ? _upperRecirc.TryGetItem(palletID, out palletItem)
                : _lowerRecirc.TryGetItem(palletID, out palletItem);
        }

        #endregion

        //==================================================================================

        #region Purge

        public bool ProcessPalletAtPurge(
            OperationCode operationCode,
            Levels level,
            string palletID,
            out PalletItem palletItem,
            out int moveCommand,
            out string extendedState,
            out string fault)
        {
            if (!MesInterface.TryFetchPalletItem(
                operationCode,
                palletID,
                out PalletItem fetchedPalletItem,
                out fault))
            {
                palletItem = null;
                moveCommand = Constant.NoMoveCommand;
                extendedState = string.Empty;
                return false;
            }

            _LockAll();
            try
            {
                if (_slugManager.TryGetSlugByPalletID(
                    palletID,
                    out Slug slug,
                    out LoadItem loadItem))
                {
                    palletItem = loadItem.Pallet;
                    moveCommand = Constant.PurgeMoveCommandForward;
                    extendedState = $"({Constant.PalletTypeLoad})  Moving Pallet {palletItem.PalletID} forward to {loadItem.SlugLetter.SlugDisplayName()}";
                    return true;
                }
                else if (TryGetPitItem(level, palletID, out PitItem pitItem))
                {
                    PitCode pitCode = pitItem.PitCode;
                    if (pitCode == PitCode.Stack)
                    {
                        if (level == Levels.Upper)
                        {
                            palletItem = pitItem.Pallet;
                            moveCommand = Constant.PurgeMoveCommandForward;
                            string palletType = palletItem.Sku == Constant.StackSku1
                                ? Constant.PalletTypeStack1
                                : Constant.PalletTypeStack2;
                            extendedState = $"({palletType}) Moving Stack {palletItem.PalletID} forward to Pallet Stack Lane";
                            return true;
                        }
                        else
                        {
                            palletItem = pitItem.Pallet;
                            SetPitPallet(level, palletItem, PitCode.Purge);
                            moveCommand = Constant.PurgeMoveCommandToPurgeLane;
                            extendedState = $"({Constant.PalletTypePurge}) Diverting {level.ToText()} Stack Pallet {palletItem.PalletID} to Purge Lane";
                            return true;
                        }
                    }
                    else if (pitCode == PitCode.Purge)
                    {
                        palletItem = pitItem.Pallet;
                        moveCommand = Constant.PurgeMoveCommandToPurgeLane;
                        extendedState = $"({Constant.PalletTypePurge}) Diverting Pallet {palletItem.PalletID} to Purge Lane";
                        return true;
                    }
                    else // Should be pitCode==Unknown
                    {
                        palletItem = pitItem.Pallet;
                        SetPitPallet(level, palletItem, PitCode.Purge);
                        moveCommand = Constant.PurgeMoveCommandToPurgeLane;
                        extendedState = $"({Constant.PalletTypePurge}) Diverting {pitCode.ToText()} Pallet {palletItem.PalletID} to Purge Lane";
                        return true;
                    }
                }
                palletItem = fetchedPalletItem;
                SetPitPallet(level, palletItem, PitCode.Purge);
                moveCommand = Constant.PurgeMoveCommandToPurgeLane;
                extendedState = $"(UNKNOWN) Diverting Unknown Pallet {palletItem.PalletID} to Purge Lane";
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

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
                        moveCommand = Constant.TransferStackMoveCommand;
                        extendedState = $"(STACK) Moving Stack {palletID} to Empty Pallet Lane.";
                        return true;
                    }
                    moveCommand = Constant.TransferFinalPurgeMoveCommand;
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
            Slug slug;
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

                _ = _slugManager.TryGetSlugByLetter(slugLetter, out slug);
                slug.SafeClear(true);
                slug.Touch();
            }
            finally
            {
                _UnlockAll();
            }
            IEnumerable<LoadItem> loadItems = slug
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
            _ = slug.Lock();
            slug.SafeClear(true);
            slug.Touch();
            slug.Unlock();
            return true;
        }

        // Must be called from within _LockAll()
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
                    now.ToString(Constant.LongDateTimeFormat24));

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
                            _ = command.ExecuteNonQuery();
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

        #region ShortageCalculator

        public void CalculateShortages()
        {
            bool touchPrimary = false;
            Slug primarySlug = null;
            bool touchSecondary = false;
            Slug secondarySlug = null;
            bool touchBroadcast = false;

            _LockAll();
            try
            {
//                 Dictionary<string, int> skuCounts = _storage.GetPickableSkuCounts(
//                     out List<string> reservedPalletIDs,
//                     out List<string> reservedJobIDs);

                Dictionary<string, int> skuCounts = _storage.GetPickableSkuCounts(
                    out List<PalletPickModeKeys> reservedPalletKeys);

                if (!_slugManager.TryGetPrimarySlug(
                    _systemSettings.GetItem(),
                    out primarySlug,
                    out secondarySlug))
                {
                    return;
                }

                IEnumerable<LoadItem> primaryItems = primarySlug.GetLoadInPickSearchOrder();
                foreach (LoadItem loadItem in primaryItems)
                {
                    bool shortage = false;
                    LoadItemStatus status = loadItem.Status;
                    if (status == LoadItemStatus.Waiting
                        || status == LoadItemStatus.Pending
                        || status == LoadItemStatus.Pickable)
                    {
                        BroadcastItem broadcast = loadItem.Broadcast;
                        if (broadcast.PickMode == PickMode.ByPalletID)
                        {
                            PalletPickModeKeys match = reservedPalletKeys
                                .FirstOrDefault(k => k.PalletID == broadcast.PickModeKey);
                            shortage = match == null;
                            if (!shortage)
                            {
                                _ = reservedPalletKeys.Remove(match);
                            }
                        }
                        else if (broadcast.PickMode == PickMode.ByJobID)
                        {
                            PalletPickModeKeys match = reservedPalletKeys
                                .FirstOrDefault(k => k.JobID == broadcast.PickModeKey);
                            shortage = match == null;
                            if (!shortage)
                            {
                                _ = reservedPalletKeys.Remove(match);
                            }
                        }
                        else
                        {
                            string sku = loadItem.Broadcast.Sku;
                            if (!skuCounts.TryGetValue(sku, out int count))
                            {
                                shortage = true;
                            }
                            else
                            {
                                if (--count < 0)
                                {
                                    shortage = true;
                                    _ = skuCounts.Remove(sku);
                                }
                                else
                                {
                                    skuCounts[sku] = count;
                                }
                            }
                        }
                    }
                    if (loadItem.Broadcast.Shortage != shortage)
                    {
                        BroadcastItem broadcast = loadItem.Broadcast;
                        broadcast.Shortage = shortage;
                        loadItem.Broadcast = broadcast;
                        _ = primarySlug.SetAt(loadItem.NodeIndex, loadItem, true);
                        touchPrimary = true;
                    }
                }

                IEnumerable<LoadItem> secondaryItems = secondarySlug.GetLoadInPickSearchOrder();
                foreach (LoadItem loadItem in secondaryItems)
                {
                    bool shortage = false;
                    LoadItemStatus status = loadItem.Status;
                    if (status == LoadItemStatus.Waiting
                        || status == LoadItemStatus.Pending
                        || status == LoadItemStatus.Pickable)
                    {
                        BroadcastItem broadcast = loadItem.Broadcast;
                        if (broadcast.PickMode == PickMode.ByPalletID)
                        {
                            PalletPickModeKeys match = reservedPalletKeys
                                .FirstOrDefault(k => k.PalletID == broadcast.PickModeKey);
                            shortage = match == null;
                            if (!shortage)
                            {
                                _ = reservedPalletKeys.Remove(match);
                            }
                        }
                        else if (broadcast.PickMode == PickMode.ByJobID)
                        {
                            PalletPickModeKeys match = reservedPalletKeys
                                .FirstOrDefault(k => k.JobID == broadcast.PickModeKey);
                            shortage = match == null;
                            if (!shortage)
                            {
                                _ = reservedPalletKeys.Remove(match);
                            }
                        }
                        else
                        {
                            string sku = loadItem.Broadcast.Sku;
                            if (!skuCounts.TryGetValue(sku, out int count))
                            {
                                shortage = true;
                            }
                            else
                            {
                                if (--count < 0)
                                {
                                    shortage = true;
                                    _ = skuCounts.Remove(sku);
                                }
                                else
                                {
                                    skuCounts[sku] = count;
                                }
                            }
                        }
                    }
                    if (loadItem.Broadcast.Shortage != shortage)
                    {
                        BroadcastItem broadcast = loadItem.Broadcast;
                        broadcast.Shortage = shortage;
                        loadItem.Broadcast = broadcast;
                        _ = secondarySlug.SetAt(loadItem.NodeIndex, loadItem, true);
                        touchSecondary = true;
                    }
                }

                List<BroadcastItem> broadcastItems = _broadcast
                    .GetCurrentBroadcastItems(
                        _systemSettings.LastCsnReleased,
                        _systemSettings.LargestRotationReceived);
                foreach (BroadcastItem broadcast in broadcastItems)
                {
                    bool shortage = false;

                    if (broadcast.PickMode == PickMode.ByPalletID)
                    {
                        PalletPickModeKeys match = reservedPalletKeys
                            .FirstOrDefault(k => k.PalletID == broadcast.PickModeKey);
                        shortage = match == null;
                        if (!shortage)
                        {
                            _ = reservedPalletKeys.Remove(match);
                        }
                    }
                    else if (broadcast.PickMode == PickMode.ByJobID)
                    {
                        PalletPickModeKeys match = reservedPalletKeys
                            .FirstOrDefault(k => k.JobID == broadcast.PickModeKey);
                        shortage = match == null;
                        if (!shortage)
                        {
                            _ = reservedPalletKeys.Remove(match);
                        }
                    }
                    else
                    {
                        string sku = broadcast.Sku;
                        if (!skuCounts.TryGetValue(sku, out int count))
                        {
                            shortage = true;
                        }
                        else
                        {
                            if (--count < 0)
                            {
                                shortage = true;
                                _ = skuCounts.Remove(sku);
                            }
                            else
                            {
                                skuCounts[sku] = count;
                            }
                        }
                    }
                    if (broadcast.Shortage != shortage)
                    {
                        broadcast.Shortage = shortage;
                        _ = _broadcast.Update(broadcast.Csn, broadcast, true);
                        touchBroadcast = true;
                    }
                }
            }
            finally
            {
                if (touchPrimary)
                {
                    primarySlug.Touch();
                }
                if (touchSecondary)
                {
                    secondarySlug.Touch();
                }
                if (touchBroadcast)
                {
                    _broadcast.Touch();
                }
                _UnlockAll();
            }
        }

        #endregion

        //==================================================================================

        #region  Command Service

        public bool TryCloseLoad(
            SlugLetter slugLetter,
            bool reopenLoad,
            out string error)
        {
            _LockAll();
            try
            {
                if (!_slugManager.TryGetSlugByLetter(slugLetter, out Slug slug))
                {
                    error = $"Unknown Slug Letter {slugLetter.Letter()}";
                    return false;
                }
                if (reopenLoad)
                {
                    for (int nodeIndex = 0; nodeIndex < Constant.LoadSize; nodeIndex++)
                    {
                        LoadItem loadItem = slug[nodeIndex];
                        if (loadItem.Status == LoadItemStatus.Invalid)
                        {
                            loadItem.Status = LoadItemStatus.Waiting;
                        }
                        _ = slug.SetAt(loadItem.NodeIndex, loadItem);
                    }
                }
                else
                {
                    for (int nodeIndex = 0; nodeIndex < Constant.LoadSize; nodeIndex++)
                    {
                        LoadItem loadItem = slug[nodeIndex];
                        if (loadItem.Status == LoadItemStatus.Waiting)
                        {
                            loadItem.Status = LoadItemStatus.Invalid;
                        }
                        _ = slug.SetAt(loadItem.NodeIndex, loadItem);
                    }
                }
                slug.Touch();
                error = null;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

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
        private bool _TryAcceptLoad(Slug load, out string error)
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
                out error))
            {
                return false;
            }

            if (!LA.LoadArchive.ArchiveLoadData(
                loadArchive,
                out error))
            {
                return false;
            }

            load.SetLoadable();
            return true;
        }

        public bool TryAutoReleaseBroadcast(out string error)
        {
            error = null;
            _LockAll();
            try
            {
                if (!_systemSettings.AutoReleaseBroadcastEnabled)
                {
                    return false;
                }

                if (!_slugManager.GetTargetSlugForBroadcastRelease(
                    _systemSettings.GetItem(),
                    out Slug targetSlug))
                {
                    return false;
                }

                List<BroadcastItem> releasableBroadcastItems = _broadcast.GetReleasableItems(
                    _systemSettings.LastCsnReleased,
                    _systemSettings.LargestRotationReceived);

                int waitingCount = targetSlug.Cleared
                    ? Constant.LoadSize
                    : targetSlug.WaitingCount;

                if (!_GetAutoCountToRelease(
                    releasableBroadcastItems.Count,
                    waitingCount,
                    out int countToRelease))
                {
                    return false;
                }
                // countToRelease will always be <= releasableBroadcastItems.Count
                if (!_ReleaseBroadcast(
                    true,
                    releasableBroadcastItems.Take(countToRelease).ToList(),
                    targetSlug,
                    out error))
                {
                    return false;
                }
                XSystemEvent.Publish(
                    "AutoReleaseBroadcast",
                    XSystemEventLevel.Information,
                    $"{countToRelease} Broadcast Records were automatically released to {targetSlug.SlugLetter.SlugDisplayName()}.");
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        public bool TryReleaseBroadcast(
            SlugLetter slugLetter,
            int countToRelease,
            out string error)
        {
            _LockAll();
            try
            {

                if (!_slugManager.TryGetSlugByLetter(slugLetter, out Slug targetSlug))
                {
                    error = string.Empty;
                    return false;
                }

                List<BroadcastItem> releasableBroadcastItems = _broadcast.GetReleasableItems(
                    _systemSettings.LastCsnReleased,
                    _systemSettings.LargestRotationReceived)
                    .Take(countToRelease)
                    .ToList();

                if (releasableBroadcastItems.Count < countToRelease)
                {
                    error = $"Unable to release {countToRelease} Broadcast Records. Only {releasableBroadcastItems.Count} records are releasable.";
                    return false;
                }

                if (!_ReleaseBroadcast(
                    false,
                    releasableBroadcastItems,
                    targetSlug,
                    out error))
                {
                    if (!error.IsNullOrWhiteSpace())
                    {
                        XSystemEvent.Publish(
                            "ReleaseBroadcast",
                            XSystemEventLevel.Error,
                            "Failed to Release Broadcast: " + error);
                    }
                }
                else
                {
                    XSystemEvent.Publish(
                        "ReleaseBroadcast",
                        XSystemEventLevel.Information,
                        $"{countToRelease} Broadcast Records manually released to {slugLetter.SlugDisplayName()}.");
                }
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private bool _ReleaseBroadcast(
            bool autoRelease,
            List<BroadcastItem> releasableBroadcastItems,
            Slug targetSlug,
            out string error)
        {
            if (releasableBroadcastItems.Count == 0)
            {
                error = autoRelease
                    ? string.Empty
                    : "There is no Releasable Broadcast!";
                return false;
            }

            bool touchCollections = false;
            _LockAll();
            try
            {
                bool firstReleaseToSlug = targetSlug.Cleared;
                string lastCsnReleased = _systemSettings.LastCsnReleased;
                int nodeIndex = firstReleaseToSlug
                    ? 0
                    : targetSlug.FirstWaitingNodeIndex;
                foreach (BroadcastItem broadcastItem in releasableBroadcastItems)
                {
                    LoadItem loadItem = targetSlug[nodeIndex];
                    loadItem.Broadcast = broadcastItem;
                    loadItem.Status = LoadItemStatus.Pending;
                    _ = targetSlug.SetAt(nodeIndex++, loadItem, true);

                    broadcastItem.Status = BroadcastStatus.Shipped;
                    _broadcast.Update(broadcastItem.Csn, broadcastItem, true);

                    lastCsnReleased = broadcastItem.Csn;

                    touchCollections = true;
                }
                for (int index = nodeIndex;  index < Constant.LoadSize; index++)
                {
                    LoadItem loadItem = targetSlug[nodeIndex];
                    loadItem.Status = LoadItemStatus.Waiting;
                    _ = targetSlug.SetAt(nodeIndex++, loadItem, true);
                }
                _systemSettings.LastCsnReleased = lastCsnReleased;

                if (targetSlug.SlugLetter == SlugLetter.A)
                {
                    _systemSettings.SlugALoadNumber = _systemSettings.NextLoadNumber;
                    if (firstReleaseToSlug)
                    {
                        _systemSettings.SlugALoadStartedOn = DateTime.Now;
                    }
                }
                else // has to be Slug B
                {
                    _systemSettings.SlugBLoadNumber = _systemSettings.NextLoadNumber;
                    if (firstReleaseToSlug)
                    {
                        _systemSettings.SlugBLoadStartedOn = DateTime.Now;
                    }
                }

//                 Slug otherSlug = _slugManager.GetOtherSlug(targetSlug);
//                 if (otherSlug.PresequencedOrGreater)
//                 {
//                     targetSlug.ApplyInitialPickableStatuses();
//                 }

                error = string.Empty;
                XMessaging.Publish(
                    Constant.CalculateShortagesMessageTopicName,
                    XMessageScopes.All);

                return true;
            }
            finally
            {
                if (touchCollections)
                {
                    targetSlug.Touch();
                    _broadcast.Touch();
                }
                _UnlockAll();
            }
        }

        private bool _GetAutoCountToRelease(
            int releasableBroadcastCount,
            int waitingCount,
            out int countToRelease)
        {
            countToRelease = 0;
            int[] releasableCounts = Utils.GetReleasableCounts(waitingCount);
            foreach (int releasableCount in releasableCounts)
            {
                if (releasableCount > releasableBroadcastCount)
                {
                    continue;
                }
                countToRelease = releasableCount;
            }
            return countToRelease > 0;
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
                .Where(l => l.Status > LoadItemStatus.Waiting)
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
                errorMessage = $"You are attempting to abort a load which was not the last load started. You must first abort the last load started.";
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
                _RecoverBroadcast(csnListToRecover);
            }
            _broadcast.Touch();
            load.Touch();
            XSystemEvent.Publish(
                load.CollectionName,
                XSystemEventLevel.Notification,
                "Load was aborted.");
            return true;
        }

        public bool RecoverBroadcast(
            RecoverBroadcastMessageData messageData,
            out string error)
        {
            _LockAll();
            try
            {
                List<string> csnListToRecover = _broadcast.Values
                    .Where(b =>
                        b.Csn.IsGreaterThan(_systemSettings.LastCsnReleased, true)
                        && b.Status == BroadcastStatus.Shipped)
                    .Select(b => b.Csn)
                    .ToList();
                if (csnListToRecover.Count == 0)
                {
                    error = $"There are no recoverable Broadcast records.";
                    return false;
                }
                _RecoverBroadcast(csnListToRecover);
                _broadcast.Touch();
                error = null;
                return true;
            }
            finally
            {
                _UnlockAll();
            }
        }

        // Must be called from within _LockAll()
        private void _RecoverBroadcast(List<string> csnListToRecover)
        {
            foreach (string csn in csnListToRecover)
            {
                if (_broadcast.TryGetItem(csn, out BroadcastItem broadcastItem))
                {
                    broadcastItem.Status = BroadcastStatus.OK;
                    _ = _broadcast.Update(csn, broadcastItem, true);
                }
            }
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
                    if (!newComment.IsNullOrWhiteSpace()
                        && (messageData.OverwriteComment
                            || palletItem.Comment.IsNullOrWhiteSpace()))
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

        #region MesInterface and Service

        public void ProcessStatusChangeRequests(
            IEnumerable<StatusChangeQueue> palletStatusChangeQueueEntries)
        {
            _LockAll();
            try
            {
                foreach (StatusChangeQueue entry in palletStatusChangeQueueEntries)
                {
                    StatusChange statusChange = entry.StatusChange;
                    if (_ChangeStoragePalletStatus(statusChange, out string error)
                        || _ChangePitPalletStatus(Levels.Upper, statusChange, out error)
                        || _ChangePitPalletStatus(Levels.Lower, statusChange, out error)
                        || _ChangeAssignmentPitPalletStatus(statusChange, out error))
                    {
                        entry.Error = null;
                        entry.Processed = true;
                        entry.ProcessedOn = DateTime.Now;
                    }
                    else
                    {
                        if (error != null)
                        {
                            XSystemEvent.Publish(
                                Constant.PalletStatusChangeEventContext,
                                XSystemEventLevel.Error,
                                error + $" Status Change ChangeID {statusChange.ChangeID}.");
                        }
                        else
                        {
                            error = $"Pallet for StatusChangeQueue Entry QueueID {entry.QueueID} was not found in Storage or PIT. Queue Entry not processed.";
                            XSystemEvent.Publish(
                                Constant.PalletStatusChangeEventContext,
                                XSystemEventLevel.Error,
                                error);
                            error = $"Pallet was not found in Storage or PIT. Queue Entry not processed.";
                        }
                        entry.Processed = false;
                        entry.ProcessedOn = DateTime.Now;
                        entry.Error = error;
                    }
                }
            }
            finally
            {
                _UnlockAll();
            }
        }

        private bool _ChangeStoragePalletStatus(
            StatusChange statusChange,
            out string error)
        {
            error = null; // has to default to null for db access reasons

            if ((!statusChange.JobID.IsNullOrWhiteSpace()
                    && _storage.TryFindBinByJobID(statusChange.JobID, out BinItem binItem))
                || (statusChange.PalletID.ValidPalletID()
                    && _storage.TryFindBinByPalletID(statusChange.PalletID, out binItem)))
            {
                // binItem != null is guaranteed here
                PalletItem palletItem = binItem.Pallet;
                PalletStatus newStatus = statusChange.PalletStatus;

//                 if (statusChange.PalletStatus == PalletStatus.Invalid
//                     || statusChange.PalletStatus == PalletStatus.Unknown)
//                 {
//                     error = $"Cannot set Status of Storage Pallet {palletItem.PalletID} to {statusChange.PalletStatus.ToText()}";
//                     return false;
//                 }

                if (palletItem.IsStack)
                {
                    error = $"StatusChange Change ID {statusChange.ChangeID}. Cannot change the Pallet Status of a Stack.";
                    return false;
                }
                if (newStatus != PalletStatus.Purge)
                {
                    if (newStatus == PalletStatus.Hold)
                    {
                        if (statusChange.HoldCode <= Constant.NoHoldCode)
                        {
                            error = $"Invalid Hold Code {statusChange.HoldCode}. StatusChange HeaderID {statusChange.ChangeID}.";
                            return false;
                        }
                        else
                        {
                            palletItem.HoldCode = statusChange.HoldCode;
                            //                         if (!statusChange.Comment.IsNullOrWhiteSpace())
                            //                         {
                            //                             palletItem.Comment = statusChange.Comment;
                            //                         }
                        }
                    }
                    else if (newStatus == PalletStatus.OK || newStatus == PalletStatus.Reserved)
                    {
                        palletItem.HoldCode = statusChange.HoldCode <= Constant.NoHoldCode
                            ? Constant.NoHoldCode
                            : statusChange.HoldCode;
                    }
                    else
                    {
                        error = $"StatusChange HeaderID {statusChange.ChangeID}. Cannot change Pallet Status to {(int)statusChange.PalletStatus}.";
                        return false;
                    }
                }
                palletItem.Status = newStatus;
                if (!statusChange.Comment.IsNullOrWhiteSpace())
                {
                    palletItem.Comment = statusChange.Comment;
                }
                binItem.Pallet = palletItem;
                _storage[binItem.NodeIndex] = binItem;
                return true;
            }
            error = null;
            return false;
        }

        private bool _ChangePitPalletStatus(
            Levels level,
            StatusChange statusChange,
            out string error)
        {
            error = null; // has to default to null for db access reasons

            if (level == Levels.None)
            {
                return false;
            }

            Pit pit = level == Levels.Upper
                ? _upperPit
                : (Pit)_lowerPit;

            if ((!statusChange.JobID.IsNullOrWhiteSpace()
                    && TryFindPitPalletByJobID(level, statusChange.JobID, out PitItem pitItem))
                || (statusChange.PalletID.ValidPalletID()
                    && pit.TryGetItem(statusChange.PalletID, out pitItem)))
            {
                // pitItem != null is guaranteed here
                PalletItem palletItem = pitItem.Pallet;
                if (palletItem.IsStack)
                {
                    error = $"Cannot change the Pallet Status of PIT Stack {palletItem.PalletID}.";
                    return false;
                }
                PalletStatus newStatus = statusChange.PalletStatus;
                if (newStatus == PalletStatus.Invalid
                    || newStatus == PalletStatus.Unknown)
                {
                    error = $"Cannot set Status of {level.ToText()} PIT Pallet {palletItem.PalletID} to {newStatus.ToText()}";
                    return false;
                }
                switch (pitItem.PitCode)
                {
                    case PitCode.Assigned1:
                    case PitCode.Assigned2:
                    case PitCode.Assigned3:
                    case PitCode.Assigned4:
                        if (newStatus == PalletStatus.Purge)
                        {
                            palletItem.Status = newStatus;
                            palletItem.HoldCode = Constant.NoHoldCode;
                            if (!statusChange.Comment.IsNullOrWhiteSpace())
                            {
                                palletItem.Comment = statusChange.Comment;
                            }
                            pit.Set(level, palletItem, PitCode.Purge);
                            return true;
                        }
                        else if (newStatus == PalletStatus.Hold)
                        {
                            if (statusChange.HoldCode <= Constant.NoHoldCode)
                            {
                                error = $"Invalid Hold Code {statusChange.HoldCode}. StatusChange HeaderID {statusChange.ChangeID}.";
                                return false;
                            }
                            else
                            {
                                palletItem.Status = newStatus;
                                palletItem.HoldCode = statusChange.HoldCode;
                                if (!statusChange.Comment.IsNullOrWhiteSpace())
                                {
                                    palletItem.Comment = statusChange.Comment;
                                }
                                pit.Set(level, palletItem, pitItem.PitCode);
                                return true;
                            }
                        }
                        else if (newStatus == PalletStatus.OK || newStatus == PalletStatus.Reserved)
                        {
                            palletItem.Status = newStatus;
                            palletItem.HoldCode = statusChange.HoldCode <= Constant.NoHoldCode
                                ? Constant.NoHoldCode
                                : statusChange.HoldCode;
                            if (!statusChange.Comment.IsNullOrWhiteSpace())
                            {
                                palletItem.Comment = statusChange.Comment;
                            }
                            pit.Set(level, palletItem, pitItem.PitCode);
                            return true;
                        }
                        break;
                    case PitCode.Stack:
                        error = $"Cannot change the Pallet Status of PIT Stack {palletItem.PalletID}.";
                        return false;
                    case PitCode.Purge:
                        if (newStatus == PalletStatus.Purge)
                        {
                            palletItem.Status = newStatus;
                            palletItem.HoldCode = Constant.NoHoldCode;
                            if (!statusChange.Comment.IsNullOrWhiteSpace())
                            {
                                palletItem.Comment = statusChange.Comment;
                            }
                            pit.Set(level, palletItem, PitCode.Purge);
                            return true;
                        }
                        error = $"Cannot change the Pallet Status of PIT Purge Pallet {palletItem.PalletID}.";
                        return false;
                    case PitCode.Upper:
                    case PitCode.Lower:
                    case PitCode.Twenty:
                    case PitCode.Unknown:
                    default:
                        palletItem.Status = PalletStatus.Purge;
                        palletItem.HoldCode = Constant.NoHoldCode;
                        if (!statusChange.Comment.IsNullOrWhiteSpace())
                        {
                            palletItem.Comment = statusChange.Comment;
                        }
                        pit.Set(level, palletItem, PitCode.Purge);
                        error = $"Pallet {palletItem.PalletID} with invalid PIT Code found in {level.ToText()} PIT.";
                        return false;
                }
            }
            error = null;
            return false;
        }

        private bool _ChangeAssignmentPitPalletStatus(
            StatusChange statusChange,
            out string error)
        {
            error = null; // has to default to null for db access reasons

            if ((!statusChange.JobID.IsNullOrWhiteSpace()
                    && TryFindAssignmentPitPalletByJobID(statusChange.JobID, out PitItem pitItem))
                || (!statusChange.PalletID.IsNullOrWhiteSpace()
                    && _assignmentPit.TryGetItem(statusChange.PalletID, out pitItem)))
            {
                // pitItem != null is guaranteed here
                PalletItem palletItem = pitItem.Pallet;
                if (palletItem.IsStack)
                {
                    error = $"Cannot change the Pallet Status of Assignment PIT Stack {palletItem.PalletID}.";
                    return false;
                }
                PalletStatus newStatus = statusChange.PalletStatus;
                if (newStatus == PalletStatus.Invalid
                    || newStatus == PalletStatus.Unknown)
                {
                    error = $"Cannot set Status of Assignment PIT Pallet {palletItem.PalletID} to {newStatus.ToText()}";
                    return false;
                }
                switch (pitItem.PitCode)
                {
                    case PitCode.Upper:
                    case PitCode.Lower:
                    case PitCode.Twenty:
                        if (newStatus == PalletStatus.Purge)
                        {
                            palletItem.Status = newStatus;
                            palletItem.HoldCode = statusChange.HoldCode <= Constant.NoHoldCode
                                ? Constant.NoHoldCode
                                : statusChange.HoldCode;
                            if (!statusChange.Comment.IsNullOrWhiteSpace())
                            {
                                palletItem.Comment = statusChange.Comment;
                            }
                            _assignmentPit.Set(Levels.None, palletItem, pitItem.PitCode);
                            return true;
                        }
                        else if (newStatus == PalletStatus.Hold)
                        {
                            if (statusChange.HoldCode <= Constant.NoHoldCode)
                            {
                                error = $"Cannot set Status to Hold with invalid Hold Code value {statusChange.HoldCode}.";
                                return false;
                            }
                            else
                            {
                                palletItem.Status = newStatus;
                                palletItem.HoldCode = statusChange.HoldCode;
                                if (!statusChange.Comment.IsNullOrWhiteSpace())
                                {
                                    palletItem.Comment = statusChange.Comment;
                                }
                                _assignmentPit.Set(Levels.None, palletItem, pitItem.PitCode);
                                return true;
                            }
                        }
                        else if (newStatus == PalletStatus.OK || newStatus == PalletStatus.Reserved)
                        {
                            palletItem.Status = newStatus;
                            palletItem.HoldCode = statusChange.HoldCode <= Constant.NoHoldCode
                                ? Constant.NoHoldCode
                                : statusChange.HoldCode;
                            if (!statusChange.Comment.IsNullOrWhiteSpace())
                            {
                                palletItem.Comment = statusChange.Comment;
                            }
                            _assignmentPit.Set(Levels.None, palletItem, pitItem.PitCode);
                            return true;
                        }
                        break;
                    case PitCode.Assigned1:
                    case PitCode.Assigned2:
                    case PitCode.Assigned3:
                    case PitCode.Assigned4:
                    case PitCode.Unknown:
                    default:
                        palletItem.Status = PalletStatus.Purge;
                        palletItem.HoldCode = Constant.NoHoldCode;
                        if (!statusChange.Comment.IsNullOrWhiteSpace())
                        {
                            palletItem.Comment = statusChange.Comment;
                        }
                        RemovePitPallet(palletItem.PalletID);
                        _assignmentPit.Set(Levels.None, palletItem, PitCode.Lower);
                        error = $"Pallet {palletItem.PalletID} with invalid PIT Code found in Assignment PIT.";
                        return false;
                }
            }
            error = null;
            return false;
        }

        #endregion

    }
}
