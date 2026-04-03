using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Devices.Tags;
using Mss.Common;


namespace Mss.Collections
{
    partial class PlcTagItem
    {
        public PlcTagItem()
        {
            SetByteConverter<PlcTagItem>();
        }

        //See DataItemReference.txt under a DFX Collection Class Library project Properties Folder for examples.
        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.DeviceNameLength)]
        public string DeviceName
        {
            get => GetString(nameof(DeviceName));
            set => SetString(nameof(DeviceName), value);
        }

        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.DisplayNameLength)]
        public string DisplayName
        {
            get => GetString(nameof(DisplayName));
            set => SetString(nameof(DisplayName), value);
        }

        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.RoleNameLength)]
        public string RoleName
        {
            get => GetString(nameof(RoleName));
            set => SetString(nameof(RoleName), value);
        }

        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.TagNameLength)]
        public string TagName
        {
            get => GetString(nameof(TagName));
            set => SetString(nameof(TagName), value);
        }

        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.PlcTagNameLength)]
        public string PlcTagName
        {
            get => GetString(nameof(PlcTagName));
            set => SetString(nameof(PlcTagName), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public XValueDataType DataType
        {
            get => GetEnum<XValueDataType>(nameof(DataType));
            set => SetEnum(nameof(DataType), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public XTagAccessMode AccessMode
        {
            get => GetEnum<XTagAccessMode>(nameof(AccessMode));
            set => SetEnum(nameof(AccessMode), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public XTagQuality Quality
        {
            get => GetEnum<XTagQuality>(nameof(Quality));
            set => SetEnum(nameof(Quality), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public short Int16Value
        {
            get => GetInt16(nameof(Int16Value));
            set => SetInt16(nameof(Int16Value), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public short NewInt16Value
        {
            get => GetInt16(nameof(NewInt16Value));
            set => SetInt16(nameof(NewInt16Value), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public int Int32Value
        {
            get => GetInt32(nameof(Int32Value));
            set => SetInt32(nameof(Int32Value), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public int NewInt32Value
        {
            get => GetInt32(nameof(NewInt32Value));
            set => SetInt32(nameof(NewInt32Value), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool BooleanValue
        {
            get => GetBoolean(nameof(BooleanValue));
            set => SetBoolean(nameof(BooleanValue), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool NewBooleanValue
        {
            get => GetBoolean(nameof(NewBooleanValue));
            set => SetBoolean(nameof(NewBooleanValue), value);
        }

        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.StringValueLength)]
        public string StringValue
        {
            get => GetString(nameof(StringValue));
            set => SetString(nameof(StringValue), value);
        }

        [XDataItemProperty(
            Comment = "",
            MaxLength = Constant.StringValueLength)]
        public string NewStringValue
        {
            get => GetString(nameof(NewStringValue));
            set => SetString(nameof(NewStringValue), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public DateTime LastUpdatedOn
        {
            get => GetDateTime(nameof(LastUpdatedOn));
            set => SetDateTime(nameof(LastUpdatedOn), value);
        }

        [XDataItemProperty(
            Comment = "")]
        public bool WriteTag
        {
            get => GetBoolean(nameof(WriteTag));
            set => SetBoolean(nameof(WriteTag), value);
        }

    }
}
