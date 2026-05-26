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

        [XDataItemProperty(
            Comment = "The status of this Broadcast.")]
        public BroadcastStatus Status
        {
            get => GetEnum<BroadcastStatus>(nameof(Status));
            set => SetEnum(nameof(Status), value);
        }

//         [XDataItemProperty(
//             Comment = "The Rotation Number of the Broadcast.")]
//         public int RotationNumber
//         {
//             get => GetInt32(nameof(RotationNumber));
//             set => SetInt32(nameof(RotationNumber), value);
//         }

        [XDataItemProperty(
            Comment = "The Sequence Number of this Broadcast.",
            MaxLength = Constant.CsnLength)]
        public string Csn
        {
            get => GetString(nameof(Csn));
            set => SetString(nameof(Csn), value);
        }

        [XDataItemProperty(
            Comment = "The SKU that pertains to the entire vehicle.",
            MaxLength = Constant.MaxSkuLength)]
        public string VehicleSku
        {
            get => GetString(nameof(VehicleSku));
            set => SetString(nameof(VehicleSku), value);
        }

        [XDataItemProperty(
            Comment = "The SKU required to fulfill this Broadcast requirement.",
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
        public PickMode 
            PickMode
        {
            get => GetEnum<PickMode>(nameof(PickMode));
            set => SetEnum(nameof(PickMode), value);
        }

        [XDataItemProperty(
            Comment = "Used to pick pallet based on Pick Mode other than BySku",
            MaxLength = Constant.PickModeKeyLength)]
        public string PickModeKey
        {
            get => GetString(nameof(PickModeKey));
            set => SetString(nameof(PickModeKey), value);
        }

        [XDataItemProperty(
            Comment = "The timestamp when the Broadcast was received from the MES.")]
        public DateTime ReceivedOn
        {
            get => GetDateTime(nameof(ReceivedOn));
            set => SetDateTime(nameof(ReceivedOn), value);
        }

        [XDataItemProperty(
            Comment = "Number of Vehicle Rows associated with this Broadcast.")]
        public int VehicleRowCount
        {
            get => GetInt32(nameof(VehicleRowCount));
            set => SetInt32(nameof(VehicleRowCount), value);
        }

        [XDataItemProperty(
            Comment = "Indicates whether or not the SKU associated with this broacast is available.")]
        public bool Shortage
        {
            get => GetBoolean(nameof(Shortage));
            set => SetBoolean(nameof(Shortage), value);
        }

        //         [XDataItemProperty(
        //             Comment = ".")]
        //         public int InternalSequenceNumber
        //         {
        //             get => GetInt32(nameof(InternalSequenceNumber));
        //             set => SetInt32(nameof(InternalSequenceNumber), value);
        //         }

    }
}
