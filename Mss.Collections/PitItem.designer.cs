using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class PitItem
    {
        public PitItem()
        {
            SetByteConverter<PitItem>();
            SetOn = Constant.BeforeBeginningOfTime;
        }

        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.


        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.PalletIDLength)]
        public int PalletID
        {
            get => GetInt32(nameof(PalletID));
            set => SetInt32(nameof(PalletID), value);
        }


        [XDataItemProperty(
            Comment = "")]
        public DateTime SetOn
        {
            get => GetDateTime(nameof(SetOn));
            set => SetDateTime(nameof(SetOn), value);
        }

        [XDataItemProperty(
           Comment = "")]
        public PalletItem Pallet
        {
            get => GetDataItem<PalletItem>(nameof(Pallet));
            set => SetDataItem(nameof(Pallet), value);
        }

        [XDataItemProperty(
         Comment = "")]
        public PitCode PitCode
        {
            get => GetEnum<PitCode>(nameof(PitCode));
            set => SetEnum(nameof(PitCode), value);
        }


    }
}
