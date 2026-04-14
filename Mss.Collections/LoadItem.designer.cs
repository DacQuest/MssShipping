using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class LoadItem
    {
        public LoadItem()
        {
            SetByteConverter<LoadItem>();
            PickedOn = Constant.BeforeBeginningOfTime;
        }

        [XDataItemProperty(
            Comment = "The current status of this Load Item.")]
        public LoadItemStatus Status
        {
            get => GetEnum<LoadItemStatus>(nameof(Status));
            set => SetEnum(nameof(Status), value);
        }

        [XDataItemProperty(
           Comment = "")]
        public CraneNumber Crane
        {
            get => GetEnum<CraneNumber>(nameof(Crane));
            set => SetEnum(nameof(Crane), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public DateTime PickedOn
        {
            get => GetDateTime(nameof(PickedOn));
            set => SetDateTime(nameof(PickedOn), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool Transferring
        {
            get => GetBoolean(nameof(Transferring));
            set => SetBoolean(nameof(Transferring), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool Shortage
        {
            get => GetBoolean(nameof(Shortage));
            set => SetBoolean(nameof(Shortage), value);
        }

        [XDataItemProperty(
            Comment = "The letter of the Load (A=1 or B=2).",
            ReadOnlyInDataItemGrid = true)]
        public SlugLetter SlugLetter
        {
            get => GetEnum<SlugLetter>(nameof(SlugLetter));
            // DO NOT SET SlugLetter IN CODE!
            set
            {
                if (SlugLetter > SlugLetter.None)
                {
                    throw new InvalidOperationException("Cannot set LoadItem.SlugLetter in code!");
                }
                SetEnum(nameof(SlugLetter), value);
            }
        }

//         [XDataItemProperty(
//             Comment = ".")]
//         public PickMode PickMode
//         {
//             get => GetEnum<PickMode>(nameof(PickMode));
//             set => SetEnum(nameof(PickMode), value);
//         }
// 
//         [XDataItemProperty(
//             Comment = ".",
//             MaxLength = Constant.PickModeValueLength)]
//         public string PickModeValue
//         {
//             get => GetString(nameof(PickModeValue));
//             set => SetString(nameof(PickModeValue), value);
//         }
// 
//         [XDataItemProperty(
//             Comment = ".",
//             MaxLength = Constant.PalletIDLength)]
//         public string PickModePalletID
//         {
//             get => GetString(nameof(PickModePalletID));
//             set => SetString(nameof(PickModePalletID), value);
//         }
// 
//         [XDataItemProperty(
//             Comment = ".")]
//         public int PickModeJobID
//         {
//             get => GetInt32(nameof(PickModeJobID));
//             set => SetInt32(nameof(PickModeJobID), value);
//         }

        [XDataItemProperty(
            Comment = "The Broadcast Item providing the requirements for this Load Item.",
            MirrorToChildTable = true)]
        public BroadcastItem Broadcast
        {
            get => GetDataItem<BroadcastItem>(nameof(Broadcast));
            set => SetDataItem(nameof(Broadcast), value);
        }

        [XDataItemProperty(
            Comment = "The Pallet Item representing the pallet that fulfills this Load requirement.",
            MirrorToChildTable = true)]
        public PalletItem Pallet
        {
            get => GetDataItem<PalletItem>(nameof(Pallet));
            set => SetDataItem(nameof(Pallet), value);
        }

    }
}
