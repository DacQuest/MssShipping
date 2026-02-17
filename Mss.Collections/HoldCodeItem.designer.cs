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
        Comment = "",
        MaxLength = Constant.CommentLength)]
        public string HoldCode
        {
            get => GetString(nameof(HoldCode));
            set => SetString(nameof(HoldCode), value);
        }


        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.

    }
}
