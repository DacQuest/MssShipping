using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.MessageBox;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DacQuest.DFX.Core.Strings;

namespace Mss.Views
{
    public partial class AdvancedSearchForm : Form
    {
        public AdvancedSearchForm()
        {
            InitializeComponent();
        }

        private StorageProxy _storageProxy;

        public Search SearchCriteria = new Search();

        public List<string> _searchSkus = null;
        public PalletStatus _searchPalletStatuses = PalletStatus.Invalid;
        public BinStatus _searchBinStatuses = BinStatus.Invalid;
        public const int SearchCrane1 = 1;
        public const int SearchCrane2 = 2;
        public const int SearchCrane3 = 4;
        public const int SearchCrane4 = 8;
        public const int SearchCrane5 = 16;
        //         public int _searchCranes = _searchCrane1 + _searchCrane2 + _searchCrane3 + _searchCrane4;
        public int _searchCranes = SearchCrane1 + SearchCrane2 + SearchCrane3 + SearchCrane4 + SearchCrane5;
        public const int SearchFront = 1;
        public const int SearchMid = 2;
        public const int SearchRear = 4;
        public int _searchRows = SearchFront + SearchMid + SearchRear;
        public bool _audit = false;
        public bool _pickOnly = false;
        public bool _disabled = false;
        public DateTime _searchStartDateTime;
        public DateTime _searchEndDateTime;
        public bool OrderByDescending => _chkOrderByDescending.Checked;
        public bool ThenBy1Descending => _chkThenBy1Descending.Checked;
        public bool ThenBy2Descending => _chkThenBy2Descending.Checked;

        public bool ApplySkus => _chkApplySku.Checked;
        public bool ApplyBinStatus => _chkApplyBinStatus.Checked;
        public bool ApplyPalletStatus => _chkApplyPalletStatus.Checked;
        public bool ApplyVehicleRow => _chkApplyVehicleRow.Checked;
        public bool ApplyBinAttributes => _chkApplyBinAttributes.Checked;
        public bool ApplyCranes => _chkApplyCrane.Checked;
        public bool ApplyDateTime => _chkApplyDateTime.Checked;
        public bool EmptyChecked => _chkEmpty.Checked;

        public List<BinItem> SearchResults = new List<BinItem>();

        private void _AdvancedSearchForm_Load(object sender, EventArgs e)
        {
            XProxyCache.Acquire(Constant.StorageName, out _storageProxy);

            _PopulateSkuListBox(null);

            _searchEndDateTime = DateTime.Now;
            _searchStartDateTime = _searchEndDateTime - new TimeSpan(1, 0, 0, 0);
            _dtStart.Value = _searchStartDateTime;
            _dtEnd.Value = _searchEndDateTime;

            _EnableSearchButton();
            if (_cmbOrderBy.SelectedIndex == -1)
            {
                _cmbOrderBy.SelectedIndex = 0;
            }
        }

        private void _PopulateSkuListBox(string prefilter)
        {
            List<string> selectedSkus = _lstSkus.SelectedItems.Cast<string>().ToList();
            _lstSkus.Items.Clear();
            IEnumerable<string> skus = _storageProxy.GetAllSkusInStorage();

            if (!string.IsNullOrWhiteSpace(prefilter))
            {
                skus = skus.Where(s => s.StartsWith(prefilter.ToUpper()));
            }
            _lstSkus.Items.AddRange(skus.ToArray());
            for (int index = 0; index < _lstSkus.Items.Count; index++)
            {
                if (selectedSkus.Contains((string)_lstSkus.Items[index]))
                {
                    _lstSkus.SetSelected(index, true);
                }
            }
        }

        private void _AdvancedSearchDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_storageProxy != null)
            {
//                _storageProxy.DataItemChanged -= _StorageProxy_DataItemChanged;
//                _storageProxy.CollectionRefreshed -= _StorageProxy_CollectionRefreshed;
                XProxyCache.Release(_storageProxy);
                _storageProxy = null;
            }
            return;
        }


        private void _EnableSearchButton()
        {
            _btnSearch.Enabled = (_chkApplySku.Checked && _searchSkus != null && _searchSkus.Count() > 0)
                || (_chkApplyPalletStatus.Checked && _searchPalletStatuses != PalletStatus.Invalid)
                || (_chkApplyCrane.Checked && _searchCranes > 0)
                || (_chkApplyDateTime.Checked && _searchStartDateTime < _searchEndDateTime)
                || (_chkApplyBinStatus.Checked && _searchBinStatuses != BinStatus.Invalid)
                || (_chkApplyVehicleRow.Checked && _searchRows > 0)
                || (_chkApplyBinAttributes.Checked
                    && (_audit || _pickOnly || _disabled));
        }

        private void _ResetForm()
        {
            SearchResults = new List<BinItem>();

            _cmbOrderBy.SelectedIndex = 0;
            //cmbOrderBy.SelectedIndex = 0;
            //OrderBy = (string)cmbOrderBy.SelectedValue;

            _cmbThenBy1.SelectedIndex = -1;
            //ThenBy1 = null;

            _cmbThenBy2.SelectedIndex = -1;
            //ThenBy2 = null;
            _chkOrderByDescending.Checked = false;
            _chkThenBy1Descending.Checked = false;
            _chkThenBy2Descending.Checked = false;

            _chkApplySku.Checked = false;

            _lstSkus.SelectedIndex = -1;
            _searchSkus = null;

            _chkApplyBinStatus.Checked = false;
            _searchBinStatuses = BinStatus.Invalid;
            _chkEmpty.Checked = false;
            _chkGetAllocated.Checked = false;
            _chkPutAllocated.Checked = false;
            _chkPickable.Checked = false;
            _chkOffline.Checked = false;

            _chkApplyPalletStatus.Checked = false;
            _searchPalletStatuses = PalletStatus.Invalid;
            _chkOK.Checked = false;
            _chkHold.Checked = false;
            _chkQCSort.Checked = false;
//             _chkStack.Checked = false;
            _chkUnknown.Checked = false;

            _chkApplyCrane.Checked = false;
//             _searchCranes = _searchCrane1 + _searchCrane2 + _searchCrane3 + _searchCrane4;
            _searchCranes = SearchCrane1 + SearchCrane2 + SearchCrane3;
            _chkCrane1.Checked = true;
            _chkCrane2.Checked = true;
            _chkCrane3.Checked = true;
//             chkCrane4.Checked = true;

            _chkApplyVehicleRow.Checked = false;
//             _searchRows = _searchRow1 + _searchRow2 + _searchRow3;
            _searchRows = SearchFront + SearchMid;
            _chkFront.Checked = true;
            _chkMid.Checked = true;
//             chkRow3.Checked = true;

            _chkApplyBinAttributes.Checked = false;
            _audit = false;
            _chkAudit.Checked = false;
            _pickOnly = false;
            _chkPickOnly.Checked = false;
            _disabled = false;
            _chkDisabled.Checked = false;

//             _chkApplyJobIDRange.Checked = false;
//             _searchStartJobID = 0;
//             txtEndJobID.Text = string.Empty;
//             _searchEndJobID = 0;
//             txtStartJobID.Text = string.Empty;

            _chkApplyDateTime.Checked = false;
            _searchEndDateTime = DateTime.Now;
            _searchStartDateTime = _searchEndDateTime - new TimeSpan(1, 0, 0, 0);
            _dtStart.Value = _searchStartDateTime;
            _dtEnd.Value = _searchEndDateTime;

            _EnableSearchButton();
        }

        private void _ChkApplySku_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkApplySku.Checked)
            {
                _txtPrefilter.Focus();
            }
            else
            {
                _txtPrefilter.Clear();
                _PopulateSkuListBox(null);
                _lstSkus.SelectedIndex = -1;
                _searchSkus = null;
            }
            _EnableSearchButton();
        }

        private void _ChkApplyPalletStatus_CheckedChanged(object sender, EventArgs e)
        {
//             _grpPalletStatus.Enabled = _chkApplyPalletStatus.Checked;
            if (!_chkApplyPalletStatus.Checked)
            {
                _chkOK.Checked = false;
                _chkHold.Checked = false;
                _chkQCSort.Checked = false;
//                 _chkStack.Checked = false;
                _chkUnknown.Checked = false;
                _searchPalletStatuses = PalletStatus.Invalid;
            }
            _EnableSearchButton();
        }

        private void _ChkApplyDateTime_CheckedChanged(object sender, EventArgs e)
        {
            if (!_chkApplyDateTime.Checked)
            {
                _searchEndDateTime = DateTime.Now;
                _searchStartDateTime = _searchEndDateTime - new TimeSpan(1, 0, 0, 0);
                _dtStart.Value = _searchStartDateTime;
                _dtEnd.Value = _searchEndDateTime;
            }
            _EnableSearchButton();
        }

        private void _ChkApplyCrane_CheckedChanged(object sender, EventArgs e)
        {
            if (!_chkApplyCrane.Checked)
            {
                _chkCrane1.Checked = true;
                _chkCrane2.Checked = true;
                _chkCrane3.Checked = true;
//                 chkCrane4.Checked = true;
//                 _searchCranes = _searchCrane1 + _searchCrane2 + _searchCrane3 + _searchCrane4;
                _searchCranes = SearchCrane1 + SearchCrane2 + SearchCrane3;
            }
            _EnableSearchButton();
        }

        private void _LstSkus_SelectedIndexChanged(object sender, EventArgs e)
        {
            _searchSkus = _lstSkus.SelectedItems.Cast<string>().ToList();
            _EnableSearchButton();
        }

        private void _ChkOK_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkOK.Checked)
            {
                _searchPalletStatuses |= PalletStatus.OK;
            }
            else
            {
                _searchPalletStatuses &= ~PalletStatus.OK;
            }
            _EnableSearchButton();
        }

        private void _ChkHold_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkHold.Checked)
            {
                _searchPalletStatuses |= PalletStatus.Hold;
            }
            else
            {
                _searchPalletStatuses &= ~PalletStatus.Hold;
            }
            _EnableSearchButton();
        }

        //private void _ChkQCSort_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (_chkQCSort.Checked)
        //    {
        //        _searchPalletStatuses |= PalletStatus.QAPick;
        //    }
        //    else
        //    {
        //        _searchPalletStatuses &= ~PalletStatus.QAPick;
        //    }
        //    _EnableSearchButton();
        //}

//         private void _ChkStackPick_CheckedChanged(object sender, EventArgs e)
//         {
//             if (_chkStack.Checked)
//             {
//                 _searchPalletStatuses |= PalletStatus.Stack;
//             }
//             else
//             {
//                 _searchPalletStatuses &= ~PalletStatus.Stack;
//             }
//             _EnableSearchButton();
//         }

        private void _ChkUnknown_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkUnknown.Checked)
            {
                _searchPalletStatuses |= PalletStatus.Unknown;
            }
            else
            {
                _searchPalletStatuses &= ~PalletStatus.Unknown;
            }
            _EnableSearchButton();
        }

        private void _ChkCrane1_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkCrane1.Checked)
            {
                _searchCranes += SearchCrane1;
            }
            else
            {
                _searchCranes -= SearchCrane1;
            }
            _EnableSearchButton();
        }

        private void _ChkCrane2_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkCrane2.Checked)
            {
                _searchCranes += SearchCrane2;
            }
            else
            {
                _searchCranes -= SearchCrane2;
            }
            _EnableSearchButton();
        }

        private void _ChkCrane3_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkCrane3.Checked)
            {
                _searchCranes += SearchCrane3;
            }
            else
            {
                _searchCranes -= SearchCrane3;
            }
            _EnableSearchButton();
        }

        private void _ChkCrane4_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkCrane4.Checked)
            {
                _searchCranes += SearchCrane4;
            }
            else
            {
                _searchCranes -= SearchCrane4;
            }
            _EnableSearchButton();
        }

        private void _ChkCrane5_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkCrane5.Checked)
            {
                _searchCranes += SearchCrane5;
            }
            else
            {
                _searchCranes -= SearchCrane5;
            }
            _EnableSearchButton();
        }

        private void _DtStart_ValueChanged(object sender, EventArgs e)
        {
            _searchStartDateTime = (DateTime)_dtStart.Value;
            _EnableSearchButton();
        }

        private void _DtEnd_ValueChanged(object sender, EventArgs e)
        {
            _searchEndDateTime = (DateTime)_dtEnd.Value;
            _EnableSearchButton();
        }

        public (string OrderBy, string ThenBy1, string ThenBy2) GetOrderByValues()
        {
            string orderBy = _cmbOrderBy.SelectedIndex > -1 ? (string)_cmbOrderBy.SelectedItem : null;
            string thenBy1 = _cmbThenBy1.SelectedIndex > -1 ? (string)_cmbThenBy1.SelectedItem : null;
            string thenBy2 = _cmbThenBy2.SelectedIndex > -1 ? (string)_cmbThenBy2.SelectedItem : null;
            return (orderBy, thenBy1, thenBy2);
        }



        private void _BtnSearch_Click(object sender, EventArgs e)
        {
            _storageProxy.Refresh();
            SearchResults.Clear();
            string orderBy = _cmbOrderBy.SelectedIndex > -1 ? (string)_cmbOrderBy.SelectedItem : null;
            string thenBy1 = _cmbThenBy1.SelectedIndex > -1 ? (string)_cmbThenBy1.SelectedItem : null;
            string thenBy2 = _cmbThenBy2.SelectedIndex > -1 ? (string)_cmbThenBy2.SelectedItem : null;

            SearchResults = Search.PerformAdvancedSearch(
                _storageProxy.Items,
                _chkEmpty.Checked,
                _chkApplyCrane.Checked,
                _chkApplyVehicleRow.Checked,
                _chkApplySku.Checked,
                _chkApplyPalletStatus.Checked,
                _chkApplyBinAttributes.Checked,
                _chkApplyBinStatus.Checked,
                _chkApplyDateTime.Checked,
                _searchSkus,
                _searchPalletStatuses,
                _searchBinStatuses,
                SearchCrane1,
                SearchCrane2,
                SearchCrane3,
                SearchCrane4,
                SearchCrane5,
                _searchCranes,
                SearchFront,
                SearchMid,
                SearchRear,
                _searchRows,
                _audit,
                _pickOnly,
                _disabled,
                _searchStartDateTime,
                _searchEndDateTime,
                orderBy,
                thenBy1,
                thenBy2,
                _chkOrderByDescending.Checked,
                _chkThenBy1Descending.Checked,
                _chkThenBy2Descending.Checked);
            Hide();
            DialogResult = DialogResult.OK;
        }

        private void _CmbOrderBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbOrderBy.SelectedIndex > -1)
            //{
            //    OrderBy = (string)cmbOrderBy.SelectedValue;
            //}
            //else
            //{
            //    OrderBy = null;
            //}
        }

        private void _CmbThenBy1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbThenBy1.SelectedIndex > -1)
            //{
            //    ThenBy1 = (string)cmbOrderBy.SelectedValue;
            //}
            //else
            //{
            //    ThenBy1 = null;
            //}
        }

        private void _CmbThenBy2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbThenBy2.SelectedIndex > -1)
            //{
            //    ThenBy2 = (string)cmbThenBy2.SelectedValue;
            //}
            //else
            //{
            //    ThenBy2 = null;
            //}
        }

        private void _ChkApplyVehicleRow_CheckedChanged(object sender, EventArgs e)
        {
//             _grpVehicleRow.Enabled = _chkApplyVehicleRow.Checked;
            if (!_chkApplyVehicleRow.Checked)
            {
                _chkFront.Checked = true;
                _chkMid.Checked = true;
                _chkRear.Checked = true;
//                 _searchRows = _searchRow1 + _searchRow2 + _searchRow3;
                _searchRows = SearchFront + SearchMid + SearchRear;
            }
            _EnableSearchButton();
        }

        private void _ChkApplyBinStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (!_chkApplyBinStatus.Checked)
            {
                _chkEmpty.Checked = false;
                _chkPickable.Checked = false;
                _chkGetAllocated.Checked = false;
                _chkPutAllocated.Checked = false;
                _chkOffline.Checked = false;
                _searchBinStatuses = BinStatus.Invalid;
            }
            _EnableSearchButton();
        }

        private void _ChkApplyBinAttributes_CheckedChanged(object sender, EventArgs e)
        {
//             _grpBinAttribute.Enabled = _chkApplyBinAttributes.Checked;
            if (!_chkApplyBinAttributes.Checked)
            {
                _chkAudit.Checked = false;
                _chkPickOnly.Checked = false;
                _chkDisabled.Checked = false;
            }
            _EnableSearchButton();
        }

        private void _ChkFront_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkFront.Checked)
            {
                _searchRows += SearchFront;
            }
            else
            {
                _searchRows -= SearchFront;
            }
            _EnableSearchButton();
        }

        private void _ChkMid_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkMid.Checked)
            {
                _searchRows += SearchMid;
            }
            else
            {
                _searchRows -= SearchMid;
            }
            _EnableSearchButton();
        }

        private void _ChkRear_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkRear.Checked)
            {
                _searchRows += SearchRear;
            }
            else
            {
                _searchRows -= SearchRear;
            }
            _EnableSearchButton();
        }

        private void _ChkEmpty_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkEmpty.Checked)
            {
                _searchBinStatuses |= BinStatus.Empty;
            }
            else
            {
                _searchBinStatuses &= ~BinStatus.Empty;
            }
            _EnableSearchButton();
        }

        private void _ChkPickable_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkPickable.Checked)
            {
                _searchBinStatuses |= BinStatus.Pickable;
            }
            else
            {
                _searchBinStatuses &= ~BinStatus.Pickable;
            }
            _EnableSearchButton();
        }

        private void _ChkGetAllocated_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkGetAllocated.Checked)
            {
                _searchBinStatuses |= BinStatus.GetAllocated;
            }
            else
            {
                _searchBinStatuses &= ~BinStatus.GetAllocated;
            }
            _EnableSearchButton();
        }

        private void _ChkPutAllocated_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkPutAllocated.Checked)
            {
                _searchBinStatuses |= BinStatus.PutAllocated;
            }
            else
            {
                _searchBinStatuses &= ~BinStatus.PutAllocated;
            }
            _EnableSearchButton();
        }

        private void _ChkOffline_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkOffline.Checked)
            {
                _searchBinStatuses |= BinStatus.Offline | BinStatus.OfflineDuplicatePalletID;
            }
            else
            {
                _searchBinStatuses &= ~(BinStatus.Offline | BinStatus.OfflineDuplicatePalletID);
            }
            _EnableSearchButton();
        }

        private void _ChkAudit_CheckedChanged(object sender, EventArgs e)
        {
            _audit = _chkAudit.Checked;
            _EnableSearchButton();
        }

        private void _ChkPickOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkPickOnly.Checked)
            {
                _pickOnly = true;
            }
            else
            {
                _pickOnly = false;
            }
            _EnableSearchButton();
        }

        private void _ChkDisabled_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkDisabled.Checked)
            {
                _disabled = true;
            }
            else
            {
                _disabled = false;
            }
            _EnableSearchButton();
        }

        private void _BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
            DialogResult = DialogResult.Cancel;
        }

        private void _BtnReset_Click(object sender, EventArgs e)
        {
            _ResetForm();
        }

        private void _TxtPrefilter_TextChanged(object sender, EventArgs e)
        {
            _PopulateSkuListBox(_txtPrefilter.Text);
        }
    }
}