using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Collections
{
    public class SystemSettings : XSharedSingleItem<SystemSettingsItem>
    {
        public bool ForcePalletDataMesQueryOnAudit
        {
            get => GetItemProperty<bool>(nameof(SystemSettingsItem.ForcePalletDataMesQueryOnAudit));
            set => SetItemProperty(nameof(SystemSettingsItem.ForcePalletDataMesQueryOnAudit), value);
        }

        public FifoMode FifoMode
        {
            get => GetItemProperty<FifoMode>(nameof(SystemSettingsItem.FifoMode));
            set => SetItemProperty(nameof(SystemSettingsItem.FifoMode), value);
        }

        public LoadPickPriority LoadPickPriority
        {
            get => GetItemProperty<LoadPickPriority>(nameof(SystemSettingsItem.LoadPickPriority));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadPickPriority), value);
        }

        // Load A
        public bool LoadAEnabled
        {
            get => GetItemProperty<bool>(nameof(SystemSettingsItem.LoadAEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadAEnabled), value);
        }

        public int LoadALoadID
        {
            get => GetItemProperty<int>(nameof(SystemSettingsItem.LoadALoadID));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadALoadID), value);
        }

        public DateTime LoadALoadStartedOn
        {
            get => GetItemProperty<DateTime>(nameof(SystemSettingsItem.LoadALoadStartedOn));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadALoadStartedOn), value);
        }

        public DateTime LoadALoadCompletedOn
        {
            get => GetItemProperty<DateTime>(nameof(SystemSettingsItem.LoadALoadCompletedOn));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadALoadCompletedOn), value);
        }

        // Load B
        public bool LoadBEnabled
        {
            get => GetItemProperty<bool>(nameof(SystemSettingsItem.LoadBEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadBEnabled), value);
        }

        public int LoadBLoadID
        {
            get => GetItemProperty<int>(nameof(SystemSettingsItem.LoadBLoadID));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadBLoadID), value);
        }

        public DateTime LoadBLoadStartedOn
        {
            get => GetItemProperty<DateTime>(nameof(SystemSettingsItem.LoadBLoadStartedOn));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadBLoadStartedOn), value);
        }

        public DateTime LoadBLoadCompletedOn
        {
            get => GetItemProperty<DateTime>(nameof(SystemSettingsItem.LoadBLoadCompletedOn));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadBLoadCompletedOn), value);
        }

        // Lower Inbounds
        public bool MasterLowerInboundsEnabled
        {
            get => IsLowerInboundEnabled(CraneNumber.None);
            set => SetLowerInboundEnabled(CraneNumber.None, value);
        }

        public bool IsLowerInboundEnabled(CraneNumber craneNumber)
            => GetItem().LowerInboundsEnabled[craneNumber.Index()];

        public void SetLowerInboundEnabled(CraneNumber craneNumber, bool value)
        {
            SetItemArrayPropertyElement(nameof(SystemSettingsItem.LowerInboundsEnabled), (int)craneNumber, value);
        }

        public bool[] LowerInboundsEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.LowerInboundsEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.LowerInboundsEnabled), value);
        }

        public bool CanRouteToLowerInbound(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return IsLowerInboundEnabled(craneNumber)
                    && MasterLowerInboundsEnabled
                    && CanDoStore(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

//         public bool LowerInboundAcceptsPallets(CraneNumber craneNumber)
//             => IsLowerInboundEnabled(craneNumber)
//                 && MasterLowerInboundsEnabled
//                 && CanDoStore(craneNumber);

        // Upper Inbounds
        public bool MasterUpperInboundsEnabled
        {
            get => IsUpperInboundEnabled(CraneNumber.None);
            set => SetUpperInboundEnabled(CraneNumber.None, value);
        }

        public bool IsUpperInboundEnabled(CraneNumber craneNumber)
            => GetItem().UpperInboundsEnabled[craneNumber.Index()];

        public void SetUpperInboundEnabled(CraneNumber craneNumber, bool value)
        {
            SetItemArrayPropertyElement(nameof(SystemSettingsItem.UpperInboundsEnabled), (int)craneNumber, value);
        }

        public bool[] UpperInboundsEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.UpperInboundsEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.UpperInboundsEnabled), value);
        }

        public bool CanRouteToUpperInbound(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return IsUpperInboundEnabled(craneNumber)
                    && MasterUpperInboundsEnabled
                    && CanDoStore(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

//         public bool UpperInboundAcceptsPallets(CraneNumber craneNumber)
//             => IsUpperInboundEnabled(craneNumber)
//                 && MasterUpperInboundsEnabled
//                 && CanDoStore(craneNumber);

        // Lower Outbounds
        public bool MasterLowerOutboundsEnabled
        {
            get => GetLowerOutboundEnabled(CraneNumber.None);
            set => SetLowerOutboundEnabled(CraneNumber.None, value);
        }

        public bool GetLowerOutboundEnabled(CraneNumber craneNumber)
        {
            return GetItem().LowerOutboundsEnabled[(int)craneNumber];
        }

        public void SetLowerOutboundEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.LowerOutboundsEnabled;
                array[(int)craneNumber] = value;
                item.LowerOutboundsEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] LowerOutboundsEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.LowerOutboundsEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.LowerOutboundsEnabled), value);
        }

        public bool LowerOutboundEnabled(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return MasterLowerOutboundsEnabled && GetLowerOutboundEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        // Upper Outbounds
        public bool MasterUpperOutboundsEnabled
        {
            get => GetUpperOutboundEnabled(CraneNumber.None);
            set => SetUpperOutboundEnabled(CraneNumber.None, value);
        }

        public bool GetUpperOutboundEnabled(CraneNumber craneNumber)
        {
            return GetItem().UpperOutboundsEnabled[(int)craneNumber];
        }

        public void SetUpperOutboundEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.UpperOutboundsEnabled;
                array[(int)craneNumber] = value;
                item.UpperOutboundsEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] UpperOutboundsEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.UpperOutboundsEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.UpperOutboundsEnabled), value);
        }

        public bool UpperOutboundEnabled(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return MasterUpperOutboundsEnabled && GetUpperOutboundEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        // Stores
        public bool MasterStoresEnabled
        {
            get => GetStoresEnabled(CraneNumber.None);
            set => SetStoresEnabled(CraneNumber.None, value);
        }

        public bool GetStoresEnabled(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return GetItem().StoresEnabled[(int)craneNumber];
            }
            finally
            {
                Unlock();
            }

        }
        public void SetStoresEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.StoresEnabled;
                array[(int)craneNumber] = value;
                item.StoresEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] StoresEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.StoresEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.StoresEnabled), value);
        }

        public bool CanDoStore(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return IsCraneInAutoMode(craneNumber)
                    && MasterStoresEnabled
                    && GetStoresEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        // Load Picks
        public bool MasterLoadPicksEnabled
        {
            get => GetLoadPicksEnabled(CraneNumber.None);
            set => SetLoadPicksEnabled(CraneNumber.None, value);
        }

        public bool GetLoadPicksEnabled(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return GetItem().LoadPicksEnabled[(int)craneNumber];
            }
            finally
            {
                Unlock();
            }

        }

        public void SetLoadPicksEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.LoadPicksEnabled;
                array[(int)craneNumber] = value;
                item.LoadPicksEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] LoadPicksEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.LoadPicksEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.LoadPicksEnabled), value);
        }

        public bool CanDoLoadPick(CraneNumber craneNumber, Levels level)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof(level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _ = Lock();
            try
            {
                return IsCraneInAutoMode(craneNumber)
                    && MasterLoadPicksEnabled
                    && GetLoadPicksEnabled(craneNumber)
                    && level == Levels.Lower
                        ? LowerOutboundEnabled(craneNumber)
                        : UpperOutboundEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        // Stack Picks
        public bool MasterStackPicksEnabled
        {
            get => GetStackPicksEnabled(CraneNumber.None);
            set => SetStackPicksEnabled(CraneNumber.None, value);
        }

        public bool GetStackPicksEnabled(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return GetItem().StackPicksEnabled[(int)craneNumber];
            }
            finally
            {
                Unlock();
            }

        }

        public void SetStackPicksEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.StackPicksEnabled;
                array[(int)craneNumber] = value;
                item.StackPicksEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] StackPicksEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.StackPicksEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.StackPicksEnabled), value);
        }

        public bool CanDoStackPicks(CraneNumber craneNumber, Levels level)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof(level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _ = Lock();
            try
            {
                return IsCraneInAutoMode(craneNumber)
                    && MasterStackPicksEnabled
                    && GetStackPicksEnabled(craneNumber)
                    && level == Levels.Lower
                        ? LowerOutboundEnabled(craneNumber)
                        : UpperOutboundEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }


        // Purge Picks
        public bool MasterPurgePicksEnabled
        {
            get => GetPurgePicksEnabled(CraneNumber.None);
            set => SetPurgePicksEnabled(CraneNumber.None, value);
        }

        public bool GetPurgePicksEnabled(CraneNumber craneNumber)
        {
            return GetItem().PurgePicksEnabled[(int)craneNumber];
        }

        public void SetPurgePicksEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.PurgePicksEnabled;
                array[(int)craneNumber] = value;
                item.PurgePicksEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] PurgePicksEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.PurgePicksEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.PurgePicksEnabled), value);
        }

        public bool CanDoPurgePicks(CraneNumber craneNumber, Levels level)
        {
            XArgumentChecker.ThrowIfNotContainedIn(
                level,
                nameof(level),
                new Levels[] { Levels.Lower, Levels.Upper });

            _ = Lock();
            try
            {
                return IsCraneInAutoMode(craneNumber)
                    && MasterPurgePicksEnabled
                    && GetPurgePicksEnabled(craneNumber)
                    && level == Levels.Lower
                        ? LowerOutboundEnabled(craneNumber)
                        : UpperOutboundEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }


        // Auto Compact
        public bool MasterAutoCompactStorageEnabled
        {
            get => IsAutoCompactStorageEnabled(CraneNumber.None);
            set => SetAutoCompactStorageEnabled(CraneNumber.None, value);
        }

        public bool IsAutoCompactStorageEnabled(CraneNumber craneNumber)
            => GetItem().AutoCompactStorageEnabled[craneNumber.Index()];

        public void SetAutoCompactStorageEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.AutoCompactStorageEnabled;
                array[craneNumber.Index()] = value;
                item.AutoCompactStorageEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] AutoCompactStorageEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.AutoCompactStorageEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.AutoCompactStorageEnabled), value);
        }

        public bool CanAutoCompactStorage(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return CanDoAuditPick(craneNumber)
                    && MasterAutoCompactStorageEnabled
                    && IsAutoCompactStorageEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        // Audit Picks
        public bool MasterAuditPicksEnabled
        {
            get => IsAuditPicksEnabled(CraneNumber.None);
            set => SetAuditPicksEnabled(CraneNumber.None, value);
        }

        public bool IsAuditPicksEnabled(CraneNumber craneNumber)
            => GetItem().AuditPicksEnabled[craneNumber.Index()];

        public void SetAuditPicksEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.AuditPicksEnabled;
                array[craneNumber.Index()] = value;
                item.AuditPicksEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] AuditPicksEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.AuditPicksEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.AuditPicksEnabled), value);
        }

        public bool CanDoAuditPick(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return IsCraneInAutoMode(craneNumber)
                    && MasterAuditPicksEnabled
                    && IsAuditPicksEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        // Crane Telemetry
        public bool MasterCraneTelemetryEnabled
        {
            get => GetCraneTelemetryEnabled(CraneNumber.None);
            set => SetCraneTelemetryEnabled(CraneNumber.None, value);
        }

        public bool GetCraneTelemetryEnabled(CraneNumber craneNumber)

        {
            return GetItem().CraneTelemetryEnabled[(int)craneNumber];
        }

        public void SetCraneTelemetryEnabled(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.CraneTelemetryEnabled;
                array[(int)craneNumber] = value;
                item.CraneTelemetryEnabled = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }
        public bool CanGenerateCraneTelemetry(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return MasterCraneTelemetryEnabled
                    && GetCraneTelemetryEnabled(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

        public bool[] CraneTelemetryEnabled
        {
            get => GetItemArrayProperty<bool>(nameof(SystemSettingsItem.CraneTelemetryEnabled));
            set => SetItemProperty(nameof(SystemSettingsItem.CraneTelemetryEnabled), value);
        }

        // Crane Mode
        public CraneMode[] CraneMode => GetItem().CraneModes;

        public bool IsCraneInAutoMode(CraneNumber craneNumber)
            => GetItemArrayPropertyElement<CraneMode>(
                nameof(SystemSettingsItem.CraneModes),
                craneNumber.Index() - 1) == Common.CraneMode.Auto;

        public CraneNumber[] AutoModeCranes
        {
            get
            {
                _ = Lock();
                try
                {
                    List<CraneNumber> autoModeCranes = new List<CraneNumber>();
                    for (CraneNumber crane = CraneNumber.Crane1; crane <= CraneNumber.Crane2; crane++)
                    {
                        if (CraneMode[((int)crane) - 1] == Common.CraneMode.Auto)
                        {
                            autoModeCranes.Add(crane);
                        }
                    }
                    return autoModeCranes.ToArray();
                }
                finally
                {
                    Unlock();
                }
            }
        }

        public void SetCraneMode(CraneNumber craneNumber, CraneMode mode)
            => SetItemArrayPropertyElement(
                nameof(SystemSettingsItem.CraneModes),
                craneNumber.Index() - 1,
                mode);

        // Prioritized Audit Picks
        public bool MasterPrioritizeAuditPicks
        {
            get => GetPrioritizeAuditPicks(CraneNumber.None);
            set => SetPrioritizeAuditPicks(CraneNumber.None, value);
        }

        public bool GetPrioritizeAuditPicks(CraneNumber craneNumber)
            => GetItem().PrioritizeAuditPicks[craneNumber.Index()];

        public void SetPrioritizeAuditPicks(CraneNumber craneNumber, bool value)
        {
            _ = Lock();
            try
            {
                SystemSettingsItem item = GetItem();
                bool[] array = item.PrioritizeAuditPicks;
                array[craneNumber.Index()] = value;
                item.PrioritizeAuditPicks = array;
                SetItem(item);
            }
            finally
            {
                Unlock();
            }
        }
        public bool CanPrioritizeAuditPicks(CraneNumber craneNumber)
        {
            _ = Lock();
            try
            {
                return MasterPrioritizeAuditPicks
                    && GetPrioritizeAuditPicks(craneNumber);
            }
            finally
            {
                Unlock();
            }
        }

    }
}
