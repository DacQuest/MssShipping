using DacQuest.DFX.Core.DataItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mss.Common;


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

        [XDataItemProperty(
            Comment = "")]
        public BinStatus BinStatus
        {
            get => GetEnum<BinStatus>(nameof(BinStatus));
            set => SetEnum<BinStatus>(nameof(BinStatus), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool Audit
        {
            get => GetBoolean(nameof(Audit));
            set => SetBoolean(nameof(Audit), value);
        }

        [XDataItemProperty(
            Comment = "",
            ReadOnlyInDataItemGrid = true)]
        public bool NotUsable
        {
            get => GetBoolean(nameof(NotUsable));
            set => SetBoolean(nameof(NotUsable), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool Disabled
        {
            get => GetBoolean(nameof(Disabled));
            set => SetBoolean(nameof(Disabled), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool PickOnly
        {
            get => GetBoolean(nameof(PickOnly));
            set => SetBoolean(nameof(PickOnly), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public DateTime StoredOn
        {
            get => GetDateTime(nameof(StoredOn));
            set => SetDateTime(nameof(StoredOn), value);
        }

        [XDataItemProperty(
            Comment = "")]
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
            Comment = "",
            MirrorToChildTable = true)]
        public PalletItem Pallet
        {
            get => GetDataItem<PalletItem>(nameof(Pallet));
            set => SetDataItem(nameof(Pallet), value);
        }

    }
}
