using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mss.Common;
using Mss.Collections;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Operations;
using DacQuest.DFX.SnapInViews;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.Strings;

namespace Mss.Views
{
    public partial class LoadDirectorView : XOperationUIView
    {
        private LoadDirectorViewParameterSetWrapper _parameters;
        private SlugProxy _slugAProxy;
        private SlugProxy _slugBProxy;
        private SystemSettingsProxy _systemSettingsProxy;
//        private BroadcastProxy _broadcastProxy;

        public LoadDirectorView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {

            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
//            _systemSettingsProxy.DataItemChanged += _SystemSettingsChangedHandler;

            XProxyCache.Acquire(Constant.SlugBName, out _slugBProxy);
            _slugBProxy.DataItemChanged += _SlugB_DataItemChanged;

            XProxyCache.Acquire(Constant.SlugAName, out _slugAProxy);
            _slugAProxy.DataItemChanged += _SlugA_DataItemChanged;
            _slugAProxy.CollectionRefreshed += _SlugA_CollectionRefreshed;
            _slugAProxy.Refresh();

//            if (_parameters.Level == OutboundLevel.Lower)
//            {
//                loadGridUpper.Visible = false;
//                loadGridLower.Visible = true;
//            }
//            else
//            {
//                loadGridLower.Visible = false;
//                loadGridUpper.Visible = true;
//            }

            _slugAGrid.Initialize(
                SlugLetter.A,
               _parameters.Level,
                _parameters.AllowReprintLabel,
                false,
                false,
                false,
                _parameters.ShowShortages,
                _parameters.FlashGridCellTimeoutSeconds);

            _slugBGrid.Initialize(
                SlugLetter.B,
               _parameters.Level,
                _parameters.AllowReprintLabel,
                false,
                false,
                false,
                _parameters.ShowShortages,
                _parameters.FlashGridCellTimeoutSeconds);

            base.OpenView();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            base.ProcessParameters(parameters);
            _parameters = parameters as LoadDirectorViewParameterSetWrapper;
        }

        protected override void ProcessUIMessage(XOperationUIMessageData uiMessageData)
        {
//             if (uiMessageData != null)
//             {
//                 PalletItem palletItem = uiMessageData.GetMessageValue<PalletItem>(Constant.LD_PalletItemName);
//                 if (palletItem == null)
//                 {
//                     _lblPalletID.Text = string.Empty;
//                     _lblSku.Text = string.Empty;
//                     _lblJobID.Text = string.Empty;
//                     _EnableAcceptButton(false);
//                     _EnableRejectButton(false);
//                     _lblSku.ForeColor = Color.Black;
//                 }
//                 else
//                 {
//                     _lblPalletID.Text = palletItem.PalletID;
//                     _lblSku.Text = palletItem.Sku;
//                     _lblJobID.Text = palletItem.JobID;
//                     bool isLoadPallet = uiMessageData.GetMessageValue<bool>(Constant.LD_IsLoadPalletName);
//                     bool isAutoMode = uiMessageData.GetMessageValue<bool>(Constant.LD_IsAutoModeName);
//                     _EnableAcceptButton(isLoadPallet && !isAutoMode);
//                     _EnableRejectButton(isLoadPallet && !isAutoMode);
//                     if (!isLoadPallet
//                         && palletItem.JobID.IsNullOrWhiteSpace()
//                         && palletItem.Sku.IsNullOrWhiteSpace())
//                     {
//                         _lblSku.Text = "PURGE";
//                         _lblSku.ForeColor = Color.Red;
//                     }
//                     else
//                     {
//                         _lblSku.ForeColor = Color.Black;
//                     }
//                 }
//             }
        }

        protected override void ProcessStateMessage(XOperationStateMessageData messageData)
        {
            string stateText;
            XOperationStatus status = messageData.Status;
            if (status == XOperationStatus.Shutdown
                || status == XOperationStatus.Stopped)
            {
//                stateText = XEnum.GetText(status);
                stateText = status.ToText();
                _lblPalletID.Text = string.Empty;
                _lblSku.Text = string.Empty;
                _lblJobID.Text = string.Empty;
                _EnableAcceptButton(false);
                _EnableRejectButton(false);
                _lblSku.ForeColor = Color.Black;
            }
            else if (status == XOperationStatus.Faulted
                || status == XOperationStatus.Paused)
            {
//                stateText = XEnum.GetText(status);
                stateText = status.ToText();
                _EnableAcceptButton(false);
                _EnableRejectButton(false);
                _lblSku.ForeColor = Color.Black;
            }
            else
            {
                if (messageData.ExtendedState.IsNullOrWhiteSpace())
                {
                    stateText = messageData.State;
                }
                else
                {
                    stateText = string.Format(
                        "{0}: {1}",
                        messageData.State,
                        messageData.ExtendedState);
                }
            }
            _lblState.Text = stateText;
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
//             XMessaging.Publish(
//                 Constant.CalculateShortagesMessageTopicName,
//                 XMessageScopes.All);
//             _systemSettingsProxy.Refresh();
//             _slugBProxy.Refresh();
//             _slugAProxy.Refresh();
//             RequestStateRefresh();
//             // Request Pallet Data
//             XOperationUIMessageData messageData = new XOperationUIMessageData();
//             messageData.SetMessageValue(Constant.LD_RequestPalletDataName, true);
//             SendUIMessage(messageData);
        }

        private void navigatorBtnOperations_Click(object sender, EventArgs e)
        {
//            SwitchToView(Constant.LD_OperationView);
        }

        private void _NavigatorBtnAcceptCurrentLoad_Click(object sender, EventArgs e)
        {
//             if (MessageBox.Show(
//                 this,
//                 "Are you sure you want to ACCEPT the Current Load?",
//                 "Accept Current Load",
//                 MessageBoxButtons.YesNo,
//                 MessageBoxIcon.Warning,
//                 MessageBoxDefaultButton.Button2) == DialogResult.Yes)
//             {
//                 ParentForm.Cursor = Cursors.WaitCursor;
//                 AcceptLoadMessageData responseMessageData;
//                 try
//                 {
//                     if (!XMessaging.SyncPublish(
//                         out responseMessageData,
//                         10000,
//                         AcceptLoadMessageData.AcceptLoadMessageTopic,
//                         new AcceptLoadMessageData(
//                             XSystemEvent.Create(
//                                 "LoadView",
//                                 XSystemEventLevel.Manual,
//                                 "Current Load Accepted.")),
//                         XMessageScopes.All,
//                         this))
//                     {
//                         string message = "Request to accept Current Load has timed out. The operation may still have completed correctly.";
//                         XSystemEvent.Publish(
//                             "LoadView",
//                             XSystemEventLevel.Error,
//                             message);
//                         MessageBox.Show(
//                             this,
//                             message,
//                             "Error",
//                             MessageBoxButtons.OK,
//                             MessageBoxIcon.Error);
//                         return;
//                     }
//                 }
//                 finally
//                 {
//                     ParentForm.Cursor = Cursors.Default;
//                 }
//                 if (!responseMessageData.ErrorMessage.IsNullOrWhiteSpace())
//                 {
//                     MessageBox.Show(
//                         this,
//                         responseMessageData.ErrorMessage,
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                 }
//             }
        }

//        private void _BroadcastRefreshedHandler(Object sender, EventArgs eventArgs)
//        {
//        }

//        private void _BroadcastItemChangedHandler(Object sender, XDataItemChangedEventArgs eventArgs)
//        {
//        }

        private void _SlugA_CollectionRefreshed(Object sender, EventArgs eventArgs)
        {
            _UpdateAcceptCurrentLoadButton();
        }

        private void _SlugA_DataItemChanged(Object sender, XDataItemChangedEventArgs eventArgs)
        {
//            _UpdateReleaseButton();
            _UpdateAcceptCurrentLoadButton();
        }

        private void _SlugB_DataItemChanged(Object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateAcceptCurrentLoadButton();
        }

//        private void _SystemSettingsChangedHandler(Object sender, XDataItemChangedEventArgs eventArgs)
//        {
//        }

        private void _UpdateAcceptCurrentLoadButton()
        {
        }

//        public int ReleasableBroadcastItemCount
//        {
//            get
//            {
//                int count = 0;
//                IEnumerable<BroadcastItem> broadcastItems = _broadcastProxy.GetCurrentBroadcastItems(
//                    _systemSettingsProxy.LastSequenceNumberReleased,
//                    _systemSettingsProxy.LargestBroadcastNumberReceived);
//                foreach (var item in broadcastItems)
//                {
//                    BroadcastStatus status = item.Status;
//                    if (status == BroadcastStatus.OK)
//                    {
//                        count++;
//                    }
//                    else if (status != BroadcastStatus.Skip)
//                    {
//                        break;
//                    }
//                }
//                return count;
//            }
//        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            _slugAGrid.Dispose();
//            loadGridLower.Dispose();
            if (_slugBProxy != null)
            {
                _slugBProxy.DataItemChanged -= _SlugB_DataItemChanged;
                XProxyCache.Release(_slugBProxy);
                _slugBProxy = null;
            }
            if (_slugAProxy != null)
            {
                _slugAProxy.DataItemChanged -= _SlugA_DataItemChanged;
                _slugAProxy.CollectionRefreshed -= _SlugA_CollectionRefreshed;
                XProxyCache.Release(_slugAProxy);
                _slugAProxy = null;
            }
            if (_systemSettingsProxy != null)
            {
//                _systemSettingsProxy.DataItemChanged -= _SystemSettingsChangedHandler;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
            return true;
        }

        private void _BtnAccept_Click(object sender, EventArgs e)
        {
//             _EnableAcceptButton(false);
//             _EnableRejectButton(false);
// 
//             XOperationUIMessageData messageData = new XOperationUIMessageData();
//             messageData.SetMessageValue(Constant.LD_OperatorResponseName, LoadDirectorOperatorResponse.Accept);
// //            messageData.PublishOperationUIMessage();
//             SendUIMessage(messageData);
        }

        private void _BtnReject_Click(object sender, EventArgs e)
        {
//             _EnableAcceptButton(false);
//             _EnableRejectButton(false);
// 
//             XOperationUIMessageData messageData = new XOperationUIMessageData();
//             messageData.SetMessageValue(Constant.LD_OperatorResponseName, LoadDirectorOperatorResponse.Reject);
// //            messageData.PublishOperationUIMessage();
//             SendUIMessage(messageData);
        }

        private void _EnableAcceptButton(bool enable)
        {
            _btnAccept.Enabled = enable;
            _btnAccept.BackColor = enable ? Color.Lime : SystemColors.Control;
        }

        private void _EnableRejectButton(bool enable)
        {
            _btnReject.Enabled = enable;
            _btnReject.BackColor = enable ? Color.Red : SystemColors.Control;
        }

//        private void _SendUIMessage(XOperationUIMessageData messageData)
//        {
//            this
//            XMessaging.Publish(
//                XOperationUIMessageData.OperationUIMessageFromUITopic,
//                messageData,
//                XMessageScopes.All,
//                this);
//        }

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
