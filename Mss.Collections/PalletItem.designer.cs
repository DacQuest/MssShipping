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
            get => GetEnum<PalletStatus>("Status");
            set => SetEnum("Status", value);
        }

        [XDataItemProperty(
            Comment = "The current Hold Code applied to this Pallet Item. A value of zero means no hold is applied.")]
        public int HoldCode
        {
            get => GetInt32("HoldCode");
            set => SetInt32("HoldCode", value);
        }

        [XDataItemProperty(
            Comment = "The MES-generated Job ID for the finished goods represented by this Pallet Item.")]
        public int JobID
        {
            get => GetInt32("JobID");
            set => SetInt32("JobID", value);
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
    Comment = "The timestamp when the seats on this Pallet were built.")]
        public DateTime BuiltOn
        {
            get => GetDateTime(nameof(BuiltOn));
            set => SetDateTime(nameof(BuiltOn), value);
        }




    }
}
