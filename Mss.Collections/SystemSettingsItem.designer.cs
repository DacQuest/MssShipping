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

//         [XDataItemProperty(
//            Comment = "A load will be release to the Preferred Slug if it is clear and the Load definition specifies No Preference.")]
//         public LoadLetter PreferredLoad
//         {
//             get => GetEnum<LoadLetter>(nameof(PreferredLoad));
//             set => SetEnum(nameof(PreferredLoad), value);
//         }

        [XDataItemProperty(
           Comment = ".")]
        public SlugLetter PreferredSlug
        {
            get => GetEnum<SlugLetter>(nameof(PreferredSlug));
            set => SetEnum(nameof(PreferredSlug), value);
        }

//         [XDataItemProperty(
//            Comment = ".")]
//         public bool AutoReleaseBroadcastEnabled
//         {
//             get => GetBoolean(nameof(AutoReleaseBroadcastEnabled));
//             set => SetBoolean(nameof(AutoReleaseBroadcastEnabled), value);
//         }

        [XDataItemProperty(
           Comment = ".")]
        public bool AutoAcceptLoadsEnabled
        {
            get => GetBoolean(nameof(AutoAcceptLoadsEnabled));
            set => SetBoolean(nameof(AutoAcceptLoadsEnabled), value);
        }

        [XDataItemProperty(
           Comment = ".")]
        public bool ForceMesPalletDataQueryOnAudit
        {
            get => GetBoolean(nameof(ForceMesPalletDataQueryOnAudit));
            set => SetBoolean(nameof(ForceMesPalletDataQueryOnAudit), value);
        }

        [XDataItemProperty(
            Comment = ".")]
        public FifoMode FifoMode
        {
            get => GetEnum<FifoMode>(nameof(FifoMode));
            set => SetEnum(nameof(FifoMode), value);
        }

//         [XDataItemProperty(
//             Comment = ".")]
//         public LoadDirectorMode LoadDirectorMode
//         {
//             get => GetEnum<LoadDirectorMode>(nameof(LoadDirectorMode));
//             set => SetEnum(nameof(LoadDirectorMode), value);
//         }

        [XDataItemProperty(
            Comment = "The largest Rotation Number received.")]
        public int LargestRotationReceived
        {
            get => GetInt32(nameof(LargestRotationReceived));
            set => SetInt32(nameof(LargestRotationReceived), value);
        }

        [XDataItemProperty(
            Comment = "The last CSN released to a load for picking.",
            MaxLength = Constant.CsnLength)]
        public string LastCsnReleased
        {
            get => GetString(nameof(LastCsnReleased));
            set => SetString(nameof(LastCsnReleased), value);
        }

        [XDataItemProperty(
           Comment = ".")]
        public SlugPickPriority SlugPickPriority
        {
            get => GetEnum<SlugPickPriority>(nameof(SlugPickPriority));
            set => SetEnum(nameof(SlugPickPriority), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether Loads can be released to Load A. Also used to pause picking to Load A.")]
        public bool SlugAEnabled
        {
            get => GetBoolean(nameof(SlugAEnabled));
            set => SetBoolean(nameof(SlugAEnabled), value);
        }

        [XDataItemProperty(
            Comment = "The number of the current Load A.")]
        public int SlugALoadNumber
        {
            get => GetInt32(nameof(SlugALoadNumber));
            set => SetInt32(nameof(SlugALoadNumber), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load A was released for picking.")]
        public DateTime SlugALoadStartedOn
        {
            get => GetDateTime(nameof(SlugALoadStartedOn));
            set => SetDateTime(nameof(SlugALoadStartedOn), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load A was completed.")]
        public DateTime SlugALoadCompletedOn
        {
            get => GetDateTime(nameof(SlugALoadCompletedOn));
            set => SetDateTime(nameof(SlugALoadCompletedOn), value);
        }

        [XDataItemProperty(
            Comment = "Determines whether Loads can be released to Load B. Also used to pause picking to Load B.")]
        public bool SlugBEnabled
        {
            get => GetBoolean(nameof(SlugBEnabled));
            set => SetBoolean(nameof(SlugBEnabled), value);
        }

        [XDataItemProperty(
            Comment = "The number of the current Load B.")]
        public int SlugBLoadNumber
        {
            get => GetInt32(nameof(SlugBLoadNumber));
            set => SetInt32(nameof(SlugBLoadNumber), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load B was released for picking.")]
        public DateTime SlugBLoadStartedOn
        {
            get => GetDateTime(nameof(SlugBLoadStartedOn));
            set => SetDateTime(nameof(SlugBLoadStartedOn), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the current Load B was completed.")]
        public DateTime SlugBLoadCompletedOn
        {
            get => GetDateTime(nameof(SlugBLoadCompletedOn));
            set => SetDateTime(nameof(SlugBLoadCompletedOn), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool UpperLevelInboundEnabled
        {
            get => GetBoolean(nameof(UpperLevelInboundEnabled));
            set => SetBoolean(nameof(UpperLevelInboundEnabled), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool LowerLevelInboundEnabled
        {
            get => GetBoolean(nameof(LowerLevelInboundEnabled));
            set => SetBoolean(nameof(LowerLevelInboundEnabled), value);
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
           Comment = "Prioritizes Audit Picks over other crane functions.",
            ArrayLength = Constant.MaxCranes + 1,
            MirrorToChildTable = true)]
        public bool[] PrioritizeAuditPicks
        {
            get => GetBooleanArray(nameof(PrioritizeAuditPicks));
            set => SetBooleanArray(nameof(PrioritizeAuditPicks), value);
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
