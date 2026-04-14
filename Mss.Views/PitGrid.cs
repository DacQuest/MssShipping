using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.DataItemEditors;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Strings;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Cells.Models;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using DevExpress.CodeParser;

namespace Mss.Views
{
    public partial class PitGrid : Grid
    {
        public const int PalletIDColumnIndex = 1;
        public const int StatusColumnIndex = 2;
        public const int HoldCodeColumnIndex = 3;
        public const int SkuColumnIndex = 4;
        public const int JobIDColumnIndex = 5;
        public const int DestinationOrBuiltOnColumnIndex = 6;
        public const int BuiltOnColumnIndex = 7;
        public int TotalColumnCount = 8;

        public const int PalletIDColumnWidth = 70;
        public const int StatusColumnWidth = 110;
        public const int HoldCodeColumnWidth = 150;
        public const int SkuColumnWidth = 110;
        public const int JobIDColumnWidth = 110;
        public const int DestinationColumnWidth = 90;
        public const int ReceivedOnColumnWidth = 162;

        private List<PitItem> _pitItems;
//         private string _pitName;
        private string _currentPalletID = string.Empty;

        private PitProxy _pitProxy = null;
        private HoldCodesProxy _holdCodesProxy = null;

        private int _headerColumnWidth = 22;
        private int _headerRowHeight = 22;
        private int _dataRowHeight = 22;
        private SourceGrid.Cells.Controllers.ToolTipText _toolTipController;

        private bool _allowPalletEditing = false;
        private bool _showDestination = false;

        //        public event EventHandler<CurrentPalletIDChangedEventArgs> CurrentPalletIDChanged;

        //        protected virtual void OnCurrentPalletIDChanged(CurrentPalletIDChangedEventArgs e)
        //        {
        //            if (CurrentPalletIDChanged != null)
        //            {
        //                CurrentPalletIDChanged(this, e);
        //            }
        //        }

        public PitGrid()
        {
            InitializeComponent();
        }

        //        protected override void OnPaint(PaintEventArgs pe)
        //        {
        //            base.OnPaint(pe);
        //        }

        private class _ValueChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            PitGrid _pitGrid;

            public _ValueChangedEvent(PitGrid pitGrid)
            {
                _pitGrid = pitGrid;
            }

            public override void OnValueChanged(CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                Position position = sender.Position;
                PitItem pitItem = _pitGrid._pitItems[position.Row - 1];
                PitItem originalPitItem = new PitItem();
                if (_pitGrid._pitProxy.ContainsKey(pitItem.PalletID))
                {
                    originalPitItem.Copy(pitItem);
                }
                PalletItem palletItem = originalPitItem.Pallet;
                if (position.Column == StatusColumnIndex)
                {
                    PalletStatus previousStatus = palletItem.Status;
                    palletItem.Status = (PalletStatus)sender.Value;
                }
                else if (position.Column == HoldCodeColumnIndex)
                {
                    if (palletItem.Status == PalletStatus.Hold)
                    {
                        int previousHoldCode = palletItem.HoldCode;
                        palletItem.HoldCode = (int)sender.Value;
                    }
                }
                if (!_pitGrid._pitProxy.SafeUpdate(pitItem.PalletID, originalPitItem, ref pitItem))
                {
                    XMessageBox.Show(
                        _pitGrid,
                        "The Pallet Item that you edited was stale. The changes were not saved.",
                        "Changes not saved",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    _pitGrid._SetUpGrid();
                }
            }
        }

        public void Initialize(
            PitProxy pitProxy,
            HoldCodesProxy holdCodesProxy,
//             string palletCollectionName,
            bool allowPalletEditing,
            bool showDestination,
            Control parent)
        {
            Parent = parent;
            _pitProxy = pitProxy;
            _holdCodesProxy = holdCodesProxy;
//             _pitName = _pitProxy.CollectionName;
            _allowPalletEditing = allowPalletEditing;
            _showDestination = showDestination;

//             XProxyCache.Acquire(_palletCollectionName, out _pitProxy);
//             _pitProxy.DataItemChanged += _PalletsProxy_DataItemChanged;
//             _pitProxy.CollectionRefreshed += _PalletsProxy_CollectionRefreshed;

            XProxyCache.Acquire(Constant.HoldCodesName, out _holdCodesProxy);

            BorderStyle = BorderStyle.FixedSingle;

            _toolTipController = new SourceGrid.Cells.Controllers.ToolTipText();
            _toolTipController.IsBalloon = true;

            this.Controller.AddController(new _ValueChangedEvent(this));

            _SetUpGrid();

        }

//         private void _PalletsProxy_DataItemChanged(object sender, XDataItemChangedEventArgs e)
//         {
//             PalletItem palletItem = (PalletItem)e.DataItem;
//             if (e.MessageType == XSharedCollectionChangedMessageType.ItemRemoved
//                 && _currentPalletID == palletItem.PalletID)
//             {
//                 _currentPalletID = 0;
//             }
//             _SetUpGrid();
//         }

        public void OnDataItemChanged(XDataItemChangedEventArgs e)
        {
            PalletItem palletItem = (PalletItem)e.DataItem;
            if (e.MessageType == XSharedCollectionChangedMessageType.ItemRemoved
                && _currentPalletID == palletItem.PalletID)
            {
                _currentPalletID = Constant.NoPalletID;
            }
            if (Parent != null)
            {
                _SetUpGrid();
            }
        }

        //         private void _PalletsProxy_CollectionRefreshed(object sender, EventArgs e)
        //         {
        //             if (Parent != null)
        //             {
        //                 _SetUpGrid();
        //             }
        //             if (_pitProxy.ContainsKey(_currentPalletID))
        //             {
        //                 for (int rowNumber = 1; rowNumber <= RowsCount; rowNumber++)
        //                 {
        //                     ICell cell = (ICell)GetCell(rowNumber, PalletIDColumnIndex);
        //                     if (int.Parse(cell.DisplayText) == _currentPalletID)
        //                     {
        //                         Selection.SelectCell(
        //                             new Position(cell.Row.Index, cell.Column.Index),
        //                             true);
        //                         Selection.Invalidate();
        //                         break;
        //                     }
        //                 }
        //             }
        //             else
        //             {
        //                 _currentPalletID = 0;
        //             }
        //         }

        public void OnCollectionRefreshed()
        {
            if (_pitProxy.ContainsKey(_currentPalletID))
            {
                for (int rowNumber = 1;rowNumber <= RowsCount;rowNumber++)
                {
                    ICell cell = (ICell)GetCell(rowNumber, PalletIDColumnIndex);
                    if (cell.DisplayText == _currentPalletID)
                    {
                        Selection.SelectCell(
                            new Position(cell.Row.Index, cell.Column.Index),
                            true);
                        Selection.Invalidate();
                        break;
                    }
                }
            }
            else
            {
                _currentPalletID = Constant.NoPalletID;
            }
            if (Parent != null)
            {
                _SetUpGrid();
            }
        }

        //         public void RefreshData()
        //         {
        //             _pitProxy.Refresh();
        //         }

        public void Delete()
        {
            if (_currentPalletID.ValidPalletID())
            {
                //if (XMessageBox.Show(
                //    this,
                //    string.Format(
                //        "Do you want to DELETE Pallet {0}?",
                //        _currentPalletID.ToString(Constant.PalletIDTextFormat)),
                //    "Delete Pallet",
                //    MessageBoxButtons.YesNo,
                //    MessageBoxIcon.Warning,
                //    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                //{
                //    int palletIDToDelete = _currentPalletID;
                //    _currentPalletID = 0;
                //    _pitProxy.Remove(palletIDToDelete);
                //}
                string pitName = _pitProxy.CollectionName == Constant.UpperPitName
                    ? "Upper"
                    : "Lower";
                PitDeleteForm form = new PitDeleteForm(_currentPalletID, pitName);
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    string palletIDToDelete = _currentPalletID;
                    _currentPalletID = Constant.NoPalletID;
                    _ = _pitProxy.Remove(palletIDToDelete);
                }
                form.Dispose();
            }
        }

//         public void Add()
//         {
//             CreatePalletItemForm form = new CreatePalletItemForm(
//                 "Add New Pallet",
//                 //                 _palletCollectionName == Constant.PitName,
//                 Constant.PalletIDValidatorName);
//             if (form.ShowDialog(this) == DialogResult.OK)
//             {
//                 PalletItem palletItem = new PalletItem();
//                 palletItem.Copy(form.PalletItem);
//                 _currentPalletID = palletItem.PalletID;
//                 _pitProxy.Add(palletItem.PalletID, ref palletItem);
//                 _SetUpGrid();
//             }
//             form.Dispose();
//         }

        private void _SetUpGrid()
        {
            //         public Int32 DestinationColumnIndex = 8;
            //         public Int32 ReceivedColumnIndex = 9;
            //         public Int32 ColumnCount = 10;
            //
            //         public const Int32 DestinationColumnWidth = 40;
            //         public Int32 ReceivedColumnWidth = 130;

            _pitItems = _pitProxy.ItemListSortedByPalletID;
            RowsCount = 0;

            this.SelectionMode = GridSelectionMode.Cell;
            this.Selection.EnableMultiSelection = false;

            // Set up header attributes
            SourceGrid.Cells.Views.Header boldHeader = new SourceGrid.Cells.Views.Header();
            boldHeader.Font = new Font(Font, FontStyle.Bold);
            boldHeader.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

            int ColumnCount = TotalColumnCount;
            if (!_showDestination)
            {
                ColumnCount--;
            }

            ColumnsCount = ColumnCount; // for row header
            FixedColumns = 1;

            Rows.Insert(0);
            Rows[0].Height = _headerRowHeight;

            ICell cell;
            for (int index = 0; index < ColumnsCount; index++)
            {
                Columns[index].AutoSizeMode = SourceGrid.AutoSizeMode.None;
                cell = new SourceGrid.Cells.ColumnHeader();
                switch (index)
                {
                    case 0:
                        Columns[0].Width = _headerColumnWidth;
                        break;
//                     case UniqueIDColumnIndex:
//                         Columns[index].Width = UniqueIDColumnWidth;
//                         cell.Value = "Unique ID";
//                         break;
                    case PalletIDColumnIndex:
                        Columns[index].Width = PalletIDColumnWidth;
                        cell.Value = "Pallet";
                        break;
                    case StatusColumnIndex:
                        Columns[index].Width = StatusColumnWidth;
                        cell.Value = "Status";
                        break;
                    case HoldCodeColumnIndex:
                        Columns[index].Width = HoldCodeColumnWidth;
                        cell.Value = "Hold Code";
                        break;
                    case SkuColumnIndex:
                        Columns[index].Width = SkuColumnWidth;
                        cell.Value = "SKU";
                        break;
                    case JobIDColumnIndex:
                        Columns[index].Width = JobIDColumnWidth;
                        cell.Value = "Job ID";
                        break;
                    case DestinationOrBuiltOnColumnIndex:
                        if (_showDestination)
                        {
                            Columns[index].Width = DestinationColumnWidth;
                            cell.Value = "Dest";
                        }
                        else
                        {
                            Columns[index].Width = DestinationColumnWidth + ReceivedOnColumnWidth;
                            cell.Value = "Built On";
                        }
                        break;
                    case BuiltOnColumnIndex:
                        Columns[index].Width = ReceivedOnColumnWidth;
                        cell.Value = "Built On";
                        break;
                }
                cell.View = boldHeader;
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[0, index] = cell;
            }

            // Set up rows
            FixedRows = 1;
            int rowNumber = 0;
            foreach (PitItem pitItem in _pitItems)
            {
                rowNumber++;
                Rows.Insert(rowNumber);

                GridRow row = Rows[rowNumber];
                row.Height = _dataRowHeight;
                row.AutoSizeMode = SourceGrid.AutoSizeMode.None;

                // Row header
                cell = new Header();
                cell.View = boldHeader;
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[rowNumber, 0] = cell;

                for (int columnNumber = 1; columnNumber < ColumnsCount; columnNumber++)
                {
                    cell = new SourceGrid.Cells.Cell();
                    cell.View = new SourceGrid.Cells.Views.Cell();
                    cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                    switch (columnNumber)
                    {
//                         case UniqueIDColumnIndex:
//                             cell.Value = palletItem.UniqueID.ToString(Constant.UniqueIDTextFormat);
//                             break;
                        case StatusColumnIndex:
                            if (_allowPalletEditing)
                            {
                                PalletStatus[] statusValues = new PalletStatus[]
                                {
                                    PalletStatus.Invalid,
                                    PalletStatus.OK,
                                    PalletStatus.Hold,
                                    PalletStatus.Purge,
                                    PalletStatus.Unknown
                                };
                                string[] textValues = new string[]
                                {
                                    PalletStatus.Invalid.ToText(),
                                    PalletStatus.OK.ToText(),
                                    PalletStatus.Hold.ToText(),
                                    PalletStatus.Purge.ToText(),
                                    PalletStatus.Unknown.ToText()
                                };
                                SourceGrid.Cells.Editors.ComboBox statusComboBox = new SourceGrid.Cells.Editors.ComboBox(typeof(PalletStatus), statusValues, true);
                                statusComboBox.Control.FormattingEnabled = true;
                                DevAge.ComponentModel.Validator.ValueMapping statusValueMapping = new DevAge.ComponentModel.Validator.ValueMapping();
                                statusValueMapping.DisplayStringList = textValues;
                                statusValueMapping.ValueList = statusValues;
                                statusValueMapping.SpecialList = textValues;
                                statusValueMapping.SpecialType = typeof(string);
                                statusValueMapping.BindValidator(statusComboBox);
                                cell = new SourceGrid.Cells.Cell(pitItem.Pallet.Status);
                                statusComboBox.Control.DropDownStyle = ComboBoxStyle.DropDownList;
                                //                             statusComboBox.Control.Validated += _ComboBoxValue_Changed;
                                cell.Editor = statusComboBox;
                            }
                            else
                            {
                                //                            cell.Value = XEnum.GetText(palletItem.Status);
                                cell.Value = pitItem.Pallet.Status.ToText();
                            }
                            break;
                        case HoldCodeColumnIndex:
                            if (pitItem.Pallet.Status == PalletStatus.Hold)
                            {
                                if (_allowPalletEditing)
                                {

                                    int[] holdCodeValues = _holdCodesProxy.Values.Select(h => h.HoldCode).ToArray();
                                    if (holdCodeValues.Count() > 0 && _holdCodesProxy.Keys.Contains(pitItem.Pallet.HoldCode))
                                    {
                                        string[] textValues = _holdCodesProxy.Values.Select(h => h.Description).ToArray();
                                        SourceGrid.Cells.Editors.ComboBox holdCodeComboBox = new SourceGrid.Cells.Editors.ComboBox(typeof(int), holdCodeValues, true);
                                        holdCodeComboBox.Control.FormattingEnabled = true;
                                        DevAge.ComponentModel.Validator.ValueMapping holdCodeValueMapping = new DevAge.ComponentModel.Validator.ValueMapping();
                                        holdCodeValueMapping.DisplayStringList = textValues;
                                        holdCodeValueMapping.ValueList = holdCodeValues;
                                        holdCodeValueMapping.SpecialList = textValues;
                                        holdCodeValueMapping.SpecialType = typeof(string);
                                        holdCodeValueMapping.BindValidator(holdCodeComboBox);
                                        cell = new SourceGrid.Cells.Cell(pitItem.Pallet.HoldCode);
                                        holdCodeComboBox.Control.DropDownStyle = ComboBoxStyle.DropDownList;
                                        //                             statusComboBox.Control.Validated += _ComboBoxValue_Changed;
                                        cell.Editor = holdCodeComboBox;
                                        cell.AddController(_toolTipController);
                                        cell.ToolTipText = _holdCodesProxy[(int)cell.Value].Description;
                                    }
                                    else
                                    {
                                        cell.Value = pitItem.Pallet.HoldCode.ToText();
                                    }
                                }
                                else
                                {
                                    //                            cell.Value = XEnum.GetText(palletItem.Status);
                                    cell.Value = pitItem.Pallet.HoldCode.ToText();
                                }
                            }
                            else
                            {
                                cell.Value = string.Empty;
                            }
                            break;
                        case PalletIDColumnIndex:
                            cell.Value = pitItem.Pallet.PalletID;
                            break;
                        case SkuColumnIndex:
                            cell.Value = pitItem.Pallet.Sku;
                            break;
                        case JobIDColumnIndex:
                            cell.Value = pitItem.Pallet.JobID;
                            break;
                        case DestinationOrBuiltOnColumnIndex:
                            if (_showDestination)
                            {
                                cell.Value = pitItem.AssignedCrane == CraneNumber.None
                                    ? string.Empty
                                    : pitItem.AssignedCrane.ToText();
                            }
                            else
                            {
                                cell.Value = pitItem.Pallet.BuiltOn.ToString(Constant.DateTimeFormat);
                            }

                            break;
                        case BuiltOnColumnIndex:
                            cell.Value = pitItem.Pallet.BuiltOn.ToString(Constant.DateTimeFormat);
                            break;
                    }
                    this[rowNumber, columnNumber] = cell;
                }
            }
            ((PitView)Parent).UpdateTitle(RowsCount - 1);
        }

        //         private void _ComboBoxValue_Changed(Object sender, EventArgs e)
        //         {
        //             SourceGrid.Cells.Editors.ComboBox comboBox = (SourceGrid.Cells.Editors.ComboBox)sender;
        //             Position position = comboBox.EditPosition;
        //             PalletItem palletItem = _pallets[position.Row - 1];
        //             PalletItem originalPalletItem = new PalletItem();
        //             if (_pitProxy.ContainsKey(palletItem.PalletID))
        //             {
        //                 originalPalletItem.Copy(palletItem);
        //             }
        //             if (position.Column == StatusColumnIndex)
        //             {
        //                 PalletStatus previousStatus = palletItem.Status;
        //                 palletItem.Status = (PalletStatus)comboBox.GetEditedValue();
        //             }
        //             if (!_pitProxy.SafeUpdate(palletItem.PalletID, originalPalletItem, ref palletItem))
        //             {
        //                 XMessageBox.Show(
        //                     this,
        //                     "The Pallet Item that you edited was stale. The changes were not saved.",
        //                     "Changes not saved",
        //                     MessageBoxButtons.OK,
        //                     MessageBoxIcon.Warning);
        //                 _SetUpGrid();
        //             }
        //         }

        private void _PalletGrid_MouseClick(object sender, MouseEventArgs e)
        {
            _ = Focus();
            if (e.Button == MouseButtons.Left
                && m_MouseCellPosition.Row > 0
                && m_MouseCellPosition.Column > 0)
            {
                ICell cell = (ICell)GetCell(m_MouseCellPosition.Row, PalletIDColumnIndex);
                _currentPalletID = cell.DisplayText;
            }
        }


        private void _ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

    }
}
