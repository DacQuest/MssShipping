using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.SnapInViews;
using DacQuest.DFX.Devices;
using DacQuest.DFX.Core;
using Mss.Common;
using Mss.Collections;
using Mss.Data;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.DataItems.Proxy;

namespace Mss.Views
{
    public partial class LabelPrinterTesterView : XSnapInView
    {
        private XDevice _device;
        private const int _groupIndex = 1;
        private LoadItem _loadItem;

        public LabelPrinterTesterView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {
            _PopulateLoadItem();

            if (!XConfigurationManager.TryGetConfigurationItem(
                XConfigurationManager.strX_TAG_DEVICE_SETS_SECTION,
                Constant.LabelPrinterTesterDeviceSetName,
                out XDeviceSetConfigurationItem deviceSetConfigurationItem))
            {
                _ = MessageBox.Show(
                    $"{Constant.LabelPrinterTesterDeviceSetName} Device Set not found in configuration",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            foreach (string name in deviceSetConfigurationItem.DeviceNames)
            {
                _ = lstLabelPrinters.Items.Add(name);
            }
            if (lstLabelPrinters.Items.Count > 0)
            {
                lstLabelPrinters.SelectedIndex = 0;
                btnConnect.Enabled = true;
            }
        }

//         protected override void ProcessParameters(XConfigurationParameterSet parameters)
//         {
//         }

        private void _PopulateLoadItem()
        {
            PalletItem palletItem = new PalletItem
            {
                PalletID = "5150",
                JobID = "765432",
                Sku = "F70-F364-1Y6"
            };
            BroadcastItem broadcastItem = new BroadcastItem
            {
                Csn = "4001219F",
                Vin = "2FD0W4HT5VBA00287",
            };
            _loadItem = new LoadItem()
            {
                Broadcast = broadcastItem,
                Pallet = palletItem
            };
        }

        private void _BtnConnect_Click(object sender, EventArgs e)
        {
            string deviceName = (string)lstLabelPrinters.SelectedItem;
            if (!XConfigurationManager.TryGetConfigurationItem(
                XConfigurationManager.strX_TAG_DEVICES_SECTION,
                deviceName,
                out XDeviceConfigurationItem deviceConfigurationItem))
            {
                _ = MessageBox.Show(
                    $"Failed to load label printer configuration: {deviceName}",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            try
            {
                _device = (XDevice)XActivator.CreateInstance(deviceConfigurationItem.ClassType, true);
                if (!_device.Initialize(deviceConfigurationItem))
                {
                    _ = MessageBox.Show(
                        $"Failed to initialize label printer: {deviceName}",
                        "ERROR",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                btnConnect.Enabled = false;
                _btnPrintShippingLabel.Enabled = true;
                if (deviceName.Contains("Manual"))
                {
                    _btnPrintTrailerLabel.Enabled = true;
                }
                btnDisconnect.Enabled = true;
            }
            catch (Exception x)
            {
                _device = null;
                x.PublishSystemEvent("Label Printer Tester");
                _ = MessageBox.Show(
                    $"Failed to initialize label printer: {deviceName}",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void _BtnDisconnect_Click(object sender, EventArgs e)
        {
            _Disconnect();
        }

        private void _Disconnect()
        {
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
            btnConnect.Enabled = true;
            _btnPrintShippingLabel.Enabled = false;
            _btnPrintTrailerLabel.Enabled = false;
            btnDisconnect.Enabled = false;
        }

        public override bool ViewClosing(bool force)
        {
            _Disconnect();
            return true;
        }

        private void _BtnPrintShippingLabel_Click(object sender, EventArgs e)
        {
            _ = _device.WriteTag(
                Constant.LabelPrintCommandRoleName,
                ShippingLabelFormatter.Format(_loadItem, "LH"));
        }

        private void _BtnPrintTrailerLabel_Click(object sender, EventArgs e)
        {
            _ = _device.WriteTag(
                Constant.LabelPrintCommandRoleName,
                TrailerLabelFormatter.Format(
                    54,
                    "1201",
                    "1232",
                    "07"));
        }

        //protected override void AutoSubscribe()
        //{
        //}

        //public override void ViewClosed()
        //{
        //}

        //protected override MenuStrip GetViewMenuStrip()
        //{
        //    return null;
        //}

        //protected override Boolean ProcessEnterKey()
        //{
        //    return false;
        //}

        //protected override Boolean ProcessEscapeKey()
        //{
        //    return false;
        //}

    }
}
