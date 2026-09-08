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
        private bool _isAwaitingOperatorResponsState = false;

        public LoadDirectorView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {

            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
           _systemSettingsProxy.DataItemChanged += _SystemSettings_DataItemChanged;

            XProxyCache.Acquire(Constant.SlugAName, out _slugAProxy);
            _slugAProxy.DataItemChanged += _SlugA_DataItemChanged;
            _slugAProxy.CollectionRefreshed += _SlugA_CollectionRefreshed;

            XProxyCache.Acquire(Constant.SlugBName, out _slugBProxy);
            _slugBProxy.DataItemChanged += _SlugB_DataItemChanged;
            _slugBProxy.CollectionRefreshed += _SlugB_CollectionRefreshed;

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
            if (uiMessageData != null)
            {
                LoadItem loadItem = uiMessageData.GetMessageValue<LoadItem>(Constant.LD_LoadItemName);
                PalletItem palletItem = uiMessageData.GetMessageValue<PalletItem>(Constant.LD_PalletItemName);
                bool isStack = uiMessageData.GetMessageValue<bool>(Constant.LD_IsStackName);
                bool autoReleaseNonLoadPallets = uiMessageData.GetMessageValue<bool>(Constant.LD_AutoReleaseNonLoadPalletsName);
                _isAwaitingOperatorResponsState = uiMessageData.GetMessageValue<bool>(Constant.LD_IsAwaitingOperatorResponseName);
                bool isLoadPallet = false;
                if (palletItem == null)
                {
                    _lblPalletID.Text = string.Empty;
                    _lblSku.Text = string.Empty;
                    _lblJobID.Text = string.Empty;
                    _lblPurge.Visible = false;
                    _lblStack.Visible = false;
                    _EnableAcceptButton(false);
                    _EnableRejectButton(false);
                }
                else if (loadItem == null)
                {
                    _lblPalletID.Text = palletItem.PalletID;
                    _lblSku.Text = palletItem.Sku;
                    _lblJobID.Text = palletItem.JobID;
                    _lblPurge.Visible = !isStack;
                    _lblStack.Visible = isStack;
                    _EnableAcceptButton(false);
                    _EnableRejectButton(!autoReleaseNonLoadPallets);
                }
                else 
                {
                    isLoadPallet = true;
                    _lblPalletID.Text = palletItem.PalletID;
                    _lblSku.Text = palletItem.Sku;
                    _lblJobID.Text = palletItem.JobID;
                    _lblPurge.Visible = false;
                    _lblStack.Visible = false;
                    _EnableAcceptButton(isLoadPallet);
                    _EnableRejectButton(isLoadPallet);
                }
            }
        }

        protected override void ProcessStateMessage(XOperationStateMessageData messageData)
        {
            string stateText;
            XOperationStatus status = messageData.Status;
            if (status == XOperationStatus.Shutdown
                || status == XOperationStatus.Stopped)
            {
                stateText = status.ToText();
                _lblPalletID.Text = string.Empty;
                _lblSku.Text = string.Empty;
                _lblJobID.Text = string.Empty;
                _EnableAcceptButton(false);
                _EnableRejectButton(false);
                _lblState.ForeColor = Color.Black;
                _lblState.BackColor = Color.White;
            }
            else if (status == XOperationStatus.Faulted)
            {
                stateText = status.ToText();
                _EnableAcceptButton(false);
                _EnableRejectButton(false);
                _lblState.ForeColor = Color.Yellow;
                _lblState.BackColor = Color.Red;
            }
            else if (status == XOperationStatus.Paused)
            {
                stateText = status.ToText();
                _EnableAcceptButton(false);
                _EnableRejectButton(false);
                _lblState.ForeColor = Color.Black;
                _lblState.BackColor = Color.White;
            }
            else
            {
                _lblState.ForeColor = Color.Black;
                _lblState.BackColor = _isAwaitingOperatorResponsState
                    ? Color.Yellow
                    : Color.White;
                stateText = _isAwaitingOperatorResponsState
                    ? "Awaiting Operator Response"
                    : messageData.State;
                stateText = messageData.ExtendedState.IsNullOrWhiteSpace()
                    ? stateText
                    : string.Format(
                        "{0}: {1}",
                        messageData.State,
                        messageData.ExtendedState);
            }
            _lblState.Text = stateText;
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _systemSettingsProxy.Refresh();
            _slugAProxy.Refresh();
            _slugBProxy.Refresh();
            // Request Pallet Data
            XOperationUIMessageData messageData = new XOperationUIMessageData();
            messageData.SetMessageValue(Constant.LD_RequestPalletDataName, true);
            SendUIMessage(messageData);

            RequestStateRefresh();
        }

        private void _SlugA_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
        }

        private void _SlugA_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
        }

        private void _SlugB_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
        }

        private void _SlugB_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
        }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
       {
       }

        public override bool ViewClosing(bool force)
        {
            if (_systemSettingsProxy != null)
            {
               _systemSettingsProxy.DataItemChanged -= _SystemSettings_DataItemChanged;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
            _slugAGrid.Dispose();
            if (_slugAProxy != null)
            {
                _slugAProxy.DataItemChanged -= _SlugA_DataItemChanged;
                _slugAProxy.CollectionRefreshed -= _SlugA_CollectionRefreshed;
                XProxyCache.Release(_slugAProxy);
                _slugAProxy = null;
            }
            _slugBGrid.Dispose();
            if (_slugBProxy != null)
            {
                _slugBProxy.DataItemChanged -= _SlugB_DataItemChanged;
                _slugBProxy.CollectionRefreshed -= _SlugB_CollectionRefreshed;
                XProxyCache.Release(_slugBProxy);
                _slugBProxy = null;
            }
            return true;
        }

        private void _BtnAccept_Click(object sender, EventArgs e)
        {
            _EnableAcceptButton(false);
            _EnableRejectButton(false);

            XOperationUIMessageData messageData = new XOperationUIMessageData();
            messageData.SetMessageValue(Constant.LD_OperatorResponseName, LoadDirectorOperatorResponse.Accept);
            SendUIMessage(messageData);
        }

        private void _BtnReject_Click(object sender, EventArgs e)
        {
            _EnableAcceptButton(false);
            _EnableRejectButton(false);

            XOperationUIMessageData messageData = new XOperationUIMessageData();
            messageData.SetMessageValue(Constant.LD_OperatorResponseName, LoadDirectorOperatorResponse.Reject);
            SendUIMessage(messageData);
        }

        private void _EnableAcceptButton(bool enable)
        {
            _btnAccept.Enabled = enable;
            _btnAccept.BackColor = enable
                ? Color.Lime
                : SystemColors.Control;
        }

        private void _EnableRejectButton(bool enable)
        {
            _btnReject.Enabled = enable;
            _btnReject.BackColor = enable
                ? Color.Red
                : SystemColors.Control;
        }

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
