using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.SnapInViews;
using Mss.Common;
using Mss.Collections;
using Mss.Data;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;

namespace Mss.Views
{
    public partial class SlugsView : XSnapInView
    {
        private SlugsViewParameterSetWrapper _parameters;
        private SystemSettingsProxy _systemSettingsProxy = null;
        private SlugProxy _slugAProxy = null;
        private SlugProxy _slugBProxy = null;
        private PitProxy _upperPitProxy = null;
        private PitProxy _lowerPitProxy = null;
        private StorageProxy _storageProxy = null;
        private bool _showAcceptLoadButtons = false;
        private bool _showAbortLoadButtons = false;

        public SlugsView()
        {
            InitializeComponent();

            ComboBox comboBox = (ComboBox)(navigatorLegend.Control);
            comboBox.SelectedIndex = 0;
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.DrawItem += (sender, e) =>
            {
                LoadItemStatus status = LoadItemStatus.Invalid;
                bool transferring = false;
                bool title = false;
                switch ((string)comboBox.Items[e.Index])
                {
                    case "Invalid":
                        status = LoadItemStatus.Invalid;
                        break;
                    case "Waiting":
                        status = LoadItemStatus.Waiting;
                        break;
                    case "Pending":
                        status = LoadItemStatus.Pending;
                        break;
                    case "Pickable":
                        status = LoadItemStatus.Pickable;
                        break;
                    case "Picking":
                        status = LoadItemStatus.Picking;
                        break;
                    case "Picked":
                        status = LoadItemStatus.Picked;
                        break;
                    case "Presequenced":
                        status = LoadItemStatus.Presequenced;
                        break;
                    case "Sequenced":
                        status = LoadItemStatus.Sequenced;
                        break;
                    case "Transferring":
                        transferring = true;
                        status = LoadItemStatus.Sequenced;
                        break;
                    case "Done":
                        status = LoadItemStatus.Done;
                        break;
                    case "Loadable":
                        status = LoadItemStatus.Loadable;
                        break;
                    default:
                        title = true;
                        break;
                }

                (Color Fore, Color Back) colors = (Color.Black, Color.White);
                string text = "Status Legend";
                Font itemFont = e.Font;
                itemFont = new Font(itemFont, FontStyle.Bold);
                if (!title)
                {
                    colors = LoadItem.GetLoadStatusColors(status, transferring, false);
                    text = transferring ? "Transferring" : status.ToText();
                }

                // Draw the background
                Brush backgroundBrush = new SolidBrush(colors.Back);
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                backgroundBrush.Dispose();

                // Draw the text
                Brush textBrush = new SolidBrush(colors.Fore);
                e.Graphics.DrawString(text, itemFont, textBrush, e.Bounds.X, e.Bounds.Y);
                textBrush.Dispose();

            };
            comboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (comboBox.SelectedIndex > 0)
                {
                    comboBox.SelectedIndex = 0;
                }
            };

        }

        protected override void OpenView()
        {
//             if (!MesQuery.TryGetPickableLoads(
//                 out List<Load> pickableLoads,
//                 out string error))
//             {
//                 // show empty grid and message box
//             }

            XProxyCache.Acquire(Constant.UpperPitName, out _upperPitProxy);
            XProxyCache.Acquire(Constant.LowerPitName, out _lowerPitProxy);
            XProxyCache.Acquire(Constant.StorageName, out _storageProxy);

            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
            _systemSettingsProxy.DataItemChanged += _SystemSettings_DataItemChanged;
            _systemSettingsProxy.CollectionRefreshed += _SystemSettings_CollectionRefreshed;

            XProxyCache.Acquire(Constant.SlugAName, out _slugAProxy);
            _slugAGridUpper.Initialize(
                SlugLetter.A,
                Levels.Upper,
                _parameters.AllowReprintLabel,
                _parameters.AllowEditItem,
                _parameters.AllowRollback,
                false,
                _parameters.ShowShortages,
                _parameters.FlashGridCellTimeoutSeconds);
            _slugAGridLower.Initialize(
                SlugLetter.A,
                Levels.Lower,
                _parameters.AllowReprintLabel,
                _parameters.AllowEditItem,
                _parameters.AllowRollback,
                false,
                _parameters.ShowShortages,
                _parameters.FlashGridCellTimeoutSeconds);

            XProxyCache.Acquire(Constant.SlugBName, out _slugBProxy);
            _slugBGridUpper.Initialize(
                SlugLetter.B,
                Levels.Upper,
                _parameters.AllowReprintLabel,
                _parameters.AllowEditItem,
                _parameters.AllowRollback,
                false,
                _parameters.ShowShortages,
                _parameters.FlashGridCellTimeoutSeconds);
            _slugBGridLower.Initialize(
                SlugLetter.B,
                Levels.Lower,
                _parameters.AllowReprintLabel,
                _parameters.AllowEditItem,
                _parameters.AllowRollback,
                false,
                _parameters.ShowShortages,
                _parameters.FlashGridCellTimeoutSeconds);

//             XSharedCollectionConfigurationItem configurationItem =
//                 XConfigurationManager.GetConfigurationItem<XSharedCollectionConfigurationItem>(
//                     XConfigurationManager.strX_TAG_SHARED_COLLECTIONS_SECTION,
//                     Constant.SlugAName);

            _lblSlugAName.Text = SlugLetter.A.SlugDisplayName();
            _lblSlugBName.Text = SlugLetter.B.SlugDisplayName();

            _slugAProxy.DataItemChanged += _SlugA_DataItemChanged;
            _slugAProxy.CollectionRefreshed += _SlugA_CollectionRefreshed;

            _slugBProxy.DataItemChanged += _SlugB_DataItemChanged;
            _slugBProxy.CollectionRefreshed += _SlugB_CollectionRefreshed;

            _showAcceptLoadButtons = _parameters.AllowAcceptLoad;
            _btnAcceptLoadA.Visible = _showAcceptLoadButtons;
            _btnAcceptLoadB.Visible = _showAcceptLoadButtons;

            _showAbortLoadButtons = _parameters.AllowAbortLoad;
            _btnAbortLoadA.Visible = _showAbortLoadButtons;
            _btnAbortLoadB.Visible = _showAbortLoadButtons;

            _navigatorBtnReleaseBroadcast.Visible = _parameters.AllowReleaseBroadcast;

            _UpdateAcceptLoadButtons();
            _UpdateAbortLoadButtons();
            _UpdateLoadNumbers();


        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            base.ProcessParameters(parameters);
            _parameters = (SlugsViewParameterSetWrapper)parameters;
        }

//         private bool _TryReleaseLoad(Load load, out string error)
//         {
//             if (!_IsPreferredSlugAvailable(load.PreferredSlug, out SlugLetter slugLetter))
//             {
//                 error = $"The Preferred Slug '{load.PreferredSlug.ToText()}' is not currently available.";
//                 return false;
//             }
//             if (!load.AllowShortage && _HasShortage(load))
//             {
//                 error = $"The load cannot be released because it has shortages and shortages are not allowed.";
//                 return false;
//             }
//             _ReleaseLoad(load, slugLetter);
//             error = string.Empty;
//             return true;
//         }
// 
//         private void _ReleaseLoad(Load load, SlugLetter slugLetter)
//         {
//             using (ConfirmReleaseLoadForm confirmReleaseForm = new ConfirmReleaseLoadForm(load, slugLetter))
//             {
//                 if (confirmReleaseForm.ShowDialog() != DialogResult.Yes)
//                 {
//                     return;
//                 }
//             }
// 
//             ParentForm.Cursor = Cursors.WaitCursor;
//             ReleaseLoadMessageData responseMessageData;
//             try
//             {
//                 ReleaseLoadMessageData messageData
//                     = new ReleaseLoadMessageData(
//                         load.ID,
//                         slugLetter,
//                         XSystemEvent.Create(
//                             ViewName,
//                             XSystemEventLevel.Manual,
//                             $"Load ID {load.ID} manually released to {slugLetter.SlugDisplayName()}."));
//                 if (!XMessaging.SyncPublish(
//                         out responseMessageData,
//                         ReleaseLoadMessageData.ReleaseLoadTimeoutMilliseconds,
//                         ReleaseLoadMessageData.ReleaseLoadMessageTopic,
//                         messageData,
//                         XMessageScopes.All,
//                         this))
//                 {
//                     ParentForm.Cursor = Cursors.Default;
//                     string message = $"Request to release Load ID {load.ID} to {slugLetter.SlugDisplayName()} has timed out. The operation may still have completed correctly.";
//                     XSystemEvent.Publish(
//                         ViewName,
//                         XSystemEventLevel.Error,
//                         message);
//                     _ = MessageBox.Show(
//                         this,
//                         message,
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                 }
//             }
//             finally
//             {
//                 ParentForm.Cursor = Cursors.Default;
//             }
//             if (!responseMessageData.ErrorMessage.IsNullOrWhiteSpace())
//             {
//                 _ = MessageBox.Show(
//                     this,
//                     responseMessageData.ErrorMessage,
//                     "Error",
//                     MessageBoxButtons.OK,
//                     MessageBoxIcon.Error);
//             }
// //             else
// //             {
// //                 if (_parameters.CloseAfterRelease)
// //                 {
// //                     CloseView();
// //                 }
// //             }
//         }

        private bool _IsSlugEnabled(SlugLetter slugLetter)
        {
            return slugLetter != SlugLetter.None 
                && (slugLetter == SlugLetter.A
                    ? _systemSettingsProxy.SlugAEnabled
                    : _systemSettingsProxy.SlugBEnabled);
        }

        private bool _IsSlugCleared(SlugLetter slugLetter)
        {
            return slugLetter != SlugLetter.None
                && (slugLetter == SlugLetter.A
                    ? _slugAProxy.Cleared
                    : _slugBProxy.Cleared);
        }

        private bool _CanReleaseToSlug(SlugLetter slugLetter)
            => _IsSlugEnabled(slugLetter) && _IsSlugCleared(slugLetter);

//         private bool _IsPreferredSlugAvailable(
//             LoadPreferredSlug loadPreferredSlug,
//             out SlugLetter slugLetter)
//         {
//             slugLetter = SlugLetter.None;
//             switch (loadPreferredSlug)
//             {
//                 case LoadPreferredSlug.A:
//                     if (_CanReleaseToSlug(SlugLetter.A))
//                     {
//                         slugLetter = SlugLetter.A;
//                     }
//                     else if (_CanReleaseToSlug(SlugLetter.B))
//                     {
//                         slugLetter = SlugLetter.B;
//                     }
//                     break;
//                 case LoadPreferredSlug.B:
//                     if (_CanReleaseToSlug(SlugLetter.B))
//                     {
//                         slugLetter = SlugLetter.B;
//                     }
//                     else if (_CanReleaseToSlug(SlugLetter.A))
//                     {
//                         slugLetter = SlugLetter.A;
//                     }
//                     break;
//                 case LoadPreferredSlug.ForceA:
//                     if (_CanReleaseToSlug(SlugLetter.A))
//                     {
//                         slugLetter = SlugLetter.A;
//                     }
//                     break;
//                 case LoadPreferredSlug.ForceB:
//                     if (_CanReleaseToSlug(SlugLetter.B))
//                     {
//                         slugLetter = SlugLetter.B;
//                     }
//                     break;
//                 case LoadPreferredSlug.NoPreference:
//                     if (_systemSettingsProxy.PreferredSlug == PreferredSlug.A)
//                     {
//                         if (_CanReleaseToSlug(SlugLetter.A))
//                         {
//                             slugLetter = SlugLetter.A;
//                         }
//                         else if (_CanReleaseToSlug(SlugLetter.B))
//                         {
//                             slugLetter = SlugLetter.B;
//                         }
//                     }
//                     else if (_systemSettingsProxy.PreferredSlug == PreferredSlug.B)
//                     {
//                         if (_CanReleaseToSlug(SlugLetter.B))
//                         {
//                             slugLetter = SlugLetter.B;
//                         }
//                         else if (_CanReleaseToSlug(SlugLetter.A))
//                         {
//                             slugLetter = SlugLetter.A;
//                         }
//                     }
//                     break;
//             }
//             return slugLetter != SlugLetter.None;
//         }

//         private bool _HasShortage(Load load)
//         {
//             InventoryCounts bank1Counts = new InventoryCounts();
//             InventoryCounts bank2Counts = new InventoryCounts();
//             InventoryCounts pitCounts = new InventoryCounts();
//             _storageProxy.GetInventoryCounts(ref bank1Counts, ref bank2Counts);
//             _pitProxy.GetInventoryCounts(ref pitCounts, ref bank2Counts);
// 
//             _slugAProxy.AllocateInventoryForLoadCheck(
//                 bank1Counts,
//                 bank2Counts,
//                 pitCounts);
// 
//             _slugBProxy.AllocateInventoryForLoadCheck(
//                 bank1Counts,
//                 bank2Counts,
//                 pitCounts);
// 
//             foreach (LoadRequirement requirement in load.LoadRequirements)
//             {
//                 bool skuShortage = false;
//                 // Process KitCode
//                 bool kitCodeShortage = InventoryCounts.IsShortage(
//                     bank1Counts,
//                     requirement.KitCode);
//                 if (kitCodeShortage)
//                 {
//                     kitCodeShortage = InventoryCounts.IsShortage(
//                         pitCounts,
//                         requirement.KitCode);
//                     if (kitCodeShortage)
//                     {
//                         kitCodeShortage = InventoryCounts.IsShortage(
//                             bank2Counts,
//                             requirement.KitCode);
//                         if (kitCodeShortage)
//                         {
//                             // Process SKU
//                             skuShortage = InventoryCounts.IsShortage(
//                                 bank1Counts,
//                                 requirement.Sku);
//                             if (skuShortage)
//                             {
//                                 skuShortage = InventoryCounts.IsShortage(
//                                     pitCounts,
//                                     requirement.Sku);
//                                 if (skuShortage)
//                                 {
//                                     skuShortage = InventoryCounts.IsShortage(
//                                         bank2Counts,
//                                         requirement.Sku);
//                                 }
//                             }
//                         }
//                     }
//                 }
//                 if (!kitCodeShortage)
//                 {
//                     return false;
//                 }
//                 if (skuShortage)
//                 {
//                     return true;
//                 }
//                 bool rearSkuShortage = false;
//                 if (!skuShortage && requirement.HasRear)
//                 {
//                     // Process Rear SKU
//                     rearSkuShortage = InventoryCounts.IsShortage(
//                         bank1Counts,
//                         requirement.RearSku);
//                     if (rearSkuShortage)
//                     {
//                         rearSkuShortage = InventoryCounts.IsShortage(
//                             pitCounts,
//                             requirement.RearSku);
//                         if (rearSkuShortage)
//                         {
//                             rearSkuShortage = InventoryCounts.IsShortage(
//                                 bank2Counts,
//                                 requirement.RearSku);
//                         }
//                     }
//                 }
//                 return rearSkuShortage;
//             }
//             return false;
//         }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateLoadNumbers();
        }

        private void _SystemSettings_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateLoadNumbers();
        }

        private void _UpdateLoadNumbers()
        {
            _lblLoadANumber.Text = _slugAProxy.Cleared
                ? string.Empty
                : _systemSettingsProxy.SlugALoadNumber.ToString();

            _lblLoadBNumber.Text = _slugBProxy.Cleared
                ? string.Empty
                : _systemSettingsProxy.SlugBLoadNumber.ToString();
        }

        private void _SlugA_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateAcceptLoadButtons();
            _UpdateAbortLoadButtons();
            _UpdateLoadNumbers();
            _UpdateLoadReleaseButton();
        }

        private void _SlugA_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateAcceptLoadButtons();
            _UpdateAbortLoadButtons();
            _UpdateLoadNumbers();
            _UpdateLoadReleaseButton();
        }

        private void _SlugB_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateAcceptLoadButtons();
            _UpdateAbortLoadButtons();
            _UpdateLoadNumbers();
            _UpdateLoadReleaseButton();
        }

        private void _SlugB_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateAcceptLoadButtons();
            _UpdateAbortLoadButtons();
            _UpdateLoadNumbers();
            _UpdateLoadReleaseButton();
        }

        private void _UpdateAcceptLoadButtons()
        {

            //             bool acceptLoadA1st = _showAcceptLoadButtons
            //                 && _slugAProxy.Completed
            //                 && (_slugAProxy.Items.Max(i => i.Requirement.BroadcastNumber)
            //                         < _slugBProxy.Items.Max(i => i.Requirement.BroadcastNumber)
            //                     || _slugBProxy.Cleared);
            // 
            //             bool acceptLoadB1st = _showAcceptLoadButtons
            //                 && _slugBProxy.Completed
            //                 && (_slugBProxy.Items.Max(i => i.Requirement.BroadcastNumber)
            //                         < _slugAProxy.Items.Max(i => i.Requirement.BroadcastNumber)
            //                     || _slugAProxy.Cleared);

            bool acceptLoadA1st = _showAcceptLoadButtons
                && _slugAProxy.Completed;
            //                 && (_slugAProxy.Items.Max(i => i.Requirement.BroadcastNumber)
            //                         < _slugBProxy.Items.Max(i => i.Requirement.BroadcastNumber)
            //                     || _slugBProxy.Cleared);

            bool acceptLoadB1st = _showAcceptLoadButtons
                && _slugBProxy.Completed;
//                 && (_slugBProxy.Items.Max(i => i.Requirement.BroadcastNumber)
//                         < _slugAProxy.Items.Max(i => i.Requirement.BroadcastNumber)
//                     || _slugAProxy.Cleared);

            _btnAcceptLoadA.Enabled = acceptLoadA1st;
            _btnAcceptLoadB.Enabled = acceptLoadB1st;

            _btnAcceptLoadA.Visible = _showAcceptLoadButtons;
            _btnAcceptLoadB.Visible = _showAcceptLoadButtons;
        }

        private void _UpdateLoadReleaseButton()
        {
            // This requires some checks. If a load with smaller CSNs needs to be aborted, then both
            // loads must be aborted.
            _navigatorBtnReleaseBroadcast.Visible = _parameters.AllowReleaseBroadcast;
            _navigatorBtnReleaseBroadcast.Enabled = _parameters.AllowReleaseBroadcast
                && (_slugAProxy.Cleared || _slugBProxy.Cleared);
        }

        private void _UpdateAbortLoadButtons()
        {
            // This requires some checks. If a load with smaller CSNs needs to be aborted, then both
            // loads must be aborted.
            _btnAbortLoadA.Enabled = _showAbortLoadButtons && !_slugAProxy.Cleared;
            _btnAbortLoadB.Enabled = _showAbortLoadButtons && !_slugBProxy.Cleared;
            _btnAbortLoadA.Visible = _showAbortLoadButtons;
            _btnAbortLoadB.Visible = _showAbortLoadButtons;
        }

        private void _BtnAcceptLoadA_Click(object sender, EventArgs e)
        {
            if (!_slugAProxy.Completed)
            {
                _btnAcceptLoadA.Enabled = false;
                return;
            }
            _AcceptLoad(_slugAProxy.SlugLetter);
        }

        private void _BtnAcceptLoadB_Click(object sender, EventArgs e)
        {
            if (!_slugBProxy.Completed)
            {
                _btnAcceptLoadB.Enabled = false;
                return;
            }
            _AcceptLoad(_slugBProxy.SlugLetter);
        }

        private void _BtnAbortLoadA_Click(object sender, EventArgs e)
        {
            _AbortLoad(_slugAProxy.SlugLetter);
        }

        private void _BtnAbortLoadB_Click(object sender, EventArgs e)
        {
            _AbortLoad(_slugBProxy.SlugLetter);
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            // This will refresh the Load Proxy Items
            // which will in turn update the two grids.
            ParentForm.Cursor = Cursors.WaitCursor;
            XMessaging.Publish(
                Constant.CalculateShortagesMessageTopicName,
                XMessageScopes.All);
            _slugAProxy.Refresh();
            _slugBProxy.Refresh();

            ParentForm.Cursor = Cursors.Default;
        }

        private void _AcceptLoad(SlugLetter slugLetter)
        {
            AcceptLoadConfirmationDlg dialog = new AcceptLoadConfirmationDlg(slugLetter);
            if (dialog.ShowDialog(this) == DialogResult.Yes)
            {
                ParentForm.Cursor = Cursors.WaitCursor;
                AcceptLoadMessageData responseMessageData;
                try
                {
                    if (!XMessaging.SyncPublish(
                        out responseMessageData,
                        20000,
                        AcceptLoadMessageData.AcceptLoadMessageTopic,
                        new AcceptLoadMessageData(
                            slugLetter,
                            XSystemEvent.Create(
                                ViewName,
                                XSystemEventLevel.Manual,
                                $"Slug {slugLetter.ToText()} Load Accepted.")),
                        XMessageScopes.All,
                        this))
                    {
                        string message = $"Request to ACCEPT the Load on Slug {slugLetter.ToText()} has timed out. The operation may still have completed correctly.";
                        XSystemEvent.Publish(
                            ViewName,
                            XSystemEventLevel.Error,
                            message);
                        MessageBox.Show(
                            this,
                            message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                finally
                {
                    ParentForm.Cursor = Cursors.Default;
                }
                if (!string.IsNullOrWhiteSpace(responseMessageData.Error))
                {
                    MessageBox.Show(
                        this,
                        responseMessageData.Error,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void _AbortLoad(SlugLetter slugLetter)
        {
            string slugDisplayName = slugLetter.SlugDisplayName();
            AbortLoadConfirmationDlg dialog = new AbortLoadConfirmationDlg(slugDisplayName);
            if (dialog.ShowDialog(this) == DialogResult.Yes)
            {
                ParentForm.Cursor = Cursors.WaitCursor;
                AbortLoadMessageData responseMessageData;
                try
                {
                    if (!XMessaging.SyncPublish(
                        out responseMessageData,
                        20000,
                        AbortLoadMessageData.AbortLoadMessageTopic,
                        new AbortLoadMessageData(
                            slugLetter,
                            dialog.RecoverBroadcast,
                            XSystemEvent.Create(
                                ViewName,
                                XSystemEventLevel.Manual,
                                $"{slugDisplayName} Aborted.")),
                        XMessageScopes.All,
                        this))
                    {
                        string message = $"Request to abort {slugDisplayName} has timed out. The operation may still have completed correctly.";
                        XSystemEvent.Publish(
                            ViewName,
                            XSystemEventLevel.Error,
                            message);
                        MessageBox.Show(
                            this,
                            message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                finally
                {
                    ParentForm.Cursor = Cursors.Default;
                }
                if (!string.IsNullOrWhiteSpace(responseMessageData.Error))
                {
                    MessageBox.Show(
                        this,
                        responseMessageData.Error,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            dialog.Dispose();
        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            if (!force)
            {
                // Check for edits. If so,
                // ask user to save/not save/cancel
                // and return accordingly
            }
            _slugAGridUpper.Dispose();
            _slugAGridLower.Dispose();
            _slugBGridUpper.Dispose();
            _slugBGridLower.Dispose();
            if (_systemSettingsProxy != null)
            {
                _systemSettingsProxy.DataItemChanged -= _SystemSettings_DataItemChanged;
                _systemSettingsProxy.CollectionRefreshed -= _SystemSettings_CollectionRefreshed;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
            if (_slugAProxy != null)
            {
                _slugAProxy.DataItemChanged -= _SlugA_DataItemChanged;
                _slugAProxy.CollectionRefreshed -= _SlugA_CollectionRefreshed;
                XProxyCache.Release(_slugAProxy);
                _slugAProxy = null;
            }
            if (_slugBProxy != null)
            {
                _slugBProxy.DataItemChanged -= _SlugB_DataItemChanged;
                _slugBProxy.CollectionRefreshed -= _SlugB_CollectionRefreshed;
                XProxyCache.Release(_slugBProxy);
                _slugBProxy = null;
            }
            if (_upperPitProxy != null)
            {
//                 _upperPitProxy.DataItemChanged -= _UpperPit_DataItemChanged;
//                 _upperPitProxy.CollectionRefreshed -= _UpperPit_CollectionRefreshed;
                XProxyCache.Release(_upperPitProxy);
                _upperPitProxy = null;
            }
            if (_lowerPitProxy != null)
            {
//                 _lowerPitProxy.DataItemChanged -= _LowerPit_DataItemChanged;
//                 _lowerPitProxy.CollectionRefreshed -= _LowerPit_CollectionRefreshed;
                XProxyCache.Release(_lowerPitProxy);
                _lowerPitProxy = null;
            }
            if (_storageProxy != null)
            {
//                 _storageProxy.DataItemChanged -= _Storage_DataItemChanged;
//                 _storageProxy.CollectionRefreshed -= _Storage_CollectionRefreshed;
                XProxyCache.Release(_storageProxy);
                _storageProxy = null;
            }
            return true;
        }

        private void _NavigatorBtnReleaseBroadcast_Click(object sender, EventArgs e)
        {

        }

//         private void _NavigatorBtnReleaseNewLoad_Click(object sender, EventArgs e)
//         {
//             try
//             {
//                 if (!MesQuery.TryGetPickableLoads(
//                     out List<Load> pickableLoads,
//                     out string error))
//                 {
//                     _ = MessageBox.Show(
//                         error,
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                     return;
//                 }
// 
//                 if (!pickableLoads.Any())
//                 {
//                     _ = MessageBox.Show(
//                         "There are no Loads ready for Release.",
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                     return;
//                 }
//                 else if (pickableLoads.First().LoadRequirements == null)
//                 {
//                     _ = MessageBox.Show(
//                         "Load Requirements is NULL!",
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                     return;
//                 }
// //                 if (!_TryReleaseLoad(pickableLoads.First(), out error))
// //                 {
// //                     _ = MessageBox.Show(
// //                         error,
// //                         "Error",
// //                         MessageBoxButtons.OK,
// //                         MessageBoxIcon.Error);
// //                 }
//             }
//             catch (Exception x)
//             {
//                 _ = MessageBox.Show(
//                     x.Message,
//                     "Exception",
//                     MessageBoxButtons.OK,
//                     MessageBoxIcon.Error);
//             }
//         }

        //public override void ViewClosed()
        //{
        //}

        //protected override MenuStrip GetViewMenuStrip()
        //{
        //    return null;
        //}

        //protected override bool ProcessEnterKey()
        //{
        //    return false;
        //}

        //protected override bool ProcessEscapeKey()
        //{
        //    return false;
        //}

    }
}
