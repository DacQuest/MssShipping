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
        }

        [XDataItemProperty(
    Comment = "The current status of this Load Item.")]
        public LoadItemStatus Status
        {
            get
            {
                return GetEnum<LoadItemStatus>("Status");
            }
            set
            {
                SetEnum("Status", value);
            }
        }

        [XDataItemProperty(
    Comment = ".")]
        public PickMode PickMode
        {
            get
            {
                return GetEnum<PickMode>("PickMode");
            }
            set
            {
                SetEnum("PickMode", value);
            }
        }

        [XDataItemProperty(
    Comment = ".")]
        public string PickModePalletID
        {
            get
            {
                return GetString("PickModePalletID");
            }
            set
            {
                SetString("PickModePalletID", value);
            }
        }
        [XDataItemProperty(
    Comment = ".")]
        public Int32 PickModeJobID
        {
            get
            {
                return GetInt32("PickModeJobID");
            }
            set
            {
                SetInt32("PickModeJobID", value);
            }
        }

        [XDataItemProperty(
    Comment = "The Pallet Item representing the pallet that fulfills this Load requirement.",
    MirrorToChildTable = true)]
        public PalletItem Pallet
        {
            get
            {
                return GetDataItem<PalletItem>("Pallet");
            }
            set
            {
                SetDataItem("Pallet", value);
            }
        }



        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.

    }
}
