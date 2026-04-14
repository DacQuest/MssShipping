using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
// using AutomatedSolutions.Win.Comm;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Devices.Tags;
using Mss.Collections.Properties;
using Mss.Common;

namespace Mss.Collections
{
    [Serializable]
    public partial class PlcTagItem : XDataItem
    {
        public string QualityText => Quality.ToText();

        public Image QualityImage
        {
            get
            {
                Image image = Resources.RoundRedBang16;
                switch (Quality)
                {
                    case XTagQuality.Bad:
                    case XTagQuality.Error:
                        image = Resources.RoundRedBang16;
                        break;
                    case XTagQuality.Uncertain:
                        image = Resources.RoundYellowBangBorder16;
                        break;
                    case XTagQuality.Good:
                        image = Resources.RoundGreenCheck16;
                        break;
                }
                return image;
            }
        }

        public string ValueText
        {
            get
            {
                if (DataType == XValueDataType.Boolean)
                {
                    return BooleanValue.ToString();
                }
                else if (DataType == XValueDataType.Int16)
                {
                    return Int16Value.ToString();
                }
                else if (DataType == XValueDataType.Int32)
                {
                    return Int32Value.ToString();
                }
                else if (DataType == XValueDataType.String)
                {
                    return StringValue;
                }
                return "UNKNOWN";
            }
        }

        public string DataTypeText
        {
            get
            {
                if (DataType == XValueDataType.Boolean)
                {
                    return "BIT/BOOL";
                }
                else if (DataType == XValueDataType.Int16)
                {
                    return "INT";
                }
                else if (DataType == XValueDataType.Int32)
                {
                    return "DINT";
                }
                else if (DataType == XValueDataType.String)
                {
                    return "STRING";
                }
                return "UNKNOWN";
            }
        }

        public string AccessText => AccessMode.ToText();

        public string LastUpdatedOnText
        {
            get
            {
                string text = "Unknown";
                if (LastUpdatedOn > Constant.BeginningOfTime)
                {
                    text = LastUpdatedOn.ToString(Constant.DateTimeFormat);
                }
                return text;
            }
        }
    }
}
