using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class BinItem
    {
        public BinItem()
        {
            SetByteConverter<BinItem>();
            StoredOn = Constant.BeforeBeginningOfTime;
            LastAuditAttemptedOn = Constant.BeforeBeginningOfTime;

        }

    //    [XDataItemProperty(
    //Comment = "",
    //ReadOnlyInDataItemGrid = true)]
    //    public BinSize BinSize
    //    {
    //        get => GetEnum<BinSize>(nameof(BinSize));
    //        set => SetEnum<BinSize>(nameof(BinSize), value);
    //    }

        [XDataItemProperty(
    Comment = "The status of the bin.")]
        public BinStatus BinStatus
        {
            get => GetEnum<BinStatus>(nameof(BinStatus));
            set => SetEnum<BinStatus>(nameof(BinStatus), value);
        }

        [XDataItemProperty(
            Comment = "Indicates whether this bin needs to be audited.")]
        public bool Audit
        {
            get => GetBoolean(nameof(Audit));
            set => SetBoolean(nameof(Audit), value);
        }

        [XDataItemProperty(
            Comment = "Indicates whether or not this bin is usable.",
            ReadOnlyInDataItemGrid = true)]
        public bool NotUsable
        {
            get => GetBoolean(nameof(NotUsable));
            set => SetBoolean(nameof(NotUsable), value);
        }

        [XDataItemProperty(
            Comment = "Indicates whether this bin is disabled.")]
        public bool Disabled
        {
            get => GetBoolean(nameof(Disabled));
            set => SetBoolean(nameof(Disabled), value);
        }

        [XDataItemProperty(
            Comment = "Indicates whether this bin can only be picked from.")]
        public bool PickOnly
        {
            get => GetBoolean(nameof(PickOnly));
            set => SetBoolean(nameof(PickOnly), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the pallet was stored to the bin.")]
        public DateTime StoredOn
        {
            get => GetDateTime(nameof(StoredOn));
            set => SetDateTime(nameof(StoredOn), value);
        }

        [XDataItemProperty(
            Comment = "The number of times the crane has unsuccessfully auited this bin.")]
        public int AuditAttempts
        {
            get => GetInt32(nameof(AuditAttempts));
            set => SetInt32(nameof(AuditAttempts), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp of the last failed Audit attempt.")]
        public DateTime LastAuditAttemptedOn
        {
            get => GetDateTime(nameof(LastAuditAttemptedOn));
            set => SetDateTime(nameof(LastAuditAttemptedOn), value);
        }

        [XDataItemProperty(
            Comment = "Pallet Item representing the pallet stored in this bin.",
            MirrorToChildTable = true)]
        public PalletItem Pallet
        {
            get => GetDataItem<PalletItem>(nameof(Pallet));
            set => SetDataItem(nameof(Pallet), value);
        }











        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.

    }
}
