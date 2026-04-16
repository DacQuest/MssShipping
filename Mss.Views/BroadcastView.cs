using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.SnapInViews;
using Mss.Common;
using Mss.Collections;
using DacQuest.DFX.Core;
using System.IO;

namespace Mss.Views
{
    public partial class BroadcastView : XSnapInView
    {
        private BroadcastViewParameterSetWrapper _parameters;
//        private List<BroadcastItem> _broadcastItems;
        private BindingSource _bindingSource = new BindingSource();
        private BroadcastProxy _broadcastProxy;
        private SystemSettingsProxy _systemSettingsProxy;
        private SlugProxy _slugAProxy;
        private SlugProxy _slugBProxy;
        private StorageProxy _storageProxy;


        public BroadcastView()
        {
            InitializeComponent();

            _broadcastGrid.DataSource = _bindingSource;
            _broadcastGrid.AutoGenerateColumns = false;
            _broadcastGrid.AutoSize = false;
            _broadcastGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            XProxyCache.Acquire(
                Constant.BroadcastName,
                Constant.CurrentBroadcastQuery,
                out _broadcastProxy);
            _broadcastProxy.DataItemChanged += _BroadcastItemChangedHandler;
            _broadcastProxy.CollectionRefreshed += _BroadcastRefreshedHandler;

            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
            _systemSettingsProxy.DataItemChanged += _SystemSettingsChangedHandler;

            XProxyCache.Acquire(Constant.SlugAName, out _slugAProxy);
            _slugAProxy.DataItemChanged += _SlugAProxy_DataItemChanged;

            XProxyCache.Acquire(Constant.SlugBName, out _slugBProxy);
            _slugBProxy.DataItemChanged += _SlugBProxy_DataItemChanged;

            XProxyCache.Acquire(Constant.StorageName, out _storageProxy);
            _storageProxy.DataItemChanged += _StorageChangedHandler;

        }

        protected override void OpenView()
        {
            DataGridViewImageColumn imageColumn;
            DataGridViewTextBoxColumn column;

            imageColumn = new DataGridViewImageColumn
            {
                DataPropertyName = "StatusImage",
                HeaderText = "",
                Name = "StatusImageColumn"
            };
            _ = _broadcastGrid.Columns.Add(imageColumn);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "StatusText",
                Name = "StatusColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "CSN",
                DataPropertyName = "Csn",
                Name = "CsnColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "SKU",
                DataPropertyName = "Sku",
                Name = "SkuColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "VIN",
                DataPropertyName = "Vin",
                Name = "VinColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Pick Mode",
                DataPropertyName = "PickMode",
                Name = "PickModeColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Pick Mode Key",
                DataPropertyName = "PickModeKey",
                Name = "PickModeKeyColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Received On",
                DataPropertyName = "ReceivedOnText",
                Name = "ReceivedOnColumn",
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            _ = _broadcastGrid.Columns.Add(column);

//             _navigatorBtnEdit.Visible = _parameters.AllowEdit;
//             _navigatorBtnRelease.Visible = _parameters.AllowRelease;
            _navigatorBtnRecover.Visible = _parameters.AllowRecover;

            _UpdateGrid();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = parameters as BroadcastViewParameterSetWrapper;
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _storageProxy.Refresh();
            _slugAProxy.Refresh();
            _systemSettingsProxy.Refresh();
            _broadcastProxy.Refresh();
        }

        private void _SystemSettingsChangedHandler(Object sender, XDataItemChangedEventArgs eventArgs)
        {
//            _broadcastProxy.Refresh();
//            _UpdateGrid();
        }

        private void _StorageChangedHandler(Object sender, XDataItemChangedEventArgs eventArgs)
        {
//            _broadcastProxy.Refresh();
//            _UpdateGrid();
        }

        private void _BroadcastItemChangedHandler(Object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _BroadcastRefreshedHandler(Object sender, EventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _SlugAProxy_DataItemChanged(Object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

        private void _SlugBProxy_DataItemChanged(Object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

        private void _UpdateGrid()
        {
//            _broadcastItems = _broadcastProxy.GetCurrentBroadcastItems(
//                _systemSettingsProxy.LastSequenceNumberReleased,
//                _systemSettingsProxy.LargestBroadcastNumberReceived);
            List<BroadcastItem> broadcastItems = _broadcastProxy.Values;

//             Int32 topAvailableCount = _CalculateShortages(_broadcastItems);
//             Int32 topAvailableCount = _CalculateShortages(broadcastItems);

            int topAvailableCount = broadcastItems.TakeWhile(b => !b.Shortage).Count();

            topAvailableCount -= topAvailableCount % 6;
//            if (topAvailableCount % 2 == 1)
//            {
//                topAvailableCount--;
//            }

            if (topAvailableCount > 0)
            {
//                lblTitle.Text = String.Format(
//                    "Broadcast   ( {0} of {1} broadcasts can be shipped )",
//                    topAvailableCount,
//                    _broadcastItems.Count);
                _lblTitle.Text = String.Format(
                    "Broadcast   ( {0} of {1} broadcasts can be shipped )",
                    topAvailableCount,
                    broadcastItems.Count);
            }
            else
            {
                _lblTitle.Text = "Broadcast   ( No broadcasts can be shipped )";
            }

//            navigatorLblCount.Text = String.Format("Count:  {0}", _broadcastItems.Count);
            _navigatorLblCount.Text = String.Format("Count:  {0}", broadcastItems.Count);

//            _bindingSource.DataSource = _broadcastItems;
            _bindingSource.DataSource = broadcastItems;
            _broadcastGrid.Update();
            _UpdateRecoverButton();
            _UpdateReleaseButton();
        }

//         private Int32 _CalculateShortages(List<BroadcastItem> broadcastItems)
//         {
//             Boolean doneCounting = false;
//             Int32 availableCount = 0;
//             List<LoadItem> loadItems = _currentLoadProxy.Items;
//             List<LaneItem> laneItems = _storageProxy.Items;
//             Boolean pickByShortTrimCode = _systemSettingsProxy.PickByShortTrimCode;
//             Dictionary<String, Int32> trimCodeCounts = new Dictionary<String, Int32>();
//             foreach (var laneItem in laneItems)
//             {
//                 laneItem.GetTrimCodeCounts(
//                     PalletStatus.OK | PalletStatus.Inspect,
//                     pickByShortTrimCode,
//                     trimCodeCounts);
//             }
//             foreach (var loadItem in loadItems)
//             {
//                 LoadItemStatus loadItemStatus = loadItem.Status;
//                 if (loadItemStatus == LoadItemStatus.AwaitingAssignable
//                     || loadItemStatus == LoadItemStatus.Assignable
//                     || loadItemStatus == LoadItemStatus.AssignmentPending)
//                 {
//                     String trimCode =
//                         pickByShortTrimCode
//                         ? loadItem.Broadcast.ShortTrimCode
//                         : loadItem.Broadcast.TrimCode;
//                     Int32 count;
//                     if (trimCodeCounts.TryGetValue(trimCode, out count))
//                     {
//                         trimCodeCounts[trimCode] = --count;
//                     }
//                 }
//             }
//             foreach (var broadcastItem in broadcastItems)
//             {
//                 String trimCode =
//                     pickByShortTrimCode
//                     ? broadcastItem.ShortTrimCode
//                     : broadcastItem.TrimCode;
//                 Int32 count;
//                 if (trimCodeCounts.TryGetValue(trimCode, out count)
//                     && (broadcastItem.Status == BroadcastStatus.OK
//                         || broadcastItem.Status == BroadcastStatus.Hold))
//                 {
//                     broadcastItem.Available = count > 0;
//                     trimCodeCounts[trimCode] = --count;
//                     if (broadcastItem.Available && !doneCounting)
//                     {
//                         availableCount++;
//                     }
//                     else if (!broadcastItem.Available)
//                     {
//                         doneCounting = true;
//                     }
//                 }
//                 else
//                 {
//                     doneCounting = true;
//                     broadcastItem.Available = false;
//                 }
//             }
//             return availableCount;
//         }

        private void _UpdateReleaseButton()
        {
//             navigatorBtnRelease.Enabled
//                 = _parameters.AllowRelease
//                 && ReleasableBroadcastItemCount >= Constant.BroadcastReleaseMultiplier
//                 && (_slugAProxy.BroadcastReadyCount >= Constant.BroadcastReleaseMultiplier
//                     || _slugAProxy.Cleared);
        }

        private void _UpdateRecoverButton()
        {
            _navigatorBtnRecover.Enabled
                = _parameters.AllowRecover
//                    && _broadcastItems
                    && _broadcastProxy.Values
                        .Where(bi => bi.Status == BroadcastStatus.Shipped)
                        .Count() > 0;
        }

        private void _NavigatorBtnEdit_Click(object sender, EventArgs e)
        {
            _ = MessageBox.Show("Not implemented!");
        }

        private void _NavigatorBtnRelease_Click(object sender, EventArgs e)
        {
            _DoRelease();
        }

        private void _NavigatorBtnRecover_Click(object sender, EventArgs e)
        {
            _DoRecover();
        }

        private void _DoRelease()
        {
//             int releasableCount = ReleasableBroadcastItemCount;
//             int broadcastReadyCount;
//             if (_slugAProxy.Cleared)
//             {
//                 broadcastReadyCount = Constant.LoadSize;
//             }
//             else
//             {
//                 broadcastReadyCount = _slugAProxy.BroadcastReadyCount;
//             }
// 
//             if (broadcastReadyCount < releasableCount)
//             {
//                 releasableCount = broadcastReadyCount;
//             }
// 
//             if (releasableCount <= 0)
//             {
//                 return;
//             }
// 
//             DefineReleaseSizeDlg dialog = new DefineReleaseSizeDlg(releasableCount);
//             if (dialog.ShowDialog(this) == DialogResult.OK)
//             {
//                 ParentForm.Cursor = Cursors.WaitCursor;
//                 ReleaseBroadcastMessageData responseMessageData;
//                 try
//                 {
//                     ReleaseBroadcastMessageData messageData
//                         = new ReleaseBroadcastMessageData(
//                             dialog.ReleaseSize,
//                             XSystemEvent.Create(
//                                 "BroadcastView",
//                                 XSystemEventLevel.Manual,
//                                 String.Format(
//                                     "{0} Broadcast Items released to Current Load.",
//                                     dialog.ReleaseSize)));
//                     if (!XMessaging.SyncPublish(
//                         out responseMessageData,
//                         Constant.BroadcastReleaseTimeoutMilliseconds,
//                         ReleaseBroadcastMessageData.ReleaseBroadcastMessageTopic,
//                         messageData,
//                         XMessageScopes.All,
//                         this))
//                     {
//                         string message = string.Format(
//                             "Request to release {0} Broadcast Items to the Current Load has timed out. The operation may still have completed correctly.",
//                             dialog.ReleaseSize);
//                         XSystemEvent.Publish(
//                             "BroadcastView",
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
//                 if (!String.IsNullOrWhiteSpace(responseMessageData.ErrorMessage))
//                 {
//                     MessageBox.Show(
//                         this,
//                         responseMessageData.ErrorMessage,
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                 }
//                 else
//                 {
//                     if (_parameters.CloseAfterRelease)
//                     {
//                         CloseView();
//                     }
//                 }
//             }
        }


        private void _DoRecover()
        {
//             ParentForm.Cursor = Cursors.WaitCursor;
//             RecoverShippedBroadcastItemsMessageData responseMessageData;
//             try
//             {
//                 RecoverShippedBroadcastItemsMessageData messageData
//                         = new RecoverShippedBroadcastItemsMessageData(
//                             XSystemEvent.Create(
//                                 "BroadcastView",
//                                 XSystemEventLevel.Manual,
//                                 "Recovered eligible Broadcast Items."));
//                 if (!XMessaging.SyncPublish(
//                     out responseMessageData,
//                     Constant.BroadcastReleaseTimeoutMilliseconds,
//                     RecoverShippedBroadcastItemsMessageData.RecoverShippedBroadcastItemsMessageTopic,
//                     messageData,
//                     XMessageScopes.All,
//                     this))
//                 {
//                     string message = "Request to recover Broadcast Items has timed out. The operation may still have completed correctly.";
//                     XSystemEvent.Publish(
//                         "BroadcastView",
//                         XSystemEventLevel.Error,
//                         message);
//                     MessageBox.Show(
//                         this,
//                         message,
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                     return;
//                 }
//             }
//             finally
//             {
//                 ParentForm.Cursor = Cursors.Default;
//             }
//             if (responseMessageData.ErrorMessage != null)
//             {
//                 MessageBox.Show(
//                     this,
//                     responseMessageData.ErrorMessage,
//                     "Error",
//                     MessageBoxButtons.OK,
//                     MessageBoxIcon.Error);
//             }
        }

        public int ReleasableBroadcastItemCount
        {
            get
            {
                int count = 0;
//                foreach (var item in _broadcastItems)
                foreach (var item in _broadcastProxy.Values)
                {
                    BroadcastStatus status = item.Status;
                    if (status == BroadcastStatus.OK)
                    {
                        count++;
                    }
                    else if (status != BroadcastStatus.Skip)
                    {
                        break;
                    }
                }
                return count;
            }
        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            if (!force)
            {

            }

            if (_systemSettingsProxy != null)
            {
                _systemSettingsProxy.DataItemChanged -= _SystemSettingsChangedHandler;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
            if (_broadcastProxy != null)
            {
                _broadcastProxy.DataItemChanged -= _BroadcastItemChangedHandler;
                _broadcastProxy.CollectionRefreshed -= _BroadcastRefreshedHandler;
                XProxyCache.Release(_broadcastProxy);
                _broadcastProxy = null;
            }
            if (_slugAProxy != null)
            {
                _slugAProxy.DataItemChanged -= _SlugAProxy_DataItemChanged;
                XProxyCache.Release(_slugAProxy);
                _slugAProxy = null;
            }
            if (_storageProxy != null)
            {
                _storageProxy.DataItemChanged -= _StorageChangedHandler;
                XProxyCache.Release(_storageProxy);
                _storageProxy = null;
            }
            return true;
        }

        private void _NavigatorBtnExport_Click(object sender, EventArgs e)
        {
//             _ExportToCsv();
        }

//         private void _ExportToCsv()
//         {
// //            _broadcastItems = _broadcastProxy.GetCurrentBroadcastItems(
// //                _systemSettingsProxy.LastSequenceNumberReleased,
// //                _systemSettingsProxy.LargestBroadcastNumberReceived);
// 
//             StringBuilder csv = new StringBuilder();
//             csv.Append("Record Number,Available,Sequence Number,Status,Trim Code,VIN,Timestamp\r\n");
//             Int32 counter = 0;
// //            foreach (var broadcastItem in _broadcastItems)
//             foreach (var broadcastItem in _broadcastProxy.Values)
//             {
//                 csv.AppendFormat(
//                     "{0},{1},{2},{3},{4},{5},{6}\r\n",
//                     ++counter,
// //                     broadcastItem.Available ? "1" : "0",
//                     broadcastItem.Shortage ? "0" : "1",
//                     broadcastItem.SequenceNumber,
// //                    XEnum.GetText(broadcastItem.Status),
//                     broadcastItem.Status.ToText(),
//                     broadcastItem.TrimCode,
//                     broadcastItem.Vin,
//                     broadcastItem.ReceivedOn.ToString());
//             }
//             if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
//             {
//                 try
//                 {
//                     using (TextWriter writer = File.CreateText(saveFileDialog1.FileName))
//                     {
//                         writer.Write(csv.ToString());
//                     }
//                 }
//                 catch (Exception)
//                 {
//                     MessageBox.Show(
//                         this,
//                         "Failed to create CSV file.",
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                 }
//             }
//         }

        private void _NavigatorBtnClose_Click(object sender, EventArgs e)
        {
            CloseView();
        }

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
