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
using DacQuest.DFX.Core.MessageBox;

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
//         private StorageProxy _storageProxy;


        public BroadcastView()
        {
            InitializeComponent();

            _broadcastGrid.DataSource = _bindingSource;
            _broadcastGrid.AutoGenerateColumns = false;
            _broadcastGrid.AutoSize = false;
            _broadcastGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        protected override void OpenView()
        {
            XProxyCache.Acquire(
                Constant.BroadcastName,
                Constant.CurrentBroadcastQuery,
                out _broadcastProxy);
            _broadcastProxy.DataItemChanged += _Broadcast_DataItemChanged;
            _broadcastProxy.CollectionRefreshed += _Broadcast_CollectionRefreshed;

            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
            _systemSettingsProxy.DataItemChanged += _SystemSettings_DataItemChanged;

            XProxyCache.Acquire(Constant.SlugAName, out _slugAProxy);
            _slugAProxy.DataItemChanged += _SlugAProxy_DataItemChanged;
            _slugAProxy.CollectionRefreshed += _SlugAProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.SlugBName, out _slugBProxy);
            _slugBProxy.DataItemChanged += _SlugBProxy_DataItemChanged;
            _slugBProxy.CollectionRefreshed += _SlugBProxy_CollectionRefreshed;

            //             XProxyCache.Acquire(Constant.StorageName, out _storageProxy);
            //             _storageProxy.DataItemChanged += _Storage_DataItemChanged;

            _broadcastGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _broadcastGrid.ColumnHeadersHeight = 30; // Set to desired height in pixels
            _broadcastGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            _broadcastGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);

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
                MinimumWidth = 100,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "CSN",
                DataPropertyName = "Csn",
                Name = "CsnColumn",
                MinimumWidth = 100,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "SKU",
                DataPropertyName = "Sku",
                Name = "SkuColumn",
                MinimumWidth = 100,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "VIN",
                DataPropertyName = "Vin",
                Name = "VinColumn",
                MinimumWidth = 140,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Pick Mode",
                DataPropertyName = "PickMode",
                Name = "PickModeColumn",
                MinimumWidth = 100,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            _ = _broadcastGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Pick Mode Key",
                DataPropertyName = "PickModeKey",
                Name = "PickModeKeyColumn",
                MinimumWidth = 120,
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
            _navigatorBtnRelease.Visible = _parameters.AllowRelease;
            _navigatorBtnRecover.Visible = _parameters.AllowRecover;

            _UpdateGrid();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = parameters as BroadcastViewParameterSetWrapper;
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
//             _storageProxy.Refresh();
            _slugAProxy.Refresh();
            _slugBProxy.Refresh();
            _systemSettingsProxy.Refresh();
            _broadcastProxy.Refresh();
        }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

//         private void _Storage_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
//         {
// //            _broadcastProxy.Refresh();
// //            _UpdateGrid();
//         }

        private void _Broadcast_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _Broadcast_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateGrid();
        }

        private void _SlugAProxy_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

        private void _SlugAProxy_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

        private void _SlugBProxy_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

        private void _SlugBProxy_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            _UpdateReleaseButton();
        }

        private void _UpdateGrid()
        {
//            _broadcastItems = _broadcastProxy.GetCurrentBroadcastItems(
//                _systemSettingsProxy.LastSequenceNumberReleased,
//                _systemSettingsProxy.LargestBroadcastNumberReceived);
            List<BroadcastItem> broadcastItems = _broadcastProxy.Values;

//             int topAvailableCount = broadcastItems.TakeWhile(b => !b.Shortage).Count();

//             topAvailableCount -= topAvailableCount % 6;
//            if (topAvailableCount % 2 == 1)
//            {
//                topAvailableCount--;
//            }

//             if (topAvailableCount > 0)
//             {
// //                lblTitle.Text = String.Format(
// //                    "Broadcast   ( {0} of {1} broadcasts can be shipped )",
// //                    topAvailableCount,
// //                    _broadcastItems.Count);
//                 _lblTitle.Text = String.Format(
//                     "Broadcast   ( {0} of {1} broadcasts can be shipped )",
//                     topAvailableCount,
//                     broadcastItems.Count);
//             }
//             else
//             {
//                 _lblTitle.Text = "Broadcast   ( No broadcasts can be shipped )";
//             }

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
            SlugProxy targetSlugProxy = null;
            if  (!_parameters.AllowRelease
                || !_GetTargetSlugForBroadcastRelease(out targetSlugProxy))
            {
                _navigatorBtnRelease.Enabled = false;
                return;
            }
            bool enable = false;
            int waitingCount = targetSlugProxy.Cleared
                ? Constant.LoadSize
                : targetSlugProxy.WaitingCount;
            int releasableBroadcastItemCount = _broadcastProxy.ReleasableBroadcastItemCount;
            int[] releasableCounts = Utils.GetReleasableCounts(waitingCount);
            foreach (int releasableCount in releasableCounts)
            {
                if (releasableBroadcastItemCount >=  releasableCount)
                {
                    enable = true;
                    break;
                }
            }
            _navigatorBtnRelease.Enabled = enable;

        }

        private void _UpdateRecoverButton()
        {
            _navigatorBtnRecover.Enabled = _parameters.AllowRecover
                && _broadcastProxy
                    .Values
                    .Count(bi => bi.Status == BroadcastStatus.Shipped) > 0;
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
            if (!_GetTargetSlugForBroadcastRelease(out SlugProxy targetSlugProxy))
            {
                _navigatorBtnRelease.Enabled = false;
                return;
            }
            int releasableBroadcastCount = ReleasableBroadcastItemCount;

            if (releasableBroadcastCount <= 0)
            {
                _navigatorBtnRelease.Enabled = false;
                return;
            }

            int waitingCount = targetSlugProxy.WaitingCount;
            int[] releasableCounts = Utils.GetReleasableCounts(waitingCount);
            List<int> filteredReleasableCounts = new List<int>();
            for (int index =  0; index < releasableCounts.Length; index++)
            {
                if (releasableCounts[index] <= releasableBroadcastCount)
                {
                    filteredReleasableCounts.Add(releasableCounts[index]);
                }
            }
            if (filteredReleasableCounts.Count == 0)
            {
                _navigatorBtnRelease.Enabled = false;
                return;
            }

            using (SelectBroadcastReleaseCountForm form = new SelectBroadcastReleaseCountForm(
                targetSlugProxy.SlugLetter,
                filteredReleasableCounts,
                releasableBroadcastCount))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    ParentForm.Cursor = Cursors.WaitCursor;
                    ReleaseBroadcastMessageData responseMessageData;
                    try
                    {
                        int countToRelease = form.CountToRelease;
                        ReleaseBroadcastMessageData messageData
                            = new ReleaseBroadcastMessageData(
                                targetSlugProxy.SlugLetter,
                                countToRelease,
                                XSystemEvent.Create(
                                    nameof(BroadcastView),
                                    XSystemEventLevel.Manual,
                                    $"{countToRelease} Broadcast Items released to Slug {targetSlugProxy.SlugLetter}."));
                        if (!XMessaging.SyncPublish(
                            out responseMessageData,
                            ReleaseBroadcastMessageData.BroadcastReleaseTimeoutMilliseconds,
                            ReleaseBroadcastMessageData.ReleaseBroadcastMessageTopicName,
                            messageData,
                            XMessageScopes.All,
                            this))
                        {
                            string message = $"Request to release {countToRelease} Broadcast Items to Slug {targetSlugProxy.SlugLetter} has timed out. The operation may still have completed correctly.";
                            XSystemEvent.Publish(
                                nameof(BroadcastView),
                                XSystemEventLevel.Error,
                                message);
                            _ = XMessageBox.Show(
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
                    if (!responseMessageData.Error.IsNullOrWhiteSpace())
                    {
                        _ = XMessageBox.Show(
                            this,
                            responseMessageData.Error,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
//                     else
//                     {
//                         if (_parameters.CloseAfterRelease)
//                         {
//                             CloseView();
//                         }
//                     }
                }
            }
        }


        private void _DoRecover()
        {
            ParentForm.Cursor = Cursors.WaitCursor;
            RecoverBroadcastMessageData responseMessageData;
            try
            {
                RecoverBroadcastMessageData messageData
                        = new RecoverBroadcastMessageData(
                            XSystemEvent.Create(
                                nameof(BroadcastView),
                                XSystemEventLevel.Manual,
                                "Recovered eligible Broadcast Items."));
                if (!XMessaging.SyncPublish(
                    out responseMessageData,
                    RecoverBroadcastMessageData.RecoverBroadcastTimeoutMilliseconds,
                    RecoverBroadcastMessageData.RecoverBroadcastMessageTopicName,
                    messageData,
                    XMessageScopes.All,
                    this))
                {
                    string message = "Request to recover Broadcast Items has timed out. The operation may still have completed correctly.";
                    XSystemEvent.Publish(
                        nameof(BroadcastView),
                        XSystemEventLevel.Error,
                        message);
                    _ = XMessageBox.Show(
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
            if (responseMessageData.ErrorMessage != null)
            {
                _ = XMessageBox.Show(
                    this,
                    responseMessageData.ErrorMessage,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public int ReleasableBroadcastItemCount
        {
            get
            {
                int count = 0;
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

        private bool _GetTargetSlugForBroadcastRelease(out SlugProxy targetSlugProxy)
        {
            targetSlugProxy = null;
            bool slugAEnabled = _systemSettingsProxy.SlugAEnabled;
            bool slugBEnabled = _systemSettingsProxy.SlugBEnabled;

            if (!slugAEnabled && !slugBEnabled)
            {
                return false;
            }

            bool slugAEmpty = _slugAProxy.Cleared && slugAEnabled;
            bool slugBEmtpy = _slugBProxy.Cleared && slugBEnabled;

            if (_slugAProxy.HasOpenLoad)
            {
                if (!_systemSettingsProxy.SlugAEnabled)
                {
                    return false;
                }
                targetSlugProxy = _slugAProxy;
            }
            else if (_slugBProxy.HasOpenLoad)
            {
                if (!_systemSettingsProxy.SlugBEnabled)
                {
                    return false;
                }
                targetSlugProxy = _slugBProxy;
            }
            else if (slugAEmpty && slugBEmtpy)
            {
                targetSlugProxy = _systemSettingsProxy.PreferredSlug == SlugLetter.A
                    ? _slugAProxy
                    : _slugBProxy;
            }
            else if (slugAEmpty)
            {
                targetSlugProxy = _slugAProxy;
            }
            else if (slugBEmtpy)
            {
                targetSlugProxy = _slugBProxy;
            }
            return targetSlugProxy != null;
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
                _systemSettingsProxy.DataItemChanged -= _SystemSettings_DataItemChanged;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
            if (_broadcastProxy != null)
            {
                _broadcastProxy.DataItemChanged -= _Broadcast_DataItemChanged;
                _broadcastProxy.CollectionRefreshed -= _Broadcast_CollectionRefreshed;
                XProxyCache.Release(_broadcastProxy);
                _broadcastProxy = null;
            }
            if (_slugAProxy != null)
            {
                _slugAProxy.DataItemChanged -= _SlugAProxy_DataItemChanged;
                XProxyCache.Release(_slugAProxy);
                _slugAProxy = null;
            }
            if (_slugBProxy != null)
            {
                _slugBProxy.DataItemChanged -= _SlugBProxy_DataItemChanged;
                XProxyCache.Release(_slugBProxy);
                _slugBProxy = null;
            }
//             if (_storageProxy != null)
//             {
//                 _storageProxy.DataItemChanged -= _Storage_DataItemChanged;
//                 XProxyCache.Release(_storageProxy);
//                 _storageProxy = null;
//             }
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
