using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    partial class HoldCodeItem
    {
        public HoldCodeItem()
        {
            SetByteConverter<HoldCodeItem>();
        }

        [XDataItemProperty(
        Comment = "")]
        public int HoldCode
        {
            get => GetInt32(nameof(HoldCode));
            set => SetInt32(nameof(HoldCode), value);
        }

        [XDataItemProperty(
        Comment = "",
        MaxLength = Constant.HoldCodeDescriptionLength)]
        public string Description
        {
            get => GetString(nameof(Description));
            set => SetString(nameof(Description), value);
        }


        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.

    }
}
