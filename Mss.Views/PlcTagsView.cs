using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
//using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.SnapInViews;
using Mss.Collections;
using Mss.Common;
using DacQuest.DFX.Devices.Tags;

namespace Mss.Views
{
    public partial class PlcTagsView : XSnapInView
    {
        private PlcTagsViewParameterSetWrapper _parameters;
        private BindingSource _bindingSource = new BindingSource();
        private PlcTagsProxy _plcTagsProxy;
        private string _currentDeviceName;
        private List<PlcTagItem> _currentTagList;

        public PlcTagsView()
        {
            InitializeComponent();

        }

        protected override void OpenView()
        {

            toolStripTitleLabel.Text = _parameters.AllowTagWrites ? "PLC Monitor/Editor" : "PLC Monitor";
            dataGrid.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGrid.DataSource = _bindingSource;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.AutoSize = false;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


            XProxyCache.Acquire(
                Constant.PlcTagsName,
                out _plcTagsProxy);
            _plcTagsProxy.DataItemChanged += _PlcMonitorProxy_DataItemChanged;
            _plcTagsProxy.CollectionRefreshed += _PlcMonitorProxy_CollectionRefreshed;


            foreach (string deviceName in _plcTagsProxy.Values.Select(p => p.DisplayName).Distinct().OrderBy(s => s))
            {
                toolStripDevicesComboBox.Items.Add(deviceName);
            }

            DataGridViewImageColumn imageColumn;
            DataGridViewTextBoxColumn column;

            imageColumn = new DataGridViewImageColumn
            {
                DataPropertyName = "QualityImage",
                HeaderText = "",
                Name = "QualityImageColumn"
            };
            dataGrid.Columns.Add(imageColumn);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Quality",
                DataPropertyName = "QualityText",
                Name = "QualityTextColumn",
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Device Name",
                DataPropertyName = "DeviceName",
                Name = "DeviceNameColumn",
                MinimumWidth = 70,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Tag Name",
                DataPropertyName = "TagName",
                Name = "TagNameColumn",
                MinimumWidth = 70,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Value",
                DataPropertyName = "ValueText",
                Name = "ValueColumn",
                MinimumWidth = 70,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Data Type",
                DataPropertyName = "DataTypeText",
                Name = "DataTypeColumn",
                MinimumWidth = 70,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Access",
                DataPropertyName = "AccessText",
                Name = "AccessColumn",
                MinimumWidth = 50,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "PLC Tag Name",
                DataPropertyName = "PlcTagName",
                Name = "PlcTagNameColumn",
                MinimumWidth = 70,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            dataGrid.Columns.Add(column);

            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "Last Update",
                DataPropertyName = "LastUpdatedOnText",
                Name = "LastUpdatedOnColumn",
                SortMode = DataGridViewColumnSortMode.Automatic,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dataGrid.Columns.Add(column);




            //             DataGridViewCheckBoxColumn checkBox;
            //             if (_parameters.AllowTagWrites)
            //             {
            //                checkBox = new DataGridViewCheckBoxColumn
            //                {
            //                    HeaderText = "Write Tag",
            //                    DataPropertyName = "WriteTagText",
            //                    Name = "WriteTag",
            //                    SortMode = DataGridViewColumnSortMode.Automatic,
            //                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            //                     
            //                };
            //                dataGrid.Columns.Add(checkBox);
            //                column = new DataGridViewTextBoxColumn
            //                {
            //                    HeaderText = "Write Tag Value",
            //                    DataPropertyName = "WriteTagValueText",
            //                    Name = "WriteTagValue",
            //                    SortMode = DataGridViewColumnSortMode.Automatic,
            //                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            // 
            //                };
            //                dataGrid.Columns.Add(column);
            //             }




            // 
            //             dataGrid.CellBeginEdit += _DataGrid_BeginCellEdit;
            //             dataGrid.CellClick += _DataGrid_CellClick;
            //             dataGrid.CellValueChanged += _DataGrid_CellChanged;
            _UpdateGrid();
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (PlcTagsViewParameterSetWrapper)parameters;
        }

        //         private void _DataGrid_BeginCellEdit(object sender, DataGridViewCellCancelEventArgs e)
        //         {
        //             if (!_parameters.AllowTagWrites || dataGrid.Columns[e.ColumnIndex].DataPropertyName != Constant.EditableDataGridColumnName)
        //             {
        //                 dataGrid.EndEdit();   
        //             }
        //         }

        //         private void _DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        //         {
        //             
        //         }

        //         private void _DataGrid_CellChanged(object sender, DataGridViewCellEventArgs e)
        //         {
        //             string key = dataGrid.Columns[e.ColumnIndex].DataPropertyName;
        //             if (_parameters.AllowTagWrites && key == Constant.EditableDataGridColumnName)
        //             {
        //                 if (_plcTagsProxy.TryGetItem(key, out PlcTagItem originalItem))
        //                 {
        //                     string cellValue = (string)dataGrid.CurrentCell.Value;
        //                     PlcTagItem workingItem = XDataItem.Clone(originalItem);
        //                     workingItem.WriteTag = true;
        //                     switch (originalItem.DataType)
        //                     {
        //                     case XValueDataType.Boolean:
        //                         if (cellValue == "0")
        //                         {
        //                             cellValue = "False";
        //                         }
        //                         else if (cellValue == "1")
        //                         {
        //                             cellValue = "True";
        //                         }
        //                         workingItem.NewBooleanValue = bool.Parse(cellValue); 
        //                         break;
        //                     case XValueDataType.Int16:
        //                         workingItem.NewInt16Value = short.Parse(cellValue);
        //                         break;
        //                     case XValueDataType.Int32:
        //                         workingItem.NewInt32Value = int.Parse(cellValue);
        //                         break;
        //                     case XValueDataType.String:
        //                             workingItem.NewStringValue = cellValue;
        //                             break;
        //                     }
        //                     try
        //                     {
        //                         UseWaitCursor = true; ;
        //                         if (!_plcTagsProxy.SafeUpdate(key, originalItem, ref workingItem))
        //                         {
        //                             UseWaitCursor = false;
        //                             XMessageBox.Show(
        //                                 this,
        //                                 "Update Failed. PLC Data was Stale.",
        //                                 "Error",
        //                                 MessageBoxButtons.OK,
        //                                 MessageBoxIcon.Error);
        //                         }
        //                         dataGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
        //                     }
        //                     finally
        //                     {
        //                         UseWaitCursor = false;
        //                     }
        //                 }
        //             }
        //         }

        private void _PlcMonitorProxy_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (((PlcTagItem)e.DataItem).DisplayName == _currentDeviceName)
            {
                _UpdateGrid();
            }
        }

        private void _PlcMonitorProxy_CollectionRefreshed(object sender, EventArgs e)
        {
            _UpdateGrid();
        }

        private void _UpdateGrid()
        {
            _currentTagList = _plcTagsProxy
                .Values
                .Where(item => item.DisplayName == _currentDeviceName)
                .OrderBy(item => item.TagName)
                .ToList();
            _bindingSource.DataSource = _currentTagList;
            dataGrid.DefaultCellStyle.ForeColor = Color.Black;
            dataGrid.Update();
        }

        private void _ToolStripRefreshButton_Click(object sender, EventArgs e)
        {
            _plcTagsProxy.Refresh();

        }

        private void _ToolStripDevicesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = toolStripDevicesComboBox.SelectedIndex;
            _currentDeviceName = (string)toolStripDevicesComboBox.Items[i];
            _UpdateGrid();
        }

        private void _DataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //             MessageBox.Show(e.RowIndex.ToString());
            int index = e.RowIndex;
            PlcTagItem tagItem = _currentTagList[index];
            if (!_parameters.AllowTagWrites
                || tagItem.AccessMode == XTagAccessMode.ReadOnly)
            {
                return;
            }
            PlcTagWriteForm form = new PlcTagWriteForm(tagItem, _plcTagsProxy);
            form.ShowDialog(this);
            form.Dispose();
        }
    }
}
