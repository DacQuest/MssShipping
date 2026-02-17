using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;
using DacQuest.DFX.DataItemEditors;
using DacQuest.DFX.Core.DataItems;
using System.Text;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core.Threading;
using Mss.Common;
using Mss.Data;
using Mss.Views;

namespace Mss.Views
{
    public partial class StorageView : XSnapInView, ISearchResultsGridParent
    {
        private const int _searchGridSizeOffset = 103;

        private StorageViewParameterSetWrapper _parameters;
        private StorageProxy _storageProxy;
        private UpperPitProxy _upperPitProxy;
        private LowerPitProxy _lowerPitProxy;
        private BinItem _workingBinItem = new BinItem();
        private BinItem _originalBinItem = new BinItem();
        private bool _allowBinEditing = false;
        private bool _allowBulkEditing = false;
        private bool _allowPalletStatusEditing = false;
        private bool _initializingControl = false;
        private List<BinItem> _searchResults = new List<BinItem>();
        private bool _forceContinue = false;

        private PalletStatus _newStatus = PalletStatus.Invalid;
        private int _newHoldCode = 0;
        private string _newComment = string.Empty;
        private bool _unmarkAudits = false;
        private bool _markAudits = false;

        private Search _lastSearch = new Search();

        private AdvancedSearchForm _advancedSearchForm = null;

        public StorageView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {

            XProxyCache.Acquire(Constant.UpperPitName, out _upperPitProxy);
            XProxyCache.Acquire(Constant.LowerPitName, out _lowerPitProxy);

            XProxyCache.Acquire(Constant.StorageName, out _storageProxy);
            _storageProxy.DataItemChanged += _StorageProxy_DataItemChanged;
            _storageProxy.CollectionRefreshed += _StorageProxy_CollectionRefreshed;

            _allowBinEditing = _parameters.AllowBinEditing;
            _allowBulkEditing = _parameters.AllowBulkEditing;
            _allowPalletStatusEditing = _parameters.AllowPalletStatusEditing;
            //             _allowSkuEditing =  _parameters.AllowSkuEditing;

            _initializingControl = true;

            _spnHorizontal.Maximum = _parameters.MaxBank1Horizontal;

            _cmbBinStatus.AddEnumItem(BinStatus.Invalid);
            _cmbBinStatus.AddEnumItem(BinStatus.Empty);
            _cmbBinStatus.AddEnumItem(BinStatus.Pickable);
            _cmbBinStatus.AddEnumItem(BinStatus.GetAllocated);
            _cmbBinStatus.AddEnumItem(BinStatus.PutAllocated);
            _cmbBinStatus.AddEnumItem(BinStatus.Offline);
            _cmbBinStatus.AddEnumItem(BinStatus.OfflineDuplicatePalletID);

            _cmbPalletStatus.AddEnumItem(PalletStatus.OK);
            _cmbPalletStatus.AddEnumItem(PalletStatus.Hold);
            _cmbPalletStatus.AddEnumItem(PalletStatus.Reserve);
            _cmbPalletStatus.AddEnumItem(PalletStatus.Purge);
            _cmbPalletStatus.AddEnumItem(PalletStatus.Stack);
            _cmbPalletStatus.AddEnumItem(PalletStatus.Unknown);

            _initializingControl = false;

            _navigatorBtnFillBin.Visible = _allowBinEditing;
            _navigatorBtnFillBin.Visible = _allowBinEditing;
            _navigatorBtnClearBin.Visible = _allowBinEditing;
            _cmbBinStatus.Visible = _allowBinEditing;
            _lblBinStatus.Visible = !_allowBinEditing;
            _chkPickOnly.Enabled = _allowBinEditing;
            _chkDisabled.Enabled = _allowBinEditing;

            _cmbPalletStatus.Visible = _allowPalletStatusEditing;
            _lblPalletStatus.Visible = !_allowPalletStatusEditing;
            _lblComment.Visible = !_allowPalletStatusEditing;
            _txtComment.Visible = _allowPalletStatusEditing;
//             lblSku.Visible = !_allowSkuEditing;

            _navigatorBtnSaveItem.Visible = _allowBinEditing
                || _allowPalletStatusEditing;
//                 || _allowSkuEditing;

            _searchResultsGrid.Initialize(this, _allowBulkEditing);

            //             cmbNewStatus.Items.Add("Audit");
            _ = _cmbNewStatus.AddEnumItem(PalletStatus.Invalid, "Audit");
            _ = _cmbNewStatus.AddEnumItem(PalletStatus.OK);
            _ = _cmbNewStatus.AddEnumItem(PalletStatus.Hold);
            _ = _cmbNewStatus.AddEnumItem(PalletStatus.Reserve);
            _ = _cmbNewStatus.AddEnumItem(PalletStatus.Purge);
            _ = _cmbNewStatus.AddEnumItem(PalletStatus.Stack);

            _lblNewStatus.Visible = _allowBulkEditing;
            //lblNewHoldCode.Visible = _allowBulkEditing;
            _chkBulkMarkAudit.Visible = _allowBulkEditing;
            _chkUnmarkAudit.Visible = _allowBinEditing;
            //cmbNewHoldCode.Visible = _allowBulkEditing;
            _cmbNewStatus.Visible = _allowBulkEditing;
            //lblNewComment.Visible = _allowBulkEditing;
            //txtNewComment.Visible = _allowBulkEditing;
            _btnConvert.Visible = _allowBulkEditing;
            _btnConvertAll.Visible = _allowBulkEditing;
            //lblRemainingCharacters.Visible = _allowBulkEditing;
            //chkOverwriteComments.Visible = _allowBulkEditing;

            _markAudits = false;
            _unmarkAudits = false;
            _chkBulkMarkAudit.Checked = false;
            _chkUnmarkAudit.Checked = false;
            _chkBulkMarkAudit.Visible = false;
            _chkUnmarkAudit.Visible = false;
//             grpBulkAudits.Enabled = false;
//             grpBulkAudits.Visible = false;
//             grpNewStatus.Enabled = true;
            _pnlConversion.Visible = _allowBulkEditing;

            if (!_allowBulkEditing)
            {
                Size size = _searchResultsGrid.Size;
                size.Height += _searchGridSizeOffset;
                _searchResultsGrid.Size = size;
            }

            if (GetViewMessage(Constant.StorageViewInitialNodeIndexMessageName, out int goToNodeIndex))
            {
                RemoveViewMessage(Constant.StorageViewInitialNodeIndexMessageName);
                _originalBinItem.Copy(_storageProxy.GetAt(goToNodeIndex));
                _workingBinItem.Copy(_originalBinItem);
                _UpdateSpinners(goToNodeIndex);
            }
            else
            {
                _originalBinItem.Copy(_storageProxy.GetAt(0));
                _workingBinItem.Copy(_originalBinItem);
                _UpdateSpinners(0);
            }
            _PopulateControls(_originalBinItem);
            _spnCrane.Focus();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = parameters as StorageViewParameterSetWrapper;
            if (!_parameters.AllowAuditPicks)
            {
                _chkAudit.Enabled = false;
            }
        }

        //         protected override void AutoSubscribe()
        //         {
        //         }

        public override bool ViewSwitching(bool viewClosing, bool forcedClose)
        {
            if (!forcedClose)
            {
                return _IsOkToContinue();
            }
            return true;
        }

        public override bool ViewClosing(bool force)
        {
//             if (!force)
//             {
//                 if (!_IsOkToContinue())
//                 {
//                     return false;
//                 }
//             }
            if (_storageProxy != null)
            {
                _storageProxy.DataItemChanged -= _StorageProxy_DataItemChanged;
                _storageProxy.CollectionRefreshed -= _StorageProxy_CollectionRefreshed;
                XProxyCache.Release(_storageProxy);
                _storageProxy = null;
            }
            if (_upperPitProxy != null)
            {
                XProxyCache.Release(_upperPitProxy);
                _upperPitProxy = null;
            }
            if (_lowerPitProxy != null)
            {
                XProxyCache.Release(_lowerPitProxy);
                _lowerPitProxy = null;
            }
            if (_advancedSearchForm != null)
            {
                _advancedSearchForm.Dispose();
                _advancedSearchForm = null;
            }
            return true;
        }

        private bool _AreStatusAndHoldCodeAndCommentConsistent()
        {
            bool needHoldCode = false;
            bool needComment = false;
            PalletItem palletItem = _workingBinItem.Pallet;
            PalletStatus status = palletItem.Status;
            switch (status)
            {
                case PalletStatus.Hold:
                    needComment = string.IsNullOrWhiteSpace(palletItem.Comment);
                    break;
                case PalletStatus.Purge:
                    needComment = string.IsNullOrWhiteSpace(palletItem.Comment);
                    break;
            }
            string message = null;
            if (needComment && needHoldCode)
            {
                message = "You must provide a Hold Code and a Comment!";
            }
            else if (needComment)
            {
                message = "You must provide a Comment!";
            }
            else if (needHoldCode)
            {
                message = "You must provide a Hold Code!";
            }
            if (needComment || needHoldCode)
            {
                XMessageBox.Show(
                    this,
                    message,
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool _IsOkToContinue()
        {
            if (!_forceContinue && _originalBinItem.Different(_workingBinItem))
            {
                switch (XMessageBox.Show(
                    this,
                    "The Bin Item has changed. Do you want to save the changes?",
                    "Save Changes?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button3))
                {
                    case DialogResult.Yes:
                        return _SaveItem();
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                }
            }
            return true;
        }

        private bool _SaveItem()
        {
            if (!_AreStatusAndHoldCodeAndCommentConsistent())
            {
                return false;
            }
            if (!_storageProxy.SafeSetAt(
                _originalBinItem.NodeIndex,
                _originalBinItem,
                ref _workingBinItem))
            {
                NotifyUser("Original Item was stale. Changes were NOT saved. The fresh Bin data is now displayed.");
            }
            else
            {
                NotifyUser("Item saved by user.");
            }
            _originalBinItem.Copy(_workingBinItem);
            _PopulateControls(_originalBinItem);
            return true;
        }

        protected void NotifyUser(String message)
        {
            _lstMessages.Items.Insert(
                0,
                string.Format(
                    "{0} : {1}",
                    DateTime.Now.ToLongTimeString(),
                    message));
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

        public void GoToBinIndex(Int32 binIndex)
        {
            if (_originalBinItem.NodeIndex == binIndex)
            {
                return;
            }
            if (_workingBinItem.Different(_originalBinItem))
            {
                if (!_IsOkToContinue())
                {
                    return;
                }
            }
            _forceContinue = true;
            _UpdateSpinners(binIndex);
            _forceContinue = false;

            //            _GoToBinIndex(binIndex);
        }

        private void _GoToBinIndex(int binIndex)
        {
            _lstMessages.Items.Clear();
            _originalBinItem.Copy(_storageProxy.GetAt(binIndex));
            //            _workingBinItem.Copy(_originalBinItem);
            _PopulateControls(_originalBinItem);
        }

        private void _StorageProxy_DataItemChanged(Object sender, XDataItemChangedEventArgs e)
        {
            BinItem binItem = (BinItem)e.DataItem;
            if (binItem.NodeIndex == _originalBinItem.NodeIndex
                && binItem.Different(_originalBinItem))
            {
                //                 NotifyUser(
                //                     String.Format(
                //                         "Bin at {0}-{1}-{2} was edited externally. The new data is displayed.",
                //                         _originalBinItem.BinAisleNumber,
                //                         _originalBinItem.BinHorizontalNumber,
                //                         _originalBinItem.BinVerticalNumber));
                _originalBinItem.Copy(binItem);
                _PopulateControls(_originalBinItem);
            }
        }

        private void _StorageProxy_CollectionRefreshed(Object sender, EventArgs e)
        {
            BinItem binItem = _storageProxy.GetAt(_workingBinItem.NodeIndex);
            if (binItem.Different(_workingBinItem))
            {
                NotifyUser("The current Item was edited externally. The new data is displayed.");
                _originalBinItem.Copy(binItem);
                //                _workingBinItem.Copy(_originalBinItem);
                _PopulateControls(_originalBinItem);
            }
        }

        private void _ShowSearchResults(List<BinItem> searchResults)
        {
            _ShowSearchResults(searchResults, false, true);
            //             if (searchResults.Count > 0)
            //             {
            //                 _SetSearchResults(searchResults);
            //             }
            //             else
            //             {
            //                 XMessageBox.Show(
            //                     this,
            //                     "No matching results were found.",
            //                     "Warning",
            //                     MessageBoxButtons.OK,
            //                     MessageBoxIcon.Warning);
            //             }
        }

        private void _ShowSearchResults(List<BinItem> searchResults, bool isAdvancedSearch, bool showEmptySearch)
        {
            if (searchResults.Count > 0)
            {
                if (isAdvancedSearch)
                {
                    _SetSearchResultsForAdvancedSearch(searchResults);
                }
                else
                {
                    _SetSearchResults(searchResults);
                }
            }
            else if (showEmptySearch)
            {
                XMessageBox.Show(
                    this,
                    "No matching results were found.",
                    "Search Results",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void _SetSearchResults(List<BinItem> searchResults)
        {
            _lblSearchResults.Text = string.Format("Search Results ( {0} )", searchResults.Count);
            _searchResults = searchResults.OrderBy(item => item.BinNumber).ToList();
            _searchResultsGrid.SetSearchResults(_searchResults);
            if (_searchResults.Count > 0)
            {
                GoToBinIndex(_searchResults[0].NodeIndex);
            }
            _EnableConvertButtons();
        }

        private void _SetSearchResultsForAdvancedSearch(List<BinItem> searchResults)
        {
            _lblSearchResults.Text = string.Format("Search Results ( {0} )", searchResults.Count);
            _searchResults = searchResults.ToList();
            _searchResultsGrid.SetSearchResults(_searchResults);
            if (_searchResults.Count > 0)
            {
                GoToBinIndex(_searchResults[0].NodeIndex);
            }
        }

        private void _BtnPrintSearchResults_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not Implemented Yet!");
            return;
//             if (_searchResults.Count > 0)
//             {
//                 SearchResultsReport report = new SearchResultsReport(_searchResults);
//                 ReportPrintTool printTool = new ReportPrintTool(report);
//                 //                 printTool.PreviewForm.Load += ReportPreviewForm_Load;
//                 printTool.ShowPreview();
//                 //                 printTool.ShowPreviewDialog();
// 
//                 //                 report.ShowPreview();
//             }
        }

        private void _BtnClearSearchResults_Click(object sender, EventArgs e)
        {
            _SetSearchResults(new List<BinItem>());
            _cmbNewStatus.SelectedIndex = -1;
        }

        private void _BtnClearMessages_Click(object sender, EventArgs e)
        {
            _lstMessages.Items.Clear();
        }

//         bool autoRefresh = false;
//         long timerID = 0;
//         private void RefreshTimerHandler(XTimerEventArgs e)
//         {
//             _holdCodesProxy.Refresh();
//             _pitProxy.Refresh();
//             _skusProxy.Refresh();
//             _storageProxy.Refresh();
//
//             timerID = 0;
//             timerID = StartTimer(RefreshTimerHandler, 2500, null);
//         }
        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _upperPitProxy.Refresh();
            _lowerPitProxy.Refresh();
            _storageProxy.Refresh();


//             autoRefresh = !autoRefresh;
//             if (autoRefresh)
//             {
//                 timerID = StartTimer(RefreshTimerHandler, 2500, null);
//             }
//             else if (timerID > 0)
//             {
//                 StopTimer(timerID);
//                 timerID = 0;
//             }
        }

        private void _NavigatorBtnSaveItem_Click(object sender, EventArgs e)
        {
            _SaveItem();
        }

        private void _ByBinStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectBinStatusesForm form = new SelectBinStatusesForm();
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            List<BinItem> searchResults = Searches.ByBinStatus(_storageProxy.Items, form.BinStatuses);
            _lastSearch.Type = SearchType.ByBinStatus;
            _lastSearch.BinItems = _storageProxy.Items;
            _lastSearch.BinStatuses = form.BinStatuses;
            _ShowSearchResults(searchResults);
            form.Dispose();
        }

        //private void byVehicleRowToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    SelectVehicleRowForm form = new SelectVehicleRowForm();
        //    if (form.ShowDialog(this) != DialogResult.OK)
        //    {
        //        return;
        //    }
        //    List<BinItem> searchResults = Searches.ByVehicleRow(_storageProxy.Items, form.VehicleRows);
        //    _lastSearch.Type = SearchType.ByVehicleRow;
        //    _lastSearch.BinItems = _storageProxy.Items;
        //    _lastSearch.VehicleRows = form.VehicleRows;
        //    _ShowSearchResults(searchResults);
        //    form.Dispose();
        //}

        private void _ByPalletIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> palletIDs = _storageProxy.GetAllPalletIDsInStorage();
            using (SelectPalletIDForm form = new SelectPalletIDForm(palletIDs))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                if (form.PalletID.IsNullOrWhiteSpace())
                {
                    return;
                }
                List<BinItem> searchResults = Searches.ByPalletID(_storageProxy.Items, form.PalletID);
                _lastSearch.Type = SearchType.ByPalletID;
                _lastSearch.BinItems = _storageProxy.Items;
                _lastSearch.PalletID = form.PalletID;
                _ShowSearchResults(searchResults);
            }
        }

        private void _AdvancedSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_advancedSearchForm == null)
            {
                _advancedSearchForm = new AdvancedSearchForm();
            }
            _advancedSearchForm.ShowDialog(this);
            if (_advancedSearchForm.DialogResult != DialogResult.OK)
            {
                return;
            }
            _lastSearch.Type = SearchType.AdvancedSearch;
            _lastSearch = _advancedSearchForm.SearchCriteria;
            _ShowSearchResults(_advancedSearchForm.SearchResults, true, true);
        }

        private void _ByPalletStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectPalletStatusesForm form = new SelectPalletStatusesForm();
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            List<BinItem> searchResults = Searches.ByPalletStatus(_storageProxy.Items, form.PalletStatuses);
            _lastSearch.Type = SearchType.ByPalletStatus;
            _lastSearch.BinItems = _storageProxy.Items;
            _lastSearch.PalletStatuses = form.PalletStatuses;
            _ShowSearchResults(searchResults);
            form.Dispose();
        }

        //private void _ByKitCodeToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    List<string> kitCodes = _storageProxy.GetAllKitCodesInStorage();
        //    using (SelectKitCodeForm form = new SelectKitCodeForm(kitCodes))
        //    {
        //        if (form.ShowDialog(this) != DialogResult.OK)
        //        {
        //            return;
        //        }
        //        List<BinItem> searchResults = Searches.ByKitCode(_storageProxy.Items, form.KitCode);
        //        _lastSearch.Type = SearchType.ByKitCode;
        //        _lastSearch.BinItems = _storageProxy.Items;
        //        _lastSearch.SeatID = form.KitCode;
        //        _ShowSearchResults(searchResults);
        //    }
        //}

        private void _BySkuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> skus = _storageProxy.GetAllSkusInStorage();
            SelectSkuForm form = new SelectSkuForm(skus);
            if (form.ShowDialog(this) != DialogResult.OK
                || string.IsNullOrWhiteSpace(form.Sku))
            {
                return;
            }
            List<BinItem> searchResults = Searches.BySku(_storageProxy.Items, form.Sku);
            _lastSearch.Type = SearchType.BySku;
            _lastSearch.BinItems = _storageProxy.Items;
            _lastSearch.Sku = form.Sku;
            _ShowSearchResults(searchResults);
            form.Dispose();
        }

        private void _DuplicatePalletsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<BinItem> searchResults = Searches.DuplicatePallets(_storageProxy.Items);
            _lastSearch.Type = SearchType.DuplicatePallets;
            _lastSearch.BinItems = _storageProxy.Items;
            _ShowSearchResults(searchResults);
        }

        private void _PopulateControls(BinItem originalBinItem)
        {
            _workingBinItem.Copy(originalBinItem);

            // Bin Data
            _lblBinNumber.Text = _workingBinItem.BinNumber.ToString();
            _lblBinStatus.Text = _workingBinItem.BinStatus.ToText();
            _chkAudit.Checked = _workingBinItem.Audit;
            //             _lblAuditAttempts.Text = _workingBinItem.AuditAttempts.ToString();
            _cmbBinStatus.SelectEnumItem(_workingBinItem.BinStatus);
            if (_workingBinItem.StoredOn > Constant.BeginningOfTime)
            {
                _lblStoredOn.Text = _workingBinItem.StoredOn.ToString("G");
                TimeSpan age = DateTime.Now - _workingBinItem.StoredOn;
                _lblAge.Text = age.TotalHours < 1
                    ? "<1 Hour"
                    : age.TotalHours <= 24.0
                        ? string.Format("{0:0.0} Hours", age.TotalHours)
                        : string.Format("{0:0.0} Days", age.TotalDays);
            }
            else
            {
                _lblStoredOn.Text = string.Empty;
                _lblAge.Text = string.Empty;
            }
            //             if (_workingBinItem.OfflineReason == BinOfflineReason.None)
            //             {
            //                 lblOfflineReason.Text = string.Empty;
            //             }
            //             else
            //             {
            //                 lblOfflineReason.Text = _workingBinItem.OfflineReason.ToText();
            //             }

            _chkPickOnly.Checked = _workingBinItem.PickOnly;
            _chkDisabled.Checked = _workingBinItem.Disabled;
            // Pallet Data
            PalletItem palletItem = _workingBinItem.Pallet;
            if (palletItem.PalletID != Constant.NoPalletID)
            {

//                 if (palletItem.PalletID == Constant.NoReadPalletID)
//                 {
//                     lblPalletID.Text = Constant.NoReadPalletIDString;
//                 }
//                 else
//                 {
                _lblPalletID.Text = palletItem.PalletID;
                _lblVehicleRow.Text = palletItem.VehicleRow.ToText();
//                 }
                _txtComment.ReadOnly = !_allowPalletStatusEditing;
                if (palletItem.Status == PalletStatus.Invalid)
                {
                    _cmbPalletStatus.SelectedIndex = -1;
                    _lblPalletStatus.Text = string.Empty;
                }
                else
                {
                    _cmbPalletStatus.SelectEnumItem(palletItem.Status);
                    _lblPalletStatus.Text = palletItem.Status.ToText();
                }
                _lblSku.Text = palletItem.Sku;
//                 string[] seatIDs = palletItem.SeatIDs;
//                 _lblSeatID1.Text = seatIDs[0];
//                 _lblSeatID2.Text = seatIDs[1];
//                 _lblSeatID3.Text = seatIDs[2];
//                 _lblSeatID4.Text = seatIDs[3];
//                 _lblSeatID5.Text = seatIDs[4];
//                 _lblSeatID6.Text = seatIDs[5];
                _lblBuiltOn.Text = palletItem.BuiltOn.ToText();
                _txtComment.Text = palletItem.Comment;
                _lblComment.Text = palletItem.Comment;
                _lblCommentLength.Text = $"{Constant.CommentLength - palletItem.Comment.Length}";
            }
            else
            {
                _lblPalletID.Text = string.Empty;
                _lblPalletID.BackColor = SystemColors.Window;
                _lblVehicleRow.Text = string.Empty;
                _cmbPalletStatus.SelectedIndex = -1;
                _lblPalletStatus.Text = string.Empty;
                _lblBuiltOn.Text = string.Empty;
                _lblSku.Text = string.Empty;
                _txtComment.Text = string.Empty;
                _txtComment.Enabled = false;
                _lblCommentLength.Text = string.Empty;
            }

            bool allowEditing = !_workingBinItem.NotUsable && _allowBinEditing;
            _cmbBinStatus.Enabled = allowEditing;
            _chkPickOnly.Enabled = allowEditing;
            _chkDisabled.Enabled = allowEditing;
            _lblBinNotUsable.Visible = _workingBinItem.NotUsable;

            _chkAudit.Enabled = !_workingBinItem.NotUsable && _parameters.AllowAuditPicks;

            allowEditing = !_workingBinItem.NotUsable && _allowPalletStatusEditing;
            _cmbPalletStatus.Enabled = allowEditing && palletItem.PalletID != Constant.NoPalletID;
            PalletStatus palletStatus = palletItem.Status;

            _txtComment.Enabled = allowEditing
                && palletItem.PalletID != Constant.NoPalletID
                && (palletStatus == PalletStatus.Hold
                    || palletStatus == PalletStatus.Purge
                    || palletStatus == PalletStatus.Unknown);

            _navigatorBtnSaveItem.Enabled = !_workingBinItem.NotUsable;
            _navigatorBtnClearBin.Enabled = !_workingBinItem.NotUsable;

//             _navigatorBtnFillBin.Enabled = !_workingBinItem.NotUsable
//                 && (_workingBinItem.BinStatus == BinStatus.Empty
//                     || palletItem.PalletID == Constant.NoPalletID);
            _navigatorBtnFillBin.Enabled = !_workingBinItem.NotUsable
                && (_workingBinItem.BinStatus == BinStatus.Empty
                    || palletItem.PalletID.IsNullOrWhiteSpace());
        }

        private void _ByDateTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateRangeForm form = new DateRangeForm();
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            List<BinItem> searchResults = Searches.ByBuiltOn(_storageProxy.Items, form.Start, form.End);
            _lastSearch.Type = SearchType.ByBuiltOn;
            _lastSearch.BinItems = _storageProxy.Items;
            _lastSearch.Start = form.Start;
            _lastSearch.End = form.End;
            _ShowSearchResults(searchResults);
            form.Dispose();
        }

        private void _CmbBinStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = _cmbBinStatus.SelectedIndex;
            if (_initializingControl || selectedIndex == -1)
            {
                return;
            }
            BinStatus selectedBinStatus = _cmbBinStatus.GetSelectedEnumItem<BinStatus>();
            if (selectedBinStatus == _workingBinItem.BinStatus)
            {
                return;
            }
            _workingBinItem.BinStatus = selectedBinStatus;
        }

        private void _CmbPalletStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 selectedIndex = _cmbPalletStatus.SelectedIndex;
            if (_initializingControl || selectedIndex == -1)
            {
                return;
            }
            PalletItem palletItem = _workingBinItem.Pallet;
            palletItem.Status = _cmbPalletStatus.GetSelectedEnumItem<PalletStatus>();
            PalletStatus palletStatus = palletItem.Status;
            _txtComment.Enabled = palletStatus == PalletStatus.Hold
                || palletStatus == PalletStatus.Unknown
                || palletStatus == PalletStatus.Reserve
                || palletStatus == PalletStatus.Purge
                || palletStatus == PalletStatus.Stack;
            _workingBinItem.Pallet = palletItem;
        }

        private void _ChkPickOnly_Click(object sender, EventArgs e)
        {
            _workingBinItem.PickOnly = _chkPickOnly.Checked;
        }

        private void _ChkDisabled_Click(object sender, EventArgs e)
        {
            _workingBinItem.Disabled = _chkDisabled.Checked;
        }

        private void _SpnCrane_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                _spnSide.Focus();
            }
        }

        private void _SpnSide_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                _spnCrane.Focus();
            }
            if (e.KeyCode == Keys.Right)
            {
                _spnHorizontal.Focus();
            }
        }

        private void _SpnHorizontal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                _spnSide.Focus();
            }
            if (e.KeyCode == Keys.Right)
            {
                _spnVertical.Focus();
            }
        }

        private void _SpnVertical_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                _spnHorizontal.Focus();
            }
        }

        private void _SpnCrane_ValueChanged(object sender, EventArgs e)
        {
            _lblCrane.Text = ((int)_spnCrane.Value).ToString();
            _CalculateAndGoToBin();
        }

        private void _SpnSide_ValueChanged(object sender, EventArgs e)
        {
            _lblSide.Text = ((int)_spnSide.Value).ToString();
            _CalculateAndGoToBin();
        }

        private void _SpnHorizontal_ValueChanged(object sender, EventArgs e)
        {
            _lblHorizontal.Text = ((int)_spnHorizontal.Value).ToString("00");
            _CalculateAndGoToBin();
        }

        private void _SpnVertical_ValueChanged(object sender, EventArgs e)
        {
            lblVertical.Text = ((int)_spnVertical.Value).ToString();
            _CalculateAndGoToBin();
        }

        private void _CalculateAndGoToBin()
        {
            if (_initializingControl)
            {
                return;
            }

            int crane = (int)_spnCrane.Value;
            int side = (int)_spnSide.Value;
            int horizontal = (int)_spnHorizontal.Value;
            int vertical = (int)_spnVertical.Value;

            int nodeIndex = BinItem.LocationCoordinatesToNodeIndex(
                crane,
                side,
                horizontal,
                vertical);
            if (_originalBinItem.NodeIndex == nodeIndex)
            {
                return;
            }
            if (_workingBinItem.Different(_originalBinItem))
            {
                if (!_IsOkToContinue())
                {
                    _initializingControl = true;
                    _spnCrane.Value = (int)_originalBinItem.CraneNumber;
                    _spnSide.Value = _originalBinItem.BinSideNumber;
                    _spnHorizontal.Value = _originalBinItem.BinHorizontalNumber;
                    _spnVertical.Value = _originalBinItem.BinVerticalNumber;
                    _initializingControl = false;
                    return;
                }
            }
            _GoToBinIndex(nodeIndex);
        }

        private void _UpdateSpinners(int nodeIndex)
        {
            _spnCrane.Value = (int)BinItem.GetCraneNumber(nodeIndex);
            _spnSide.Value = BinItem.GetBinSideNumber(nodeIndex);
            _spnHorizontal.Value = BinItem.GetBinHorizontalNumber(nodeIndex);
            _spnVertical.Value = BinItem.GetBinVerticalNumber(nodeIndex);
        }

        private void _NavigatorBtnClearBin_Click(object sender, EventArgs e)
        {
            StorageEmptyBinForm form = new StorageEmptyBinForm();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                _originalBinItem.BinStatus = BinStatus.Empty;
                _originalBinItem.StoredOn = Constant.BeginningOfTime;
                _originalBinItem.Audit = false;
                _originalBinItem.Pallet = new PalletItem();
                _storageProxy.SetAt(_originalBinItem.NodeIndex, _originalBinItem);
                _PopulateControls(_originalBinItem);
            }
            form.Dispose();
        }

        private void _LblBinNumber_DoubleClick(object sender, EventArgs e)
        {
            if (CanSwitchToView(Constant.AdminStorageViewName))
            {
                SetViewMessage(
                    Constant.StorageViewInitialNodeIndexMessageName,
                    _originalBinItem.NodeIndex);
                SwitchToView(Constant.AdminStorageViewName);
            }
        }
        private void _BtnGoToBin_Click(object sender, EventArgs e)
        {
            if (_searchResultsGrid.SelectedSearchResults != null
                && _searchResultsGrid.SelectedSearchResults.Count > 0)
            {
                GoToBinIndex(_searchResultsGrid.SelectedSearchResults[0].NodeIndex);
            }
        }

        private void _InventorySummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
//             InventorySummaryReport report = new InventorySummaryReport(_storageProxy.Items);
//             ReportPrintTool printTool = new ReportPrintTool(report);
//             printTool.ShowPreview();
        }

        private void _FullInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
//             FullInventoryReport report = new FullInventoryReport(_storageProxy.Items);
//             ReportPrintTool printTool = new ReportPrintTool(report);
//             printTool.ShowPreview();
        }

//         private void ReportPreviewForm_Load(object sender, EventArgs e)
//         {
//             PrintPreviewFormEx form = (PrintPreviewFormEx)sender;
//             frm.PrintingSystem.ExecCommand(PrintingSystemCommand.Scale, new object[] { 0.7f });
//         }

        private void _NavigatorBtnFillBin_Click(object sender, EventArgs e)
        {
            //QuickBinFillForm form = new QuickBinFillForm("Fill Bin");
            //if (form.ShowDialog(this) == DialogResult.OK)
            //{
            //    if (!MesQuery.TryFetchPalletItem(
            //        form.PalletID,
            //        out PalletItem palletItem))
            //    {
            //        _ = XMessageBox.Show(
            //            this,
            //            "No data exists for this pallet.",
            //            "Error",
            //            MessageBoxButtons.OK,
            //            MessageBoxIcon.Error);
            //        return;
            //    }
            //    _originalBinItem.BinStatus = BinStatus.Pickable;
            //    _originalBinItem.StoredOn = DateTime.Now;
            //    _originalBinItem.Pallet = palletItem;
            //    _storageProxy.SetAt(_originalBinItem.NodeIndex, _originalBinItem);
            //    _upperPitProxy.Remove(palletItem.PalletID);
            //    _lowerPitProxy.Remove(palletItem.PalletID);
            //    _PopulateControls(_originalBinItem);
            //}
            //form.Dispose();
        }

        private void _TxtComment_TextChanged(object sender, EventArgs e)
        {
            PalletItem palletItem = _workingBinItem.Pallet;
            palletItem.Comment = _txtComment.Text;
            _workingBinItem.Pallet = palletItem;
            _lblCommentLength.Text = $"{Constant.CommentLength - _txtComment.Text.Length}";
        }

        private void _BtnPickSku_Click(object sender, EventArgs e)
        {

        }

        private void _CmbSku_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void _CmbSku_TextUpdate(object sender, EventArgs e)
        {

        }

        private void _CmbPickSku_ValueMemberChanged(object sender, EventArgs e)
        {

        }

        private void _CmbSku_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                //                 if (!_skusProxy.ContainsKey(cmbPickSku.Text))
                //                 {
                //                     XMessageBox.Show(
                //                         this,
                //                         String.Format(
                //                             "'{0}' is not a valid SKU.",
                //                             cmbPickSku.Text),
                //                         "Invalid SKU",
                //                         MessageBoxButtons.OK,
                //                         MessageBoxIcon.Error);
                //                     cmbPickSku.SelectedItem = _workingBinItem.Pallet.PickSku;
                //                     cmbPickSku.SelectAll();
                //                 }
                //                 else if (cmbPickSku.Text != _workingBinItem.Pallet.PickSku)
                //                 {
                //                     PalletItem palletItem = _workingBinItem.Pallet;
                //                     palletItem.PickSku = cmbPickSku.Text;
                //                     _workingBinItem.Pallet = palletItem;
                //                 }
                //                 _ValidatePickSku();
            }
        }

        private void _CmbSku_Leave(object sender, EventArgs e)
        {
            //             if (!_skusProxy.ContainsKey(cmbPickSku.Text))
            //             {
            //                 XMessageBox.Show(
            //                     this,
            //                     String.Format(
            //                         "'{0}' is not a valid SKU.",
            //                         cmbPickSku.Text),
            //                     "Invalid SKU",
            //                     MessageBoxButtons.OK,
            //                     MessageBoxIcon.Error);
            //                 cmbPickSku.Focus();
            //                 cmbPickSku.SelectedItem = _workingBinItem.Pallet.PickSku;
            //                 cmbPickSku.SelectAll();
            //             }
            //             else if (cmbPickSku.Text != _workingBinItem.Pallet.PickSku)
            //             {
            //                 PalletItem palletItem = _workingBinItem.Pallet;
            //                 palletItem.PickSku = cmbPickSku.Text;
            //                 _workingBinItem.Pallet = palletItem;
            //             }
            //             _ValidatePickSku();
        }

        private void _SpnCrane_Enter(object sender, EventArgs e)
        {
            _spnCrane.ForeColor = Color.White;
            _spnCrane.BackColor = Color.Blue;
            _lblCrane.ForeColor = Color.White;
            _lblCrane.BackColor = Color.Blue;
        }

        private void _SpnSide_Enter(object sender, EventArgs e)
        {
            _spnSide.ForeColor = Color.White;
            _spnSide.BackColor = Color.Blue;
            _lblSide.ForeColor = Color.White;
            _lblSide.BackColor = Color.Blue;
        }

        private void _SpnHorizontal_Enter(object sender, EventArgs e)
        {
            _spnHorizontal.ForeColor = Color.White;
            _spnHorizontal.BackColor = Color.Blue;
            _lblHorizontal.ForeColor = Color.White;
            _lblHorizontal.BackColor = Color.Blue;
        }

        private void _SpnVertical_Enter(object sender, EventArgs e)
        {
            _spnVertical.ForeColor = Color.White;
            _spnVertical.BackColor = Color.Blue;
            lblVertical.ForeColor = Color.White;
            lblVertical.BackColor = Color.Blue;
        }

        private void _SpnCrane_Leave(object sender, EventArgs e)
        {
            _spnCrane.ForeColor = Color.Black;
            _spnCrane.BackColor = Color.White;
            _lblCrane.ForeColor = Color.Black;
            _lblCrane.BackColor = Color.White;
        }

        private void _SpnSide_Leave(object sender, EventArgs e)
        {
            _spnSide.ForeColor = Color.Black;
            _spnSide.BackColor = Color.White;
            _lblSide.ForeColor = Color.Black;
            _lblSide.BackColor = Color.White;
        }

        private void _SpnHorizontal_Leave(object sender, EventArgs e)
        {
            _spnHorizontal.ForeColor = Color.Black;
            _spnHorizontal.BackColor = Color.White;
            _lblHorizontal.ForeColor = Color.Black;
            _lblHorizontal.BackColor = Color.White;
        }

        private void _SpnVertical_Leave(object sender, EventArgs e)
        {
            _spnVertical.ForeColor = Color.Black;
            _spnVertical.BackColor = Color.White;
            lblVertical.ForeColor = Color.Black;
            lblVertical.BackColor = Color.White;
        }

        private void _LblCrane_Click(object sender, EventArgs e)
        {
            _ = _spnCrane.Focus();
        }

        private void _LblSide_Click(object sender, EventArgs e)
        {
            _ = _spnSide.Focus();
        }

        private void _LblHorizontal_Click(object sender, EventArgs e)
        {
            _ = _spnHorizontal.Focus();
        }

        private void _LblVertical_Click(object sender, EventArgs e)
        {
            _ = _spnVertical.Focus();
        }

        private void _StorageView_Load(object sender, EventArgs e)
        {

        }

        private void _ChkAudit_CheckedChanged(object sender, EventArgs e)
        {
            _workingBinItem.Audit = _chkAudit.Checked;
            //if (_originalBinItem.BinStatus == BinStatus.Empty)
            //{
            //    _workingBinItem.BinStatus = BinStatus.Offline;
            //}
        }

        private void _ByAuditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<BinItem> searchResults = Searches.Audits(_storageProxy.Items);
            _lastSearch.Type = SearchType.Audits;
            _lastSearch.BinItems = _storageProxy.Items;
            _ShowSearchResults(searchResults);
        }

        private void _PickOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<BinItem> searchResults = Searches.PickOnly(_storageProxy.Items);
            _lastSearch.Type = SearchType.PickOnly;
            _lastSearch.BinItems = _storageProxy.Items;
            _ShowSearchResults(searchResults);
        }

        private void _DisabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<BinItem> searchResults = Searches.Disabled(_storageProxy.Items);
            _lastSearch.Type = SearchType.Disabled;
            _lastSearch.BinItems = _storageProxy.Items;
            _ShowSearchResults(searchResults);
        }

        private void _LblBinNumber_Click(object sender, EventArgs e)
        {
            //             if (CanSwitchToView(Constant.AdminStorageViewName))
            //             {
            // //                 int nodeIndex = _workingBinItem.NodeIndex;
            //                 BinItem editGridItem = XDataItem.Clone(_workingBinItem);
            //                 XDataItemEditorForm<BinItem> editForm = new XDataItemEditorForm<BinItem>();
            //                 editForm.Initialize(
            //                     "Bin Item Editor",
            //                     editGridItem);
            //                 if (editForm.ShowDialog(this) == DialogResult.OK)
            //                 {
            //                     editGridItem = editForm.EditedDataItem;
            //                     _PopulateControls(editGridItem);
            // //                     _workingBinItem.NodeIndex = nodeIndex;
            //                     _SaveItem();
            // //                     BinItem editedItem = editForm.EditedDataItem;
            // //                     if (!_storageProxy.SafeSetAt(
            // //                         binItem.NodeIndex,
            // //                         editForm.OriginalDataItem,
            // //                         ref editedItem))
            // //                     {
            // //                         MessageBox.Show(
            // //                             this,
            // //                             "Edit failed because the original Bin Item was stale.",
            // //                             "Edit Failed");
            // //                     }
            //                 }
            //             }
        }

        private void _EnableConvertButtons()
        {
            bool enabled = _searchResults != null
                && _searchResults.Count > 0
                && ((_AreNewStatusAndHoldCodeAndCommentConsistent(
                    _newStatus,
                    _newHoldCode,
                    _newComment,
                    false)
                        && _newStatus > PalletStatus.Invalid)
                        || _markAudits || _unmarkAudits);

            _btnConvert.Enabled = enabled;
            _btnConvertAll.Enabled = enabled;
        }

        private void _ConvertAll(List<BinItem> searchResults)
        {
            if (!_AreNewStatusAndHoldCodeAndCommentConsistent(
                    _newStatus,
                    _newHoldCode,
                    _newComment,
                    true))
            {
                return;
            }
            ConvertAllStatusesForm form = new ConvertAllStatusesForm();
            bool applyToAll = false;
            DialogResult result = DialogResult.No;
            bool cancel = false;
            foreach (BinItem searchResult in searchResults)
            {
                PalletItem searchPallet = searchResult.Pallet;
                //if (applyToAll && result == DialogResult.Yes)

                //{
                //    Application.DoEvents();
                //    if (!form.Stop)
                //    {
                //        string message = string.Format(
                //            "Converting the status of Pallet {0} from '{1}' to '{2}'...",
                //            searchPallet.PalletID.ToString(Constant.PalletIDTextFormat),
                //            searchPallet.Status.ToText(),
                //            _newStatus.ToText());
                //        form.PulseProgress(message);
                //    }
                //    else
                //    {
                //        result = DialogResult.Cancel;
                //    }
                //}
                //else
                //{
                string message = string.Empty;
                bool isStatusConversion = true;
                if (_newStatus > PalletStatus.Invalid)
                {
                    message = string.Format(
                        "Do you want to convert the status of Pallet {0} from '{1}' to '{2}'?",
                        searchPallet.PalletID,
                        searchPallet.Status.ToText(),
                        _newStatus.ToText());
                    isStatusConversion = true;

                }
                else if (_markAudits || _unmarkAudits)
                {
                    isStatusConversion = false;
                    string auditString = _markAudits ? "mark" : "unmark";
                    message = $"Do you want to {auditString} Bin {searchResult.LocationText} for Audit?";
                }
                form.SetMessage(message);
                result = form.ShowDialog(this);
                applyToAll = form.ApplyToAll;
                if (applyToAll && result == DialogResult.Yes)
                {
                    string messageBoxString = string.Empty;
                    if (isStatusConversion)
                    {
                        messageBoxString = string.Format(
                            "Are you sure you want to convert the status of all {0} pallets to '{1}'?",
                            searchResults.Count,
                            _newStatus.ToText());
                    }
                    else
                    {
                        string auditString = _markAudits ? "mark" : "unmark";
                        messageBoxString = $"Do you want to " + auditString + $" all {searchResults.Count} pallets Audit?";
                    }
                    if (XMessageBox.Show(
                            this,
                                messageBoxString,
                                "Convert All?",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (form.UseBulkMode)
                        {
                            BulkPalletStatusConversionMessageData messageData = new BulkPalletStatusConversionMessageData(
                                searchResults,
                                _newStatus,
                                _newHoldCode,
                                _newComment,
                                _chkOverwriteComments.Checked,
                                _markAudits,
                                _unmarkAudits);
                            if (!XMessaging.SyncPublish(
                                out BulkPalletStatusConversionMessageData responseMessageData,
                                15000,
                                BulkPalletStatusConversionMessageData.BulkPalletStatusConversionMessageTopic,
                                messageData,
                                XMessageScopes.All,
                                this))
                            {
                                string errorMessage = "Request to change multiple statuses has timed out. The operation still may have completed correctly.";
                                XSystemEvent.Publish(
                                    string.Format("StorageView"),
                                    XSystemEventLevel.Error,
                                    errorMessage);
                                XMessageBox.Show(
                                    this,
                                    errorMessage,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }
                            if (!responseMessageData.Success)
                            {
                                StringBuilder failures = new StringBuilder();
                                foreach (BinItem binItem in responseMessageData.BinItems)
                                {
                                    PalletItem pallet = binItem.Pallet;
                                    failures.AppendField(
                                        "\n",
                                        string.Format(
                                            "Pallet {0}     Status {1}",
                                            pallet.PalletID,
                                            pallet.Status.ToText()));
                                }
                                string boxMessage = string.Format(
                                    "Stale data prevented the changes for the following pallets.\n\n{1}",
                                    _newStatus.ToText(),
                                    failures.ToString());
                                XMessageBox.Show(
                                    this,
                                    boxMessage,
                                    "Warning!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                            break;
                        }
                        //else
                        //{
                        //    message = string.Format(
                        //        "Converting the status of Pallet {0} from '{1}' to '{2}'...",
                        //        searchPallet.PalletID.ToString(Constant.PalletIDTextFormat),
                        //        searchPallet.Status.ToText(),
                        //        _newStatus.ToText());
                        //    form.SetProgressMode(searchResults.Count, message);
                        //    form.Owner = XApplication.MainForm;
                        //    form.StartPosition = FormStartPosition.Manual;
                        //    int x = form.Owner.Location.X + (form.Owner.Width - form.Width) / 2;
                        //    int y = form.Owner.Location.Y + (form.Owner.Height - form.Height) / 2;
                        //    form.Location = new System.Drawing.Point(x, y);
                        //    form.Show(this);
                        //}
                    }
                    else
                    {
                        break;
                    }
                }
                // }
                switch (result)
                {
                    case DialogResult.Yes:
                        {
                            BinItem originalBinItem = _storageProxy.GetAt(searchResult.NodeIndex);
                            BinItem newBinItem = XDataItem.Clone(originalBinItem);
                            PalletItem pallet = newBinItem.Pallet;
                            pallet.Status = _newStatus;
//                         if (!String.IsNullOrWhiteSpace(_newComment)
//                             && chkOverwriteComments.Checked)
//                         {
//                             pallet.Comment = _newComment;
//                         }
                            if (_newStatus == PalletStatus.Invalid)
                            {
                                pallet.Status = originalBinItem.Pallet.Status;
                            }
                            if (!string.IsNullOrWhiteSpace(_newComment)
                                && (_chkOverwriteComments.Checked
                                    || string.IsNullOrWhiteSpace(pallet.Comment)))
                            {
                                pallet.Comment = _newComment;
                            }
                            if (_markAudits)
                            {
                                newBinItem.Audit = true;
                            }
                            else if (_unmarkAudits)
                            {
                                newBinItem.Audit = false;
                            }
                            newBinItem.Pallet = pallet;
                            if (!_storageProxy.SafeSetAt(
                                originalBinItem.NodeIndex,
                                originalBinItem,
                                ref newBinItem)
                                && !applyToAll)
                            {
                                XMessageBox.Show(
                                    this,
                                    "The conversion failed because the local data item was stale.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                            _storageProxy.Refresh();
                            //if (!applyToAll)
                            //{
                            //    btnClearSearchResults.PerformClick();
                            //    _RedoLastSearch();
                            //    //_showNoMatchingResultsMessage = false;
                            //    //btnSearch.PerformClick();
                            //}
                        }
                        break;
                    case DialogResult.Cancel:
                        cancel = true;
                        break;
                    case DialogResult.No:
                        if (applyToAll)
                        {
                            cancel = true;
                        }
                        // Do nothing
                        break;
                }
                if (cancel)
                {
                    break;
                }
            }
            _storageProxy.Refresh();
            _btnClearSearchResults.PerformClick();
            //_showNoMatchingResultsMessage = false;
            //btnSearch.PerformClick();
            _RedoLastSearch();
            if ((applyToAll && result == DialogResult.Yes)
                || form.Stop)
            {
                form.Hide();
            }
        }

        private bool _AreNewStatusAndHoldCodeAndCommentConsistent(
            PalletStatus palletStatus,
            int holdCode,
            string comment,
            bool showMessage)
        {
            bool needHoldCode = false;
            bool needComment = false;
            switch (palletStatus)
            {
                case PalletStatus.Hold:
                    needComment = string.IsNullOrWhiteSpace(comment);
                    break;
                case PalletStatus.Unknown:
                case PalletStatus.Purge:
                    needComment = string.IsNullOrWhiteSpace(comment);
                    break;
            }
            string message = null;
            if (needComment && needHoldCode)
            {
                message = "You must provide a Hold Code and a Comment!";
            }
            else if (needComment)
            {
                message = "You must provide a Comment!";
            }
            else if (needHoldCode)
            {
                message = "You must provide a Hold Code!";
            }
            if (needComment || needHoldCode)
            {
                if (showMessage)
                {
                    XMessageBox.Show(
                        this,
                        message,
                        "Missing Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                return false;
            }
            return true;
        }

        private void _ConvertOne(BinItem searchResult)
        {
            if (!_AreNewStatusAndHoldCodeAndCommentConsistent(
                _newStatus,
                _newHoldCode,
                _newComment,
                true))
            {
                return;
            }
            PalletItem searchPallet = searchResult.Pallet;
            string message = string.Empty;
            //             bool isStatusConversion = true;
            if (_newStatus > PalletStatus.Invalid)
            {
                message = string.Format(
                    "Do you want to convert the status of Pallet {0} from '{1}' to '{2}'?",
                    searchPallet.PalletID,
                    searchPallet.Status.ToText(),
                    _newStatus.ToText());
                //                 isStatusConversion = true;

            }
            else if ((_markAudits || _unmarkAudits))
            {
                //                 isStatusConversion = false;
                string auditString = _markAudits ? "mark" : "unmark";
                message = $"Do you want to " + auditString + $" Pallet {searchPallet.PalletID} Audit?";
            }
            if (XMessageBox.Show(
                this,
                message,
                "Convert?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                BinItem originalBinItem = _storageProxy.GetAt(searchResult.NodeIndex);
                BinItem newBinItem = XDataItem.Clone(originalBinItem);
                PalletItem pallet = newBinItem.Pallet;
                pallet.Status = _newStatus;
                //                 if (!String.IsNullOrWhiteSpace(_newComment)
                //                     && chkOverwriteComments.Checked)
                //                 {
                //                     pallet.Comment = _newComment;
                //                 }
                if (_newStatus == PalletStatus.Invalid)
                {
                    pallet.Status = originalBinItem.Pallet.Status;
                }
                if (!string.IsNullOrWhiteSpace(_newComment)
                    && (_chkOverwriteComments.Checked
                        || string.IsNullOrWhiteSpace(pallet.Comment)))
                {
                    pallet.Comment = _newComment;
                }
                if (_markAudits)
                {
                    newBinItem.Audit = true;
                }
                else if (_unmarkAudits)
                {
                    newBinItem.Audit = false;
                }
                newBinItem.Pallet = pallet;
                if (!_storageProxy.SafeSetAt(
                    originalBinItem.NodeIndex,
                    originalBinItem,
                    ref newBinItem))
                {
                    XMessageBox.Show(
                        this,
                        "The conversion failed because the local data item was stale.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                //btnSearch.PerformClick();
                _storageProxy.Refresh();
                _RedoLastSearch();
            }
        }

        private void _RedoLastSearch()
        {
            _storageProxy.Refresh();
            _lastSearch.BinItems = _storageProxy.Items;
            List<BinItem> searchResults = new List<BinItem>();
            if (_lastSearch.Type == SearchType.AdvancedSearch && _advancedSearchForm != null)
            {
                (string orderBy, string thenBy1, string thenBy2) = _advancedSearchForm.GetOrderByValues();
                searchResults = Search.PerformAdvancedSearch(
                    _storageProxy.Items,
                    _advancedSearchForm.EmptyChecked,
                    _advancedSearchForm.ApplyCranes,
                    _advancedSearchForm.ApplyVehicleRow,
                    _advancedSearchForm.ApplySkus,
                    _advancedSearchForm.ApplyPalletStatus,
                    _advancedSearchForm.ApplyBinAttributes,
                    _advancedSearchForm.ApplyBinStatus,
                    _advancedSearchForm.ApplyDateTime,
                    _advancedSearchForm._searchSkus,
                    _advancedSearchForm._searchPalletStatuses,
                    _advancedSearchForm._searchBinStatuses,
                    AdvancedSearchForm.SearchCrane1,
                    AdvancedSearchForm.SearchCrane2,
                    AdvancedSearchForm.SearchCrane3,
                    AdvancedSearchForm.SearchCrane4,
                    AdvancedSearchForm.SearchCrane5,
                    _advancedSearchForm._searchCranes,
                    AdvancedSearchForm.SearchFront,
                    AdvancedSearchForm.SearchMid,
                    AdvancedSearchForm.SearchRear,
                    _advancedSearchForm._searchRows,
                    _advancedSearchForm._audit,
                    _advancedSearchForm._pickOnly,
                    _advancedSearchForm._disabled,
                    _advancedSearchForm._searchStartDateTime,
                    _advancedSearchForm._searchEndDateTime,
                    orderBy,
                    thenBy1,
                    thenBy2,
                    _advancedSearchForm.OrderByDescending,
                    _advancedSearchForm.ThenBy1Descending,
                    _advancedSearchForm.ThenBy2Descending);
                _ShowSearchResults(searchResults, true, false);
            }
            else
            {
                searchResults = _lastSearch.PerformSearch();
                _ShowSearchResults(searchResults, false, false);
            }
        }

        private void _BtnConvert_Click(object sender, EventArgs e)
        {
            if (_searchResultsGrid.SelectedSearchResults != null
                && _searchResultsGrid.SelectedSearchResults.Count > 0)
            {
                if (_searchResultsGrid.SelectedSearchResults.Count > 1)
                {
                    _ConvertAll(_searchResultsGrid.SelectedSearchResults);
                }
                else
                {
                    BinItem searchResult = _searchResultsGrid.SelectedSearchResults[0];
                    _ConvertOne(searchResult);
                }
            }
        }

        private void _BtnConvertAll_Click(object sender, EventArgs e)
        {
            _ConvertAll(_searchResults);
        }

        private void _ChkOverwriteComments_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void _TxtNewComment_TextChanged(object sender, EventArgs e)
        {
            _lblRemainingCharacters.Text = $"({Constant.CommentLength - _txtNewComment.Text.Length})";
            _newComment = _txtNewComment.Text;
            _EnableConvertButtons();
        }

        private void _CmbNewStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbNewStatus.SelectedIndex > -1)
            {
                _newStatus = _cmbNewStatus.GetSelectedEnumItem<PalletStatus>();
                switch (_newStatus)
                {
                    case PalletStatus.Invalid:
                        _lblNewComment.Visible = true;
                        _lblRemainingCharacters.Visible = true;
                        _markAudits = false;
                        _unmarkAudits = false;
                        _chkBulkMarkAudit.Checked = false;
                        _chkUnmarkAudit.Checked = false;
                        _chkBulkMarkAudit.Visible = true;
                        _chkUnmarkAudit.Visible = true;
                        break;
                    case PalletStatus.OK:
                    case PalletStatus.Hold:
                    case PalletStatus.Purge:
                    case PalletStatus.Reserve:
                    case PalletStatus.Stack:
                        _lblNewComment.Visible = true;
                        _lblRemainingCharacters.Visible = true;
                        _markAudits = false;
                        _unmarkAudits = false;
                        _chkBulkMarkAudit.Checked = false;
                        _chkUnmarkAudit.Checked = false;
                        _chkBulkMarkAudit.Visible = false;
                        _chkUnmarkAudit.Visible = false;
                        break;
//                     case PalletStatus.Hold:
//                         _lblNewComment.Visible = true;
//                         _lblRemainingCharacters.Visible = true;
//                         _markAudits = false;
//                         _unmarkAudits = false;
//                         _chkBulkMarkAudit.Checked = false;
//                         _chkUnmarkAudit.Checked = false;
//                         _chkBulkMarkAudit.Visible = false;
//                         _chkUnmarkAudit.Visible = false;
//                         break;
//                     case PalletStatus.QCSort:
//                         _lblNewComment.Visible = true;
//                         _lblRemainingCharacters.Visible = true;
//                         _markAudits = false;
//                         _unmarkAudits = false;
//                         _chkBulkMarkAudit.Checked = false;
//                         _chkUnmarkAudit.Checked = false;
//                         _chkBulkMarkAudit.Visible = false;
//                         _chkUnmarkAudit.Visible = false;
//                         break;

                }
                _txtNewComment.Clear();
                _txtNewComment.Visible = true;
                _chkOverwriteComments.Visible = true;
                _chkOverwriteComments.Checked = true;
            }
            else
            {
                _newStatus = PalletStatus.Invalid;
                _lblNewComment.Visible = false;
                _lblRemainingCharacters.Visible = false;
                _txtNewComment.Visible = false;
                _chkOverwriteComments.Visible = false;

                _markAudits = false;
                _unmarkAudits = false;
                _chkBulkMarkAudit.Checked = false;
                _chkUnmarkAudit.Checked = false;
                _chkBulkMarkAudit.Visible = false;
                _chkUnmarkAudit.Visible = false;
                _txtNewComment.Clear();
            }
            _EnableConvertButtons();
        }

        //         private void cmbNewStatus_SelectedIndexChanged(object sender, EventArgs e)
        //         {
        //             if (cmbNewStatus.SelectedIndex > 0)
        //             {
        //                 _newStatus = cmbNewStatus.GetSelectedEnumItem<PalletStatus>();
        //                 if (_newStatus == PalletStatus.OK)
        //                 {
        //                     lblNewHoldCode.Visible = false;
        //                     cmbNewHoldCode.Visible = false;
        //                     cmbNewHoldCode.Enabled = false;
        //                     lblNewComment.Visible = true;
        //                     lblRemainingCharacters.Visible = true;
        //                     txtNewComment.Visible = true;
        //                     chkOverwriteComments.Visible = true;
        //                 }
        //                 else if (_newStatus == PalletStatus.Hold)
        //                 {
        //                     lblNewHoldCode.Visible = true;
        //                     cmbNewHoldCode.Visible = true;
        //                     cmbNewHoldCode.Enabled = true;
        //                     lblNewComment.Visible = true;
        //                     lblRemainingCharacters.Visible = true;
        //                     txtNewComment.Visible = true;
        //                     chkOverwriteComments.Visible = true;
        //                 }
        //                 else
        //                 {
        //                     lblNewHoldCode.Visible = false;
        //                     cmbNewHoldCode.Visible = false;
        //                     cmbNewHoldCode.Enabled = false;
        //                     lblNewComment.Visible = true;
        //                     lblRemainingCharacters.Visible = true;
        //                     txtNewComment.Visible = true;
        //                     chkOverwriteComments.Visible = true;
        //                 }
        //                 cmbNewHoldCode.SelectedIndex = 0;
        //                 txtNewComment.Clear();
        //                 chkOverwriteComments.Checked = true;
        //
        // //                 grpBulkAudits.Enabled = false;
        // //                 grpBulkAudits.Visible = false;
        //                 _markAudits = false;
        //                 _unmarkAudits = false;
        //                 chkBulkMarkAudit.Checked = false;
        //                 chkUnmarkAudit.Checked = false;
        //                 chkBulkMarkAudit.Visible = false;
        //                 chkUnmarkAudit.Visible = false;
        //             }
        //             else if (cmbNewStatus.SelectedIndex == 0)
        //             {
        //                 _newStatus = PalletStatus.Invalid;
        //                 lblNewHoldCode.Visible = false;
        //                 cmbNewHoldCode.Visible = false;
        //                 cmbNewHoldCode.Enabled = false;
        //                 lblNewComment.Visible = true;
        //                 lblRemainingCharacters.Visible = true;
        //                 txtNewComment.Visible = true;
        //                 chkOverwriteComments.Visible = true;
        //
        //                 //                 grpBulkAudits.Enabled = true;
        //                 //                 grpBulkAudits.Visible = true;
        //                 _markAudits = false;
        //                 _unmarkAudits = false;
        //                 chkBulkMarkAudit.Checked = false;
        //                 chkUnmarkAudit.Checked = false;
        //                 chkBulkMarkAudit.Visible = true;
        //                 chkUnmarkAudit.Visible = true;
        //                 txtNewComment.Clear();
        //             }
        //             else
        //             {
        //                 _newStatus = PalletStatus.Invalid;
        //                 lblNewHoldCode.Visible = false;
        //                 cmbNewHoldCode.Visible = false;
        //                 cmbNewHoldCode.Enabled = false;
        //                 lblNewComment.Visible = false;
        //                 lblRemainingCharacters.Visible = false;
        //                 txtNewComment.Visible = false;
        //                 chkOverwriteComments.Visible = false;
        //
        // //                 grpBulkAudits.Enabled = false;
        // //                 grpBulkAudits.Visible = false;
        //                 _markAudits = false;
        //                 _unmarkAudits = false;
        //                 chkBulkMarkAudit.Checked = false;
        //                 chkUnmarkAudit.Checked = false;
        //                 chkBulkMarkAudit.Visible = false;
        //                 chkUnmarkAudit.Visible = false;
        //                 txtNewComment.Clear();
        //             }
        //             _EnableConvertButtons();
        //         }
        //
        private void _ChkBulkMarkAudit_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkBulkMarkAudit.Checked)
            {
                _chkUnmarkAudit.Checked = false;
                _unmarkAudits = false;
            }
            _markAudits = _chkBulkMarkAudit.Checked;
            _EnableConvertButtons();
        }

        private void _ChkUnmarkAudit_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkUnmarkAudit.Checked)
            {
                _chkBulkMarkAudit.Checked = false;
                _markAudits = false;
            }
            _unmarkAudits = _chkUnmarkAudit.Checked;
            _EnableConvertButtons();
        }

        private void _lblComment_TextChanged(object sender, EventArgs e)
        {
            _lblCommentLength.Text = (Constant.CommentLength - _lblComment.Text.Length).ToString();
        }

        //private void chkApplyBulkAudits_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkApplyBulkAudits.Checked)
        //    {
        //        grpBulkAudits.Enabled = true;
        //        grpBulkAudits.Visible = true;

        //        grpNewStatus.Enabled = false;
        //        _newStatus = PalletStatus.Invalid;
        //        _newHoldCode = 0;
        //        _newComment = string.Empty;
        //        cmbNewHoldCode.SelectedIndex = -1;
        //        cmbNewStatus.SelectedIndex = -1;
        //        txtNewComment.Text = string.Empty;
        //    }
        //    else
        //    {
        //        grpBulkAudits.Enabled = false;
        //        grpBulkAudits.Visible = false;
        //        _markAudits = false;
        //        _unmarkAudits = false;
        //        chkBulkMarkAudit.Checked = false;
        //        chkUnmarkAudit.Checked = false;
        //        grpNewStatus.Enabled = true;
        //    }
        //    _EnableConvertButtons();
        //}
    }
}
