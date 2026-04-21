using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Services;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using Mss.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Services
{
    public class MesInterfaceService : XService
    {

        private const int _commandFetchBroadcast = FirstCustomCommand + 1;
        private const int _commandFetchHoldCodes = FirstCustomCommand + 2;
        private const int _commandProcessPalletStatusChanges = FirstCustomCommand + 3;

        private MesInterfaceServiceParameterSetWrapper _parameters;

        private Storage _storage;
        private AssignmentPit _assignmentPit;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private HoldCodes _holdCodes;
        private LowerRecircBuffer _lowerRecircBuffer;
        private UpperRecircBuffer _upperRecircBuffer;
        private SlugA _slugA;
        private SlugB _slugB;

        protected DataLayer DataLayer
        {
            get; private set;
        }

        private long _broadcastTimerID = 0;
        private long _holdCodesTimerID = 0;
        private long _statusChangeTimerID = 0;

        protected override bool OnStart()
        {

            DataLayer = DataLayer.Create(
                out _storage,
                out _assignmentPit,
                out _lowerPit,
                out _upperPit,
                out _systemSettings,
                out _broadcast,
                out _holdCodes,
                out _lowerRecircBuffer,
                out _upperRecircBuffer,
                out _slugA,
                out _slugB);

            // There is no need to call SetServiceStatus() when overriding
            RegisterCustomCommand(_commandFetchBroadcast, "Fetch Broadcast Now");
            RegisterCustomCommand(_commandFetchHoldCodes, "Fetch Hold Codes Now");
            RegisterCustomCommand(_commandProcessPalletStatusChanges, "Process Status Changes Now");

            if (_parameters.FetchBroadcastOnStartUp)
            {
                _FetchBroadcast();
            }
            if (_parameters.FetchHoldCodesOnStartUp)
            {
                _FetchHoldCodes();
            }
            if (_parameters.ProcessPalletStatusChangesOnStartUp)
            {
                _ProcessPalletStatusChanges();
            }

            _StartAllPolling();

            return true;
        }

        protected override int OnStop()
        {
            // There is no need to call SetServiceStatus() when overriding
            _StopAllPolling();
            return ExitCode;
        }

        protected override bool ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (MesInterfaceServiceParameterSetWrapper)parameters;
            return true;
        }

        private void _StartAllPolling()
        {
            _StartBroadcastPollingTimer();
            _StartHoldCodesPollingTimer();
            _StartStatusChangePollingTimer();
        }

        private void _StopAllPolling()
        {
            _StopBroadcastPollingTimer();
            _StopHoldCodesPollingTimer();
            _StopStatusChangePollingTimer();
        }

        protected override void OnCustomCommand(int customCommand)
        {
            // Remember to call SetServiceStatus() as necessary when overriding
            switch (customCommand)
            {
                case _commandFetchBroadcast:
                    _StopBroadcastPollingTimer();
                    _FetchBroadcast();
                    _StartBroadcastPollingTimer();
                    break;
                case _commandFetchHoldCodes:
                    _StopHoldCodesPollingTimer();
                    _FetchHoldCodes();
                    _StartHoldCodesPollingTimer();
                    break;
                case _commandProcessPalletStatusChanges:
                    _StopStatusChangePollingTimer();
                    _ProcessPalletStatusChanges();
                    _StartStatusChangePollingTimer();
                    break;
            }
        }

        private void _StartBroadcastPollingTimer()
        {
            _StopBroadcastPollingTimer();
            if (_parameters.BroadcastPollingPeriodSeconds > 0)
            {
                _broadcastTimerID = StartTimer(_BroadcastPollingTimer_Expired, _parameters.BroadcastPollingPeriodSeconds * 1000, null);
            }
        }

        private void _StopBroadcastPollingTimer()
        {
            if (_broadcastTimerID > 0)
            {
                StopTimer(_broadcastTimerID);
                _broadcastTimerID = 0;
            }
        }

        private void _BroadcastPollingTimer_Expired(XTimerEventArgs e)
        {
            _broadcastTimerID = 0;
            _FetchBroadcast();
            _StartBroadcastPollingTimer();
        }

        private void _FetchBroadcast()
        {
            try
            {
                int maxBroadcastNumbersToFetch = Constant.LoadSize;

                // GetItem() locks SystemSettings
                SystemSettingsItem systemSettingsItem = _systemSettings.GetItem();

                if (!MesInterface.TryFetchBroadcast(
                    maxBroadcastNumbersToFetch,
                    systemSettingsItem.LastCsnReleased,
                    systemSettingsItem.LargestRotationReceived,
                    out List<BroadcastItem> broadcastItems))
                {
                    XSystemEvent.Publish(
                        $"{ConfigurationItem.Name}.{nameof(_FetchBroadcast)}",
                        XSystemEventLevel.Error,
                        "Failed to fetch Broadcast from MES");
                }
                DataLayer.ReceiveBroadcast(broadcastItems);
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(_FetchBroadcast));
            }
        }

        private void _StartHoldCodesPollingTimer()
        {
            _StopHoldCodesPollingTimer();
            if (_parameters.HoldCodesPollingPeriodSeconds > 0)
            {
                _holdCodesTimerID = StartTimer(
                    _HoldCodesPollingTimer_Expired,
                    _parameters.HoldCodesPollingPeriodSeconds * 1000, null);
            }
        }

        private void _StopHoldCodesPollingTimer()
        {
            if (_holdCodesTimerID > 0)
            {
                StopTimer(_holdCodesTimerID);
                _holdCodesTimerID = 0;
            }
        }

        private void _HoldCodesPollingTimer_Expired(XTimerEventArgs e)
        {
            _holdCodesTimerID = 0;
            _FetchHoldCodes();
            _StartHoldCodesPollingTimer();
        }

        private void _FetchHoldCodes()
        {
            try
            {
                if (!MesInterface.TryFetchHoldCodes(out List<HoldCodeItem> holdCodes))
                {
                    XSystemEvent.Publish(
                        $"{ConfigurationItem.Name}.{nameof(_FetchHoldCodes)}",
                        XSystemEventLevel.Warning,
                        "Failed to fetch Hold Codes from MES");
                }
                DataLayer.ReceiveHoldCodes(holdCodes);
            }
            catch (Exception x)
            {
                x.PublishSystemEvent($"{ConfigurationItem.Name}.{nameof(_FetchHoldCodes)}");
            }
        }

        private void _StartStatusChangePollingTimer()
        {
            _StopStatusChangePollingTimer();
            if (_parameters.StatusChangePollingPeriodSeconds > 0)
            {
                _statusChangeTimerID = StartTimer(
                    _StatusChangePollingTimer_Expired,
                    _parameters.StatusChangePollingPeriodSeconds * 1000, null);
            }
        }

        private void _StopStatusChangePollingTimer()
        {
            if (_statusChangeTimerID > 0)
            {
                StopTimer(_statusChangeTimerID);
                _statusChangeTimerID = 0;
            }
        }

        private void _StatusChangePollingTimer_Expired(XTimerEventArgs e)
        {
            _statusChangeTimerID = 0;
            _ProcessPalletStatusChanges();
            _StartStatusChangePollingTimer();
        }

        private void _ProcessPalletStatusChanges()
        {
            if (MesInterface.TryFetchPendingStatusChangeRequests(
                out IEnumerable<StatusChangeQueue> pendingRequests))
            {
                DataLayer.ProcessStatusChangeRequests(pendingRequests);
                MesInterface.UpdateProcessedStatusChangeRequests(pendingRequests);
            }
        }


//         private void _FetchSkus()
//         {
//             if (!MesQuery.FetchSkus(out List<SkuItem> skus))
//             {
//                 XSystemEvent.Publish(
//                     $"{ConfigurationItem.Name}._FetchSkus()",
//                     XSystemEventLevel.Warning,
//                     "Failed to fetch SKUs from MES");
//                 return;
//             }
// 
//             bool oldInhibitSetting = false;
//             _skus.Lock();
//             try
//             {
//                 oldInhibitSetting = _skus.InhibitChangeNotifications;
//                 _skus.InhibitChangeNotifications = true;
//                 _skus.RemoveAll();
//                 for (int index = 0;index < skus.Count;index++)
//                 {
//                     SkuItem sku = skus[index];
//                     _skus[sku.Sku] = sku;
//                 }
//             }
//             finally
//             {
//                 _skus.InhibitChangeNotifications = oldInhibitSetting;
//                 _skus.Touch();
//                 _skus.Unlock();
//             }
//         }

//         private void _FetchBroadcast()
//         {
//             bool oldFrontInhibitSetting = false;
//             bool oldRearInhibitSetting = false;
//             try
//             {
//                 _frontBroadcast.Lock();
//                 _rearBroadcast.Lock();
//                 _systemSettings.Lock();
//                 oldFrontInhibitSetting = _frontBroadcast.InhibitChangeNotifications;
//                 oldRearInhibitSetting = _rearBroadcast.InhibitChangeNotifications;
//                 _frontBroadcast.InhibitChangeNotifications = true;
//                 _rearBroadcast.InhibitChangeNotifications = true;
//                 _frontBroadcast.PurgeOldBroadcast();
//                 _rearBroadcast.PurgeOldBroadcast();
// 
//                 int largestBroadcastReceived = _systemSettings.LargestBroadcastNumberReceived;
//                 if (!MesQuery.FetchBroadcast(
//                     _frontBroadcast.AvailableCapacity,
//                     _systemSettings.LastRearBroadcastNumberReleased,
//                     ref largestBroadcastReceived,
//                     out List<BroadcastItem> broadcastItems))
//                 {
//                     return;
//                 }
//                 if (broadcastItems.Count() == 0)
//                 {
//                     return;
//                 }
//                 foreach (BroadcastItem broadcastItem in broadcastItems)
//                 {
//                     if (broadcastItem.VehicleRow == VehicleRow.Row1)
//                     {
//                         _frontBroadcast.Update(broadcastItem.Csn, broadcastItem);
//                     }
//                     else
//                     {
//                         _rearBroadcast.Update(broadcastItem.Csn, broadcastItem);
//                     }
//                 }
//                 _systemSettings.LargestBroadcastNumberReceived = largestBroadcastReceived;
//             }
//             finally
//             {
//                 _rearBroadcast.InhibitChangeNotifications = oldRearInhibitSetting;
//                 _frontBroadcast.InhibitChangeNotifications = oldFrontInhibitSetting;
//                 _rearBroadcast.Touch();
//                 _frontBroadcast.Touch();
//                 _systemSettings.Unlock();
//                 _rearBroadcast.Unlock();
//                 _frontBroadcast.Unlock();
//             }
//         }

        protected override void DoDispose()
        {
            if (DataLayer != null)
            {
                DataLayer.Dispose();
                DataLayer = null;
            }

            base.DoDispose();
        }
    }
}
