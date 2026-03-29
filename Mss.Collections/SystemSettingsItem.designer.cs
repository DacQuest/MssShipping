using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class SystemSettingsItem
    {
        public SystemSettingsItem()
        {
            SetByteConverter<SystemSettingsItem>();
        }

        [XDataItemProperty(
           Comment = ".")]
        public bool ForcePalletDataMesQueryOnAudit
        {
            get => GetBoolean(nameof(ForcePalletDataMesQueryOnAudit));
            set => SetBoolean(nameof(ForcePalletDataMesQueryOnAudit), value);
        }

        [XDataItemProperty(
            Comment = ".")]
        public FifoMode FifoMode
        {
            get => GetEnum<FifoMode>(nameof(FifoMode));
            set => SetEnum(nameof(FifoMode), value);
        }

        [XDataItemProperty(
            Comment = "The largest Rotation Number received.")]
        public int LargestRotationReceived
        {
            get => GetInt32(nameof(LargestRotationReceived));
            set => SetInt32(nameof(LargestRotationReceived), value);
        }

        [XDataItemProperty(
            Comment = "The last CSN released to a load for picking.")]
        public string LastCsnReleased
        {
            get => GetString(nameof(LastCsnReleased));
            set => SetString(nameof(LastCsnReleased), value);
        }

        [XDataItemProperty(
           Comment = "Prioritizes Audit Picks over other crane functions.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] PrioritizeAuditPicks
        {
            get => GetBooleanArray(nameof(PrioritizeAuditPicks));
            set => SetBooleanArray(nameof(PrioritizeAuditPicks), value);
        }

        [XDataItemProperty(
           Comment = ".")]
        public LoadPickPriority LoadPickPriority
        {
            get => GetEnum<LoadPickPriority>(nameof(LoadPickPriority));
            set => SetEnum(nameof(LoadPickPriority), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether Loads can be released to Load A. Also used to pause picking to Load A.")]
        public bool LoadAEnabled
        {
            get => GetBoolean(nameof(LoadAEnabled));
            set => SetBoolean(nameof(LoadAEnabled), value);
        }

        [XDataItemProperty(
            Comment = "The Load ID of the current Load A.")]
        public int LoadALoadID
        {
            get => GetInt32(nameof(LoadALoadID));
            set => SetInt32(nameof(LoadALoadID), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load A was released for picking.")]
        public DateTime LoadALoadStartedOn
        {
            get => GetDateTime(nameof(LoadALoadStartedOn));
            set => SetDateTime(nameof(LoadALoadStartedOn), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load A was completed.")]
        public DateTime LoadALoadCompletedOn
        {
            get => GetDateTime(nameof(LoadALoadCompletedOn));
            set => SetDateTime(nameof(LoadALoadCompletedOn), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether Loads can be released to Load B. Also used to pause picking to Load B.")]
        public bool LoadBEnabled
        {
            get => GetBoolean(nameof(LoadBEnabled));
            set => SetBoolean(nameof(LoadBEnabled), value);
        }

        [XDataItemProperty(
            Comment = "The Load ID of the current Load B.")]
        public int LoadBLoadID
        {
            get => GetInt32(nameof(LoadBLoadID));
            set => SetInt32(nameof(LoadBLoadID), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load B was released for picking.")]
        public DateTime LoadBLoadStartedOn
        {
            get => GetDateTime(nameof(LoadBLoadStartedOn));
            set => SetDateTime(nameof(LoadBLoadStartedOn), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load B was completed.")]
        public DateTime LoadBLoadCompletedOn
        {
            get => GetDateTime(nameof(LoadBLoadCompletedOn));
            set => SetDateTime(nameof(LoadBLoadCompletedOn), value);
        }




        [XDataItemProperty(
            Comment = "Determines whether Crane Lower Inbounds will accept pallets.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] LowerInboundsEnabled
        {
            get => GetBooleanArray(nameof(LowerInboundsEnabled));
            set => SetBooleanArray(nameof(LowerInboundsEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether Crane Upper Inbounds will accept pallets.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] UpperInboundsEnabled
        {
            get => GetBooleanArray(nameof(UpperInboundsEnabled));
            set => SetBooleanArray(nameof(UpperInboundsEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can pick to Outbounds.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] LowerOutboundsEnabled
        {
            get => GetBooleanArray(nameof(LowerOutboundsEnabled));
            set => SetBooleanArray(nameof(LowerOutboundsEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can pick to Outbounds.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] UpperOutboundsEnabled
        {
            get => GetBooleanArray(nameof(UpperOutboundsEnabled));
            set => SetBooleanArray(nameof(UpperOutboundsEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can do Load Picks.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] LoadPicksEnabled
        {
            get => GetBooleanArray(nameof(SystemSettings.LoadPicksEnabled));
            set => SetBooleanArray(nameof(SystemSettings.LoadPicksEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can do Stack Picks.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] StackPicksEnabled
        {
            get => GetBooleanArray(nameof(SystemSettings.StackPicksEnabled));
            set => SetBooleanArray(nameof(SystemSettings.StackPicksEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can do Q/A Picks.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] PurgePicksEnabled
        {
            get => GetBooleanArray(nameof(PurgePicksEnabled));
            set => SetBooleanArray(nameof(PurgePicksEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can do Audit Picks.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] AuditPicksEnabled
        {
            get => GetBooleanArray(nameof(AuditPicksEnabled));
            set => SetBooleanArray(nameof(AuditPicksEnabled), value);
        }

        [XDataItemProperty(
           Comment = ".",
           ArrayLength = Constant.MaxCranes + 1,
           MirrorToChildTable = true)]
        public bool[] AutoCompactStorageEnabled
        {
            get => GetBooleanArray(nameof(AutoCompactStorageEnabled));
            set => SetBooleanArray(nameof(AutoCompactStorageEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes will produce telemetry data.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] CraneTelemetryEnabled
        {
            get => GetBooleanArray(nameof(CraneTelemetryEnabled));
            set => SetBooleanArray(nameof(CraneTelemetryEnabled), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether the Cranes can Store pallets.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] StoresEnabled
        {
            get => GetBooleanArray(nameof(StoresEnabled));
            set => SetBooleanArray(nameof(StoresEnabled), value);
        }

        [XDataItemProperty(
           Comment = "Reflects whether Cranes are in Auto Mode.",
           ArrayLength = Constant.MaxCranes,
           IsMirrored = false,
           ReadOnlyInDataItemGrid = true)]
        public CraneMode[] CraneModes
        {
            get => GetEnumArray<CraneMode>(nameof(CraneModes));
            set => SetEnumArray(nameof(CraneModes), value);
        }


    }
}
