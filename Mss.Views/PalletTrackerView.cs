using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.SnapInViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mss.Common;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core;

namespace Mss.Views
{
    public partial class PalletTrackerView : XSnapInView
    {
        private string _palletID;

        public PalletTrackerView()
        {
            InitializeComponent();
        }

        protected override void OpenView()
        {
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
        }

        private void _BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
                using (SqlDataAdapter adapter = new SqlDataAdapter(
                    $"SELECT * FROM [{Constant.PalletTrackerTableName}] WHERE [PalletID]= '{_palletID}' ORDER BY [OccurredOn] {(_chkDescendingOrder.Checked ? "DESC" : "ASC")}",
                    connectionString))
                {
                    DataTable table = new DataTable();
                    _ = adapter.Fill(table);
                    _dgvRecords.DataSource = null;
                    _dgvRecords.Update();
                    _dgvRecords.DataSource = table;

                    foreach (DataGridViewColumn col in _dgvRecords.Columns)
                    {
                        if (col.ValueType == typeof(DateTime))
                        {
                            col.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss.fff";
                        }
                    }
                }
            }
            catch (Exception x)
            {
                _ = MessageBox.Show("Error loading Pallet Event records: " + x.Message);
            }
        }

        private void _BtnClear_Click(object sender, EventArgs e)
        {
            _txtPalletID.Clear();
            _dgvRecords.DataSource = null;
            _dgvRecords.Update();
        }

        private void _TxtPalletID_TextChanged(object sender, EventArgs e)
        {
            _palletID = _txtPalletID.Text.Trim();
            if (!_palletID.ValidPalletID())
            {
                _btnSearch.Enabled = false;
                _palletID = string.Empty;
                return;
            }
        }

        private void _TxtPalletID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == XSpecialCharacters.CR_CharValue)
            {
                _btnSearch.PerformClick();
                e.Handled = true;
            }
        }

        //protected override void AutoSubscribe()
        //{
        //}

        //public override bool ViewClosing(bool force)
        //{
        //    return true;
        //}

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
