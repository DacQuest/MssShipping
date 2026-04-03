using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.MessageBox;
using Mss.Collections;

namespace Mss.Views
{
    public partial class PlcTagWriteForm : Form
    {
        PlcTagItem _plcTagItem;
        PlcTagsProxy _plcTagsProxy;

        public PlcTagWriteForm(
            PlcTagItem plcTagItem,
            PlcTagsProxy proxy)
        {
            InitializeComponent();
            _plcTagItem = plcTagItem;
            _plcTagsProxy = proxy;
            Text = $"Write Tag: {plcTagItem.TagName}";
        }

        private void _TxtNewTagValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void _BtnWriteTag_Click(object sender, EventArgs e)
        {
            string text = _txtNewTagValue.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            PlcTagItem workingTag = XDataItem.Clone(_plcTagItem);
            switch (_plcTagItem.DataType)
            {
                case XValueDataType.Boolean:
                    if (text == "0")
                    {
                        text = "False";
                    }
                    else if (text == "1")
                    {
                        text = "True";
                    }
                    if (text != "True" && text != "False")
                    {
                        MessageBox.Show(
                            this,
                            $"The new value is not valid for Tag '{_plcTagItem.TagName}' which is of type '{_plcTagItem.DataTypeText}'.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    workingTag.NewBooleanValue = bool.Parse(text);
                    break;
                case XValueDataType.Int16:
                    if (!short.TryParse(text, out short shortValue))
                    {
                        MessageBox.Show(
                            this,
                            $"The new value is not valid for Tag '{_plcTagItem.TagName}' which is of type '{_plcTagItem.DataTypeText}'.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    workingTag.NewInt16Value = shortValue;
                    break;
                case XValueDataType.Int32:
                    if (!int.TryParse(text, out int intValue))
                    {
                        MessageBox.Show(
                            this,
                            $"The new value is not valid for Tag '{_plcTagItem.TagName}' which is of type '{_plcTagItem.DataTypeText}'.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    workingTag.NewInt32Value = intValue;
                    break;
                case XValueDataType.String:
                    workingTag.NewStringValue = text;
                    break;
                default:
                    return;
            }
            workingTag.WriteTag = true;
            try
            {
                UseWaitCursor = true;
                if (!_plcTagsProxy.SafeUpdate(_plcTagItem.TagName, _plcTagItem, ref workingTag))
                {
                    UseWaitCursor = false;
                    XMessageBox.Show(
                        this,
                        "Update Failed. PLC Data was Stale.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                DialogResult = DialogResult.OK;
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

    }
}
