using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.SnapInViews;
using Mss.Common;
using Mss.Collections;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using System.Threading;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Strings;
// using DevExpress.XtraEditors.Controls;

namespace Mss.Views
{
    public partial class SystemSettingsView : XSnapInView
    {
        private SystemSettingsViewParameterSetWrapper _parameters;
        private SystemSettingsProxy _systemSettingsProxy;

        public SystemSettingsView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {
            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
            _systemSettingsProxy.DataItemChanged += _SystemSettings_DataItemChanged;
            _systemSettingsProxy.CollectionRefreshed += _SystemSettings_CollectionRefreshed;

//             _cmbMaxAuditAttempts.Items.AddRange(Enumerable.Range(0, 6).Cast<object>().ToArray());
//             _cmbMaxAuditAttempts.SelectedIndex = _systemSettingsProxy.Item.MaxAuditAttempts;

            _cmbFifoMode.AddEnumItem(FifoMode.CraneFifo);
            _cmbFifoMode.AddEnumItem(FifoMode.BuildFifo);
            _cmbFifoMode.AddEnumItem(FifoMode.Closest);
            _cmbFifoMode.SelectEnumItem(_systemSettingsProxy.Item.FifoMode);

            _cmbSlugPickPriority.AddEnumItem(SlugPickPriority.SmallerLoadNumber);
            _cmbSlugPickPriority.AddEnumItem(SlugPickPriority.Balanced);
            _cmbSlugPickPriority.AddEnumItem(SlugPickPriority.SlugA);
            _cmbSlugPickPriority.AddEnumItem(SlugPickPriority.SlugB);
            _cmbSlugPickPriority.AddEnumItem(SlugPickPriority.SlugAOnly);
            _cmbSlugPickPriority.AddEnumItem(SlugPickPriority.SlugBOnly);
            _cmbSlugPickPriority.SelectEnumItem(_systemSettingsProxy.Item.SlugPickPriority);

            _cmbPreferredSlug.AddEnumItem(SlugLetter.A);
            _cmbPreferredSlug.AddEnumItem(SlugLetter.B);
            _cmbPreferredSlug.SelectEnumItem(_systemSettingsProxy.Item.PreferredSlug);

            bool allowEditing = _parameters.AllowEditing;
            bool confirmEdits = _parameters.ConfirmEdits;

            _craneFunctionGrid.Initialize(
                allowEditing,
                confirmEdits);

            _btnLoadAEnabled.Visible = allowEditing;
//             _btnAutoReleaseLoadsEnabled.Visible = allowEditing;
            _btnLoadBEnabled.Visible = allowEditing;
            _btnAutoAcceptLoads.Visible = allowEditing;
//             _cmbMaxAuditAttempts.Visible = allowEditing;
            _cmbPreferredSlug.Visible = allowEditing;
            _cmbFifoMode.Visible = allowEditing;
            _cmbSlugPickPriority.Visible = allowEditing;

            _PopulateControls();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (SystemSettingsViewParameterSetWrapper)parameters;
        }

        private bool _ConfirmChange(bool settingValue, string settingName)
        {

            string actionString = settingValue ? "disable" : "enable";

            if (_parameters.ConfirmEdits
                && XMessageBox.Show(
                    this,
                    $"Do you want to {actionString} the {settingName} setting?",
                    "Confirm Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return false;
            }
            return true;
        }

        private bool _ConfirmChange(string settingName)
        {
            if (_parameters.ConfirmEdits
                && XMessageBox.Show(
                    this,
                    $"Do you want to change the {settingName} setting?",
                    "Confirm Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return false;
            }
            return true;
        }

        private void _PopulateControls()
        {
            SystemSettingsItem item = _systemSettingsProxy.Item;

            _UpdateBooleanControls(item.SlugAEnabled, _lblLoadAEnabled, _btnLoadAEnabled);
            _UpdateBooleanControls(item.SlugBEnabled, _lblLoadBEnabled, _btnLoadBEnabled);

//             _UpdateBooleanControls(item.AutoReleaseLoadsEnabled, _lblAutoReleasLoadsEnabled, _btnAutoReleaseLoadsEnabled);
            _UpdateBooleanControls(item.AutoAcceptLoadsEnabled, _lblAutoAcceptLoads, _btnAutoAcceptLoads);

//             _cmbMaxAuditAttempts.SelectedIndex = item.MaxAuditAttempts;
//             _lblAuditAttempts.Text = item.MaxAuditAttempts.ToText();

            _cmbFifoMode.SelectEnumItem(item.FifoMode);
            _lblFifoMode.Text = item.FifoMode.ToText();

            _cmbSlugPickPriority.SelectEnumItem(item.SlugPickPriority);
            _lblSlugPickPriority.Text = item.SlugPickPriority.ToText();

            _cmbPreferredSlug.SelectEnumItem(item.PreferredSlug);
            _lblPreferredSlug.Text = item.PreferredSlug.ToText();

        }

        private void _UpdateBooleanControls(
            bool setting,
            Label label,
            Button button)
        {
            if (setting)
            {
                label.Text = "Enabled";
                label.ForeColor = Color.Black;
                label.BackColor = Color.Lime;

                button.Text = "Enabled";
                button.ForeColor = Color.Black;
                button.BackColor = Color.Lime;
            }
            else
            {
                label.Text = "Disabled";
                label.ForeColor = Color.White;
                label.BackColor = Color.Gray;

                button.Text = "Enable";
                button.ForeColor = SystemColors.ControlText;
                button.BackColor = SystemColors.ControlLight;
            }
        }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _PopulateControls();
        }

        private void _SystemSettings_CollectionRefreshed(object sender, EventArgs e)
        {
            _PopulateControls();
        }

        private void _BtnDisableOutbounds_Click(object sender, EventArgs e)
        {
            if (XMessageBox.Show(
                this,
                "Are you sure you want to DISABLE all Crane Outbounds?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            _EnableAllOutbounds(false);
        }

        private void _BtnEnableOutbounds_Click(object sender, EventArgs e)
        {
            _EnableAllOutbounds(true);
        }

        private void _EnableAllOutbounds(bool enable)
        {
//             _systemSettingsProxy.SetItemProperty(nameof(SystemSettings.OutboundsEnabled), 0, enable);
        }

        //protected override void AutoSubscribe()
        //{
        //}

        private void _CmbMaxAuditAttempts_SelectedIndexChanged(object sender, EventArgs e)
        {
//             if (_cmbMaxAuditAttempts.SelectedIndex > -1)
//             {
//                 int attempts = (int)_cmbMaxAuditAttempts.SelectedItem;
//                 if (attempts != _systemSettingsProxy.GetItem().MaxAuditAttempts)
//                 {
//                     _systemSettingsProxy.SetItemProperty(nameof(SystemSettings.MaxAuditAttempts), attempts);
//                 }
//             }
        }

        private bool _CanChangePickMode(out string error)
        {
            error = string.Empty;
            return true;
        }

        private void _CmbFifoMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            FifoMode currentFifoMode = _systemSettingsProxy.Item.FifoMode;
            FifoMode newFifoMode = _cmbFifoMode.GetSelectedEnumItem<FifoMode>();
            if (newFifoMode != currentFifoMode)
            {
                if (_ConfirmChange("Fifo Mode"))
                {
                    _systemSettingsProxy.SetItemProperty(
                      nameof(SystemSettingsItem.FifoMode),
                      newFifoMode,
                      this);
                }
                else
                {
                    _cmbFifoMode.SelectEnumItem(currentFifoMode);
                }
            }
        }

        private void _CmbPreferredLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            SlugLetter currentPreferredLoad = _systemSettingsProxy.Item.PreferredSlug;
            SlugLetter newPreferredLoad = _cmbPreferredSlug.GetSelectedEnumItem<SlugLetter>();
            if (newPreferredLoad != currentPreferredLoad)
            {
                if (_ConfirmChange("Preferred Load"))
                {
                    _systemSettingsProxy.SetItemProperty(
                      nameof (SystemSettingsItem.PreferredSlug),
                      newPreferredLoad,
                      this);
                }
                else
                {
                    _cmbPreferredSlug.SelectEnumItem(currentPreferredLoad);
                }
            }
        }

        private void _BtnLoadAEnabled_Click(object sender, EventArgs e)
        {
            _systemSettingsProxy.Refresh();
            if (_ConfirmChange(nameof(SystemSettingsItem.SlugAEnabled)))
            {

                _systemSettingsProxy.SetItemProperty(
                    nameof(SystemSettingsItem.SlugAEnabled),
                    !_systemSettingsProxy.Item.SlugAEnabled);
            }
        }

        private void _BtnLoadBEnabled_Click(object sender, EventArgs e)
        {
            _systemSettingsProxy.Refresh();
            if (_ConfirmChange(nameof(SystemSettingsItem.SlugBEnabled)))
            {

                _systemSettingsProxy.SetItemProperty(
                    nameof(SystemSettingsItem.SlugBEnabled),
                    !_systemSettingsProxy.Item.SlugBEnabled);
            }
        }

//         private void _BtnAutoReleaseLoads_Click(object sender, EventArgs e)
//         {
//             _systemSettingsProxy.Refresh();
//             bool newEnabled = !_systemSettingsProxy.Item.AutoReleaseLoadsEnabled;
//             if (_ConfirmChange(nameof(SystemSettingsItem.AutoReleaseLoadsEnabled)))
//             {
//                 _systemSettingsProxy.SetItemProperty(
//                     nameof(SystemSettingsItem.AutoReleaseLoadsEnabled),
//                     newEnabled);
//             }
//         }

//         private void _BtnAutoReleaseLoadBBroadcast_Click(object sender, EventArgs e)
//         {
//             _systemSettingsProxy.Refresh();
//             bool newEnabled = !_systemSettingsProxy.Item.LoadBAutoReleaseBroadcastEnabled;
//             if (_ConfirmChange(nameof(SystemSettingsItem.LoadBAutoReleaseBroadcastEnabled)))
//             {
//                 if (newEnabled)
//                 {
//                     // if enabling Auto Release, disable Partial Load
//                     _systemSettingsProxy.SetItemProperty(
//                         nameof(SystemSettingsItem.LoadBPartialBroadcastReleaseEnabled),
//                         false);
//                 }
//                 _systemSettingsProxy.SetItemProperty(
//                     nameof(SystemSettingsItem.LoadBAutoReleaseBroadcastEnabled),
//                     newEnabled);
//             }
//         }

//         private void _BtnAllowPartialLoadARelease_Click(object sender, EventArgs e)
//         {
//             _systemSettingsProxy.Refresh();
//             bool newEnabled = !_systemSettingsProxy.Item.LoadAPartialBroadcastReleaseEnabled;
//             if (_ConfirmChange(nameof(SystemSettingsItem.LoadAPartialBroadcastReleaseEnabled)))
//             {
//                 if (newEnabled)
//                 {
//                     // if enabling Partial Load, disable Auto Release
//                     _systemSettingsProxy.SetItemProperty(
//                         nameof(SystemSettingsItem.LoadAAutoReleaseBroadcastEnabled),
//                         false);
//                 }
//                 _systemSettingsProxy.SetItemProperty(
//                     nameof(SystemSettingsItem.LoadAPartialBroadcastReleaseEnabled),
//                     newEnabled);
//             }
//         }

//         private void _BtnAllowPartialLoadBRelease_Click(object sender, EventArgs e)
//         {
//             _systemSettingsProxy.Refresh();
//             bool newEnabled = !_systemSettingsProxy.Item.LoadBPartialBroadcastReleaseEnabled;
//             if (_ConfirmChange(nameof(SystemSettingsItem.LoadBPartialBroadcastReleaseEnabled)))
//             {
//                 if (newEnabled)
//                 {
//                     // if enabling Partial Load, disable Auto Release
//                     _systemSettingsProxy.SetItemProperty(
//                         nameof(SystemSettingsItem.LoadBAutoReleaseBroadcastEnabled),
//                         false);
//                 }
//                 _systemSettingsProxy.SetItemProperty(
//                     nameof(SystemSettingsItem.LoadBPartialBroadcastReleaseEnabled),
//                     newEnabled);
//             }
//         }

        private void _BtnAutoAcceptLoads_Click(object sender, EventArgs e)
        {
            _systemSettingsProxy.Refresh();
            if (_ConfirmChange(nameof(SystemSettingsItem.AutoAcceptLoadsEnabled)))
            {

                _systemSettingsProxy.SetItemProperty(
                    nameof(SystemSettingsItem.AutoAcceptLoadsEnabled),
                    !_systemSettingsProxy.Item.AutoAcceptLoadsEnabled);
            }
        }

//         private void _BtnUse2ndPrinter_Click(object sender, EventArgs e)
//         {
//             _systemSettingsProxy.Refresh();
//             if (_ConfirmChange(nameof(SystemSettingsItem.UseSecondaryShipperReportPrinter)))
//             {
// 
//                 _systemSettingsProxy.SetItemProperty(
//                     nameof(SystemSettingsItem.UseSecondaryShipperReportPrinter),
//                     !_systemSettingsProxy.Item.UseSecondaryShipperReportPrinter);
//             }
//         }

//         private void _BtnUseDedicatedLoadLanes_Click(object sender, EventArgs e)
//         {
//             _systemSettingsProxy.Refresh();
//             if (_ConfirmChange(nameof(SystemSettingsItem.UseDedicatedLoadLanes)))
//             {
//                 _systemSettingsProxy.SetItemProperty(
//                     nameof(SystemSettingsItem.UseDedicatedLoadLanes),
//                     !_systemSettingsProxy.Item.UseDedicatedLoadLanes);
//             }
//         }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _craneFunctionGrid.RefreshData();
        }

        //!!!Revisit, is this a thing?
//         private void _BtnStoreAllPalletsToHold(object sender, EventArgs e)
//         {
            //_systemSettingsProxy.Refresh();
            //if (_ConfirmChange(_systemSettingsProxy.Item.StoreAllPalletsAsHold, "StoreAllRow1PalletsAsHold"))
            //{
            //    _systemSettingsProxy.SetItemProperty(
            //        "StoreAllRow1PalletsAsHold",
            //        !_systemSettingsProxy.Item.StoreAllPalletsAsHold,
            //        this);
//         }

        public override bool ViewClosing(bool force)
        {
            if (_systemSettingsProxy != null)
            {
                _systemSettingsProxy.DataItemChanged -= _SystemSettings_DataItemChanged;
                _systemSettingsProxy.CollectionRefreshed -= _SystemSettings_CollectionRefreshed;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
//             if (_currentLoadProxy != null)
//             {
//                 XProxyCache.Release(_currentLoadProxy);
//                 _currentLoadProxy = null;
//             }
//             if (_pitProxy != null)
//             {
//                 XProxyCache.Release(_pitProxy);
//                 _pitProxy = null;
//             }
            return true;
        }

        private void SystemSettingsView_Load(object sender, EventArgs e)
        {

        }

        private void _CmbSlugPickPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            SlugPickPriority currentPriority = _systemSettingsProxy.Item.SlugPickPriority;
            SlugPickPriority newPriority = _cmbSlugPickPriority.GetSelectedEnumItem<SlugPickPriority>();
            if (newPriority != currentPriority)
            {
                if (_ConfirmChange("Slug Pick Priority"))
                {
                    _systemSettingsProxy.SetItemProperty(
                      nameof(SystemSettingsItem.SlugPickPriority),
                      newPriority,
                      this);
                }
                else
                {
                    _cmbSlugPickPriority.SelectEnumItem(currentPriority);
                }
            }
        }

        //         private void _CmbFrontLeftLabelAt_SelectedIndexChanged(object sender, EventArgs e)
        //         {
        //             PrintingOperation currentPrintingOperation = _systemSettingsProxy.Item.PrintFrontLeftLabelAt;
        //             PrintingOperation newPrintingOperation = _cmbFrontLeftLabelAt.GetSelectedEnumItem<PrintingOperation>();
        //             if (newPrintingOperation != currentPrintingOperation)
        //             {
        //                 if (_ConfirmChange("Front Left Label Printing Operation"))
        //                 {
        //                     _systemSettingsProxy.SetItemProperty(
        //                       nameof(SystemSettingsItem.PrintFrontLeftLabelAt),
        //                       newPrintingOperation,
        //                       this);
        //                 }
        //                 else
        //                 {
        //                     _cmbFrontLeftLabelAt.SelectEnumItem(currentPrintingOperation);
        //                 }
        //             }
        //         }

        //         private void _CmbFrontRightLabelAt_SelectedIndexChanged(object sender, EventArgs e)
        //         {
        //             PrintingOperation currentPrintingOperation = _systemSettingsProxy.Item.PrintFrontRightLabelAt;
        //             PrintingOperation newPrintingOperation = _cmbFrontRightLabelAt.GetSelectedEnumItem<PrintingOperation>();
        //             if (newPrintingOperation != currentPrintingOperation)
        //             {
        //                 if (_ConfirmChange("Front Right Label Printing Operation"))
        //                 {
        //                     _systemSettingsProxy.SetItemProperty(
        //                       nameof(SystemSettingsItem.PrintFrontRightLabelAt),
        //                       newPrintingOperation,
        //                       this);
        //                 }
        //                 else
        //                 {
        //                     _cmbFrontRightLabelAt.SelectEnumItem(currentPrintingOperation);
        //                 }
        //             }
        //         }

        //         private void _CmbRearLabelAt_SelectedIndexChanged(object sender, EventArgs e)
        //         {
        //             PrintingOperation currentPrintingOperation = _systemSettingsProxy.Item.PrintRearLabelAt;
        //             PrintingOperation newPrintingOperation = _cmbRearLabelAt.GetSelectedEnumItem<PrintingOperation>();
        //             if (newPrintingOperation != currentPrintingOperation)
        //             {
        //                 if (_ConfirmChange("Rear Label Printing Operation"))
        //                 {
        //                     _systemSettingsProxy.SetItemProperty(
        //                       nameof(SystemSettingsItem.PrintRearLabelAt),
        //                       newPrintingOperation,
        //                       this);
        //                 }
        //                 else
        //                 {
        //                     _cmbRearLabelAt.SelectEnumItem(currentPrintingOperation);
        //                 }
        //             }
        //         }

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
