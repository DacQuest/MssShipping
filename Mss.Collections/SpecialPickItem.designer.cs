using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class SpecialPickItem
    {
        public SpecialPickItem()
        {
            SetByteConverter<SpecialPickItem>();
        }
        [XDataItemProperty(
    Comment = "The VIN of this Special Pick.",
    MaxLength = Constant.CsnLength)]
        public string Vin
        {
            get => GetString(nameof(Vin));
            set => SetString(nameof(Vin), value);
        }

        //         [XDataItemProperty(
        //             Comment = "Not Used.",
        //             MaxLength = Constant.CsnLength)]
        //         public string Csn
        //         {
        //             get => GetString(nameof(Csn));
        //             set => SetString(nameof(Csn), value);
        //         }

        [XDataItemProperty(
            Comment = "The Front Pick Mode for this Special Pick.")]
        public PickMode FrontPickMode
        {
            get => GetEnum<PickMode>(nameof(FrontPickMode));
            set => SetEnum(nameof(FrontPickMode), value);
        }

        [XDataItemProperty(
            Comment = "The Pallet ID or Job ID used to select the desired front pallet from Storage.")]
        public int FrontPickModeKey
        {
            get => GetInt32(nameof(FrontPickModeKey));
            set => SetInt32(nameof(FrontPickModeKey), value);
        }

        [XDataItemProperty(
            Comment = "The Rear Pick Mode for this Special Pick.")]
        public PickMode RearPickMode
        {
            get => GetEnum<PickMode>(nameof(RearPickMode));
            set => SetEnum(nameof(RearPickMode), value);
        }

        [XDataItemProperty(
            Comment = "The Pallet ID or Job ID used to select the desired rear pallet from Storage.")]
        public int RearPickModeKey
        {
            get => GetInt32(nameof(RearPickModeKey));
            set => SetInt32(nameof(RearPickModeKey), value);
        }

    }


    //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.

}

