using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class BroadcastItem
    {
        public BroadcastItem()
        {
            SetByteConverter<BroadcastItem>();
            ReceivedOn = Constant.BeginningOfTime;
        }

        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.

        [XDataItemProperty(
            Comment = "The status of this Broadcast.")]
        public BroadcastStatus Status
        {
            get => GetEnum<BroadcastStatus>(nameof(Status));
            set => SetEnum(nameof(Status), value);
        }

        [XDataItemProperty(
            Comment = "The Sequence Number of the Broadcast.",
            MaxLength = Constant.CsnLength)]
        public string Csn
        {
            get => GetString(nameof(Csn));
            set => SetString(nameof(Csn), value);
        }

        [XDataItemProperty(
            Comment = "The SKU required to fulfill the Broadcast requirement.",
            MaxLength = Constant.MaxSkuLength)]
        public string Sku
        {
            get => GetString(nameof(Sku));
            set => SetString(nameof(Sku), value);
        }

        [XDataItemProperty(
            Comment = "The VIN.",
            MaxLength = Constant.VinLength)]
        public string Vin
        {
            get => GetString(nameof(Vin));
            set => SetString(nameof(Vin), value);
        }

        [XDataItemProperty(
            Comment = "The Pick Mode of this Broadcast.")]
        public PickMode PickMode
        {
            get => GetEnum<PickMode>(nameof(PickMode));
            set => SetEnum(nameof(PickMode), value);
        }

        [XDataItemProperty(
            Comment = ".")]
        public int PickModeJobID
        {
            get => GetInt32(nameof(PickModeJobID));
            set => SetInt32(nameof(PickModeJobID), value);
        }

        [XDataItemProperty(
            Comment = ".",
            MaxLength = Constant.PalletIDLength)]
        public string PickModePalletID
        {
            get => GetString(nameof(PickModePalletID));
            set => SetString(nameof(PickModePalletID), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the Broadcast was received from the MES.")]
        public DateTime ReceivedOn
        {
            get => GetDateTime(nameof(ReceivedOn));
            set => SetDateTime(nameof(ReceivedOn), value);
        }

        [XDataItemProperty(
            Comment = ".")]
        public int InternalSequenceNumber
        {
            get => GetInt32(nameof(InternalSequenceNumber));
            set => SetInt32(nameof(InternalSequenceNumber), value);
        }

//         [XDataItemProperty(
//             Comment = ".",
//             MaxLength = Constant.Row2ConsolePartLength)]
//         public string Row2ConsolePart
//         {
//             get => GetString(nameof(Row2ConsolePart));
//             set => SetString(nameof(Row2ConsolePart), value);
//         }

    }
}
