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
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;
using Mss.Common;
using DacQuest.DFX.Core.DataItems.Collections;

namespace Mss.Views
{
    public partial class PitView : XSnapInView
    {
        private PitViewParameterSetWrapper _parameters;
        private PitProxy _pitProxy;
        private HoldCodesProxy _holdCodesProxy;

        public PitView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {
            XProxyCache.Acquire(_parameters.CollectionName, out _pitProxy);
            _pitProxy.DataItemChanged += _PalletsProxy_DataItemChanged;
            _pitProxy.CollectionRefreshed += _PalletsProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.HoldCodesName, out _holdCodesProxy);

            _pitGrid.Initialize(
//                 _parameters.CollectionName,
                _pitProxy,
                _holdCodesProxy,
                _parameters.AllowEditing,
                true,
                this);

            navigatorBtnAddItem.Visible = false;
//             navigatorBtnQuickAdd.Visible = _parameters.AllowEditing
//                 && _parameters.CollectionName == Constant.InboundPitName;
            navigatorBtnAddSingleEmpty.Visible = false;
            navigatorBtnDelete.Visible = _parameters.AllowEditing;
        }

        private void _PalletsProxy_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _pitGrid.OnDataItemChanged(e);
        }

        private void _PalletsProxy_CollectionRefreshed(object sender, EventArgs e)
        {
            _pitGrid.OnCollectionRefreshed();
        }

        public void UpdateTitle(int rowCount)
        {
            string format = rowCount == 1 ? "{0}   ( {1} Pallet )" : "{0}   ( {1} Pallets )";
            lblCollectionName.Text = string.Format(
                format,
                _parameters.DisplayName,
                rowCount);
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (PitViewParameterSetWrapper)parameters;
        }

        private void _NavigatorBtnAddItem_Click(object sender, EventArgs e)
        {
//             palletGrid.Add();
            XMessageBox.Show(
                this,
                "NOT IMPLEMENTED!",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

        }

        private void _NavigatorBtnDelete_Click(object sender, EventArgs e)
        {
            _pitGrid.Delete();
        }

        private void _NavigatorBtnRefreshItem_Click(object sender, EventArgs e)
        {
            _pitProxy.Refresh();
            _holdCodesProxy.Refresh();
        }

        private void _NavigatorBtnQuickAdd_Click(object sender, EventArgs e)
        {
//             NumberEntryForm form = new NumberEntryForm("Add Pallet", 0);
//             if (form.ShowDialog(this) == DialogResult.OK)
//             {
//                 if (!MesInterface.FetchPalletItem(form.Value, out PalletItem palletItem))
//                 {
//                     XMessageBox.Show(
//                         this,
//                         "No data exists for this pallet.",
//                         "Error",
//                         MessageBoxButtons.OK,
//                         MessageBoxIcon.Error);
//                     return;
//                 }
//                 _pitProxy.Update(palletItem.PalletID, palletItem);
//             }
//             form.Dispose();
        }

        //protected override void AutoSubscribe()
        //{
        //}

        public override bool ViewClosing(bool force)
        {
            if (_pitProxy != null)
            {
                _pitProxy.DataItemChanged -= _PalletsProxy_DataItemChanged;
                _pitProxy.CollectionRefreshed -= _PalletsProxy_CollectionRefreshed;
                _pitProxy.Close();
                _pitProxy = null;
            }
            if (_holdCodesProxy != null)
            {
                _holdCodesProxy.Close();
                _holdCodesProxy = null;
            }
            return true;
        }

        private void _NavigatorBtnAddSingleEmpty_Click(object sender, EventArgs e)
        {
//             NumberEntryForm form = new NumberEntryForm("Enter Unique ID", 0);
//             if (form.ShowDialog(this) != DialogResult.OK)
//             {
//                 return;
//             }
//             int palletID = form.Value;
//             form.Dispose();
//             if (palletID <= 0)
//             {
//                 return;
//             }
//             string sku = string.Empty;
//             string comment = string.Empty;
//             if (XValueValidator.Validate(Constant.PalletIDValidatorName, palletID))
//             {
//                 //sku = XConfiguration.GetAlias(Constant.FirstRowEmptyPalletSkuName);
//                 comment = "Front Row Empty Pallet";
//             }
//             else
//             {
//                 XMessageBox.Show(
//                     this,
//                     "Not a valid Unique ID.",
//                     "Error",
//                     MessageBoxButtons.OK,
//                     MessageBoxIcon.Error);
//                 return;
//             }
//             PalletItem palletItem = new PitItem
//             {
//                 PalletID = palletID,
//                 Sku = sku,
//                 Status = PalletStatus.OK,
//                 InitialStatus = PalletStatus.OK,
//                 ReceivedOn = DateTime.Now,
//                 Comment = comment
//             };
// 
//             _pitProxy.Update(palletItem.PalletID, palletItem);
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
