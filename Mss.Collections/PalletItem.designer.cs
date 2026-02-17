using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class PalletItem
    {
        public PalletItem()
        {
            SetByteConverter<PalletItem>();
        }

        [XDataItemProperty(
           Comment = "",
           MaxLength = Constant.PalletIDLength)]
        public string PalletID
        {
            get => GetString(nameof(PalletID));
            set => SetString(nameof(PalletID), value);
        }


        [XDataItemProperty(
            Comment = "The current status of this Pallet Item.")]
        public PalletStatus Status
        {
            get { return GetEnum<PalletStatus>("Status"); }
            set { SetEnum("Status", value); }
        }

        [XDataItemProperty(
            Comment = "The current Hold Code applied to this Pallet Item. A value of zero means no hold is applied.")]
        public Byte HoldCode
        {
            get { return GetByte("HoldCode"); }
            set { SetByte("HoldCode", value); }
        }

        [XDataItemProperty(
            Comment = "The MES-generated Job ID for the finished goods represented by this Pallet Item.")]
        public Int32 JobID
        {
            get { return GetInt32("JobID"); }
            set { SetInt32("JobID", value); }
        }

        [XDataItemProperty(
            Comment = "The timestamp when this Pallet Item was received from the MES.")]
        public DateTime ReceivedOn
        {
            get { return GetDateTime("ReceivedOn"); }
            set { SetDateTime("ReceivedOn", value); }
        }

        [XDataItemProperty(
    Comment = "The length of the longest SKU.",
    MaxLength = Constant.MaxSkuLength)]
        public string Sku
        {
            get => GetString(nameof(Sku));
            set => SetString(nameof(Sku), value);
        }

        [XDataItemProperty(
    Comment = "A comment regarding this Pallet.",
    MaxLength = Constant.CommentLength)]
        public string Comment
        {
            get => GetString(nameof(Comment));
            set => SetString(nameof(Comment), value);
        }

        [XDataItemProperty(
    Comment = "The timestamp when this Pallet Item was received from the MES.")]
        public DateTime BuiltOn
        {
            get => GetDateTime(nameof(BuiltOn));
            set => SetDateTime(nameof(BuiltOn), value);
        }




    }
}
