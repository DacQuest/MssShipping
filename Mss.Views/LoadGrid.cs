using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.MessageBox;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.DataItemEditors;
using SourceGrid;
using SourceGrid.Cells;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Views
{
    public partial class LoadGrid : Grid
    {
        private Levels _level = Levels.None;
        private LoadProxy _loadProxy = null;
        private int _headerColumnWidth = 60;
        private int _dataColumnWidth = 245;
        private int _headerRowHeight = 26;
        private int _dataRowHeight = 38;
        private int _rowCount = Constant.LoadSize / 6;
        private StorageProxy _storageProxy = null;
//         private SystemSettingsProxy _systemSettingsProxy = null;

        private string _loadName;
        private bool _allowLabelReprint = false;
//         private bool _leftClickLabelReprint = false;
        private bool _allowItemEdit = false;
        private bool _allowRollback = false;
        private bool _showShortages = false;
        private bool _allowInsertEmpty = false;
        private int _clickLoadIndex = -1;
        private bool _flashInverted = false;
        private int _flashDelayedPalletTimeoutSeconds = 0;

        public LoadGrid()
        {
            InitializeComponent();
        }

//        protected override void OnPaint(PaintEventArgs pe)
//        {
//            base.OnPaint(pe);
//        }

        public void Initialize(
            string loadName,
            Levels level,
            bool allowLabelReprint,
            bool allowItemEdit,
            bool allowRollback,
            bool allowInsertEmpty,
            bool showShortages,
            int flashDelayedPalletTimeoutSeconds)
        {
            _loadName = loadName;
            _level = level;
            _allowLabelReprint = allowLabelReprint;
            _allowRollback = allowRollback;
            _allowItemEdit = allowItemEdit;
            _allowInsertEmpty = allowInsertEmpty;
            //             _leftClickLabelReprint = leftClickLabelReprint;
            _showShortages = showShortages;
            _flashDelayedPalletTimeoutSeconds = flashDelayedPalletTimeoutSeconds;

            BorderStyle = BorderStyle.FixedSingle;
            string title = $"{level.ToText()} {_loadName.Right(1)}";

            XProxyCache.Acquire(_loadName, out _loadProxy);
            _loadProxy.DataItemChanged += _Load_DataItemChanged;
            _loadProxy.CollectionRefreshed += _Load_ColletionRefreshed;
            if (_showShortages)
            {
                XProxyCache.Acquire(Constant.StorageName, out _storageProxy);
                _storageProxy.DataItemChanged += _Storage_DataItemChanged;
                _storageProxy.CollectionRefreshed += _Storage_CollectionRefreshed;
//                 XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
            }

            // Set up header attributes
            SourceGrid.Cells.Views.Header boldHeader = new SourceGrid.Cells.Views.Header
            {
                Font = new Font(Font, FontStyle.Bold),
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
            };

            ColumnsCount = 4;
            FixedColumns = 1;

            Rows.Insert(0);
            Rows[0].Height = _headerRowHeight;

            ICell cell;
            for (int index = 0; index < ColumnsCount; index++)
            {
                if (index == 0)
                {
                    Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
                    Columns[0].Width = _headerColumnWidth;
                    cell = new Header(title);
                }
                else
                {
                    Columns[index].AutoSizeMode = SourceGrid.AutoSizeMode.None;
                    Columns[index].Width = _dataColumnWidth;
                    int lane = index;
                    cell = new SourceGrid.Cells.ColumnHeader($"Lane {lane}");
                }
                cell.View = boldHeader;
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[0, index] = cell;
            }

            // Set up rows
            FixedRows = 1;
            for (int rowNumber = 1; rowNumber <= _rowCount; rowNumber++)
            {
                Rows.Insert(rowNumber);

                GridRow row = Rows[rowNumber];
                row.Height = _dataRowHeight;
                row.AutoSizeMode = SourceGrid.AutoSizeMode.None;

                // Row header
                cell = new Header((_rowCount - rowNumber + 1).ToString())
                {
                    View = boldHeader
                };
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[rowNumber, 0] = cell;

                for (int columnNumber = 1; columnNumber <= 3; columnNumber++)
                {
                    cell = new Cell();
                    cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                    cell.View = new SourceGrid.Cells.Views.Cell
                    {
                        TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
                    };
                    this[rowNumber, columnNumber] = cell;
                }
            }
            Reset();

            RefreshItems(_loadProxy.Items);

            _flashTimer.Enabled = _flashDelayedPalletTimeoutSeconds > 0;
        }

        public void Reset()
        {
            (Color fore, Color back) = LoadItem.GetLoadStatusColors(LoadItemStatus.Invalid, false, false);
            for (int rowNumber = 1; rowNumber <= _rowCount; rowNumber++)
            {
                ICell cell = this[rowNumber, 1];
                cell.View.Font = new Font("Courier New", 10F);

                cell = this[rowNumber, 2];
                cell.View.Font = new Font("Courier New", 10F);

                cell = this[rowNumber, 3];
                cell.View.Font = new Font("Courier New", 10F);
            }
        }

        //         public Levels Level => _level;

        private void _Load_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            RefreshItem((LoadItem)eventArgs.DataItem, false);
        }

        private void _Load_ColletionRefreshed(object sender, EventArgs eventArgs)
        {
            RefreshItems(_loadProxy.Items);
        }

        private void _Storage_DataItemChanged(object sender, XDataItemChangedEventArgs eventArgs)
        {
            RefreshItems(_loadProxy.Items);
        }

        private void _Storage_CollectionRefreshed(object sender, EventArgs eventArgs)
        {
            RefreshItems(_loadProxy.Items);
        }

//         private LoadItemStatus _GetStatusForDisplay(LoadItem loadItem)
//         {
//             if (loadItem.Has3rdRow)
//             {
//                 if (loadItem.Status <= loadItem.Status3rd)
//                 {
//                     return loadItem.Status;
//                 }
//                 else if (loadItem.Status3rd < loadItem.Status)
//                 {
//                     return loadItem.Status3rd;
//                 }
//             }
//             return loadItem.Status;
//         }

        public void RefreshItem(LoadItem loadItem, bool applyFlash)
        {
            if (loadItem.LoadLevel == _level)
            {
                //_ApplyShortages();
                Position position = ConvertToGridPosition(loadItem.NodeIndex);
                ICell cell = this[position.Row, position.Column];

                (Color fore, Color back) = LoadItem.GetLoadStatusColors(
                    loadItem.Status,
                    loadItem.Transferring,
                    applyFlash && _flashInverted);
                cell.View.ForeColor = fore;
                cell.View.BackColor = back;
                cell.Value = _FormatCellText(loadItem);
                Color borderColor = SystemColors.Control;
                bool wideBorder = false;
//                 if (loadItem.InsertEmpty)
//                 {
//                     borderColor = Color.Lime;
//                     wideBorder = true;
//                 }
//                 else
                if (_showShortages
                    && loadItem.Status != LoadItemStatus.Invalid
                    && loadItem.Shortage)
                {
                    borderColor = Color.Red;
                    wideBorder = true;
                }
                _DrawBorder(cell, wideBorder, borderColor);
                InvalidateCell(cell);
                Update();
            }
        }

        private void _DrawBorder(ICell cell, bool wideBorder, Color borderColor)
        {
            int borderWidth = wideBorder ? 3 : 1;
            cell.View.Border = new DevAge.Drawing.RectangleBorder(
                new DevAge.Drawing.BorderLine(borderColor, borderWidth),
                new DevAge.Drawing.BorderLine(borderColor, borderWidth),
                new DevAge.Drawing.BorderLine(borderColor, borderWidth),
                new DevAge.Drawing.BorderLine(borderColor, borderWidth));
            InvalidateCell(cell);
        }

        private string _FormatCellText(LoadItem loadItem)
        {
            if (loadItem.Status == LoadItemStatus.Invalid)
            {
                return string.Empty;
            }

            string sequence = loadItem.Broadcast.Csn;
            if (sequence.Length == 0)
            {
                sequence = new string('-', Constant.CsnLength);
            }

            string sku = loadItem.Broadcast.Sku;
            if (sku.Length == 0)
            {
                sku = new string('-', Constant.SkuLength - sku.Length);
            }

            string palletID;
            if (loadItem.Pallet.PalletID == Constant.NoPalletID)
            {
                palletID = new string('-', Constant.PalletIDLength);
            }
            else
            {
                palletID = loadItem.Pallet.PalletID;
            }

            string bottomSpacer = new string(
                ' ',
                Constant.LoadCellCharacterWidth
                    - Constant.PalletIDLength
                    - sequence.Length);
            return string.Format(
                "{0}\n{1}{2}{3}",
                sku,
                palletID,
                bottomSpacer,
                sequence);
        }

        public void RefreshItems()
        {
            _loadProxy.Refresh();
        }

        public void RefreshItems(IEnumerable<LoadItem> loadItems)
        {
            foreach (LoadItem loadItem in loadItems)
            {
                RefreshItem(loadItem, false);
            }
        }

        public static Position ConvertToGridPosition(int loadIndex)
        {
            int row = 10 - LoadItem.RowNumberFromNodeIndex(loadIndex) + 1;
            int col = LoadItem.LaneFromNodeIndex(loadIndex);


            return new Position(row, col);
        }

        private int _ConvertToLoadIndex(Position position)
        {
            return _ConvertToLoadIndex(position.Row, position.Column);
        }

        private int _ConvertToLoadIndex(int row, int column)
        {
            return ((10 - row) * 3) + (column - 1) + (_level == Levels.Upper ? Constant.LoadSize / 2 : 0);
        }

        private void _LoadGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right
                && m_MouseCellPosition.Row > 0
                && m_MouseCellPosition.Column > 0)
            {
                _clickLoadIndex = _ConvertToLoadIndex(m_MouseCellPosition);
                LoadItem loadItem = _loadProxy.Items[_clickLoadIndex];

                LoadItemStatus status = loadItem.Status;

                bool labelReprintPossible
                    = status == LoadItemStatus.Presequenced
                        || status == LoadItemStatus.Sequenced
                        || status == LoadItemStatus.Done
                        || status == LoadItemStatus.Loadable;

                contextMenuReprint.Visible = _allowLabelReprint && labelReprintPossible;
                contextMenuEditItem.Visible = _allowItemEdit;
//                 contextMenuRollback.Visible = _allowRollback
//                     && ((loadItem.Status < LoadItemStatus.Sequenced && loadItem.Status > LoadItemStatus.Pickable)
//                     || (loadItem.Status3rd < LoadItemStatus.Sequenced && loadItem.Status3rd > LoadItemStatus.Pickable));

                // Insert Empty
//                 bool allowInsertEmptyPallet = _allowInsertEmpty
//                     && (status == LoadItemStatus.Pending || status == LoadItemStatus.Pickable)
//                     && loadItem.Shortage;
//                 contextMenuInsertEmptyPallet.Text = loadItem.InsertEmpty
//                     ? "DO NOT Insert Empty Pallet"
//                     : "Insert Empty Pallet";
//                 contextMenuInsertEmptyPallet.Visible = allowInsertEmptyPallet;

                // Show Context Menu
//                 if (allowInsertEmptyPallet
//                     || (_allowLabelReprint && labelReprintPossible)
//                     || _allowItemEdit)
//                 {
//                     contextMenu.Show(this, e.X, e.Y);
//                 }
            }
        }

        private void _ReprintShippingLabel(int loadIndex)
        {
            LoadItem loadItem = _loadProxy.Items[loadIndex];

            ReprintLabelConfirmationForm form = new ReprintLabelConfirmationForm(
                $"Do you want to reprint the label(s) for Pallet {loadItem.Pallet.PalletID}");
            if (form.ShowDialog(this) == DialogResult.Yes)
            {
                XMessaging.Publish(
                    ReprintLabelMessageData.ReprintLabelRequest,
                    new ReprintLabelMessageData(loadItem.LoadLetter, loadItem.NodeIndex),
                    XMessageScopes.All,
                    this);
            }
            form.Dispose();
        }

        private void _Rollback(int loadIndex)
        {
            LoadItem loadItem = _loadProxy.Items[loadIndex];

            RollbackConfirmationForm form = new RollbackConfirmationForm(
                $"Do you want to rollback Pallet {loadItem.Pallet.PalletID}");
            if (form.ShowDialog(this) == DialogResult.Yes)
            {
                XMessaging.Publish(
                    RollbackLoadItemMessageData.RollbackRequest,
                    new RollbackLoadItemMessageData(loadItem.LoadLetter, loadItem.NodeIndex),
                    XMessageScopes.All,
                    this);
            }
            form.Dispose();
        }

        private void _EditItem(int loadIndex)
        {
            LoadItem loadItem = _loadProxy.Items[loadIndex];
            XDataItemEditorForm<LoadItem> editForm = new XDataItemEditorForm<LoadItem>();
            editForm.Initialize(
                "Load Item Editor",
                loadItem);
            if (editForm.ShowDialog(this) == DialogResult.OK)
            {
                LoadItem editedItem = editForm.EditedDataItem;
                if (!_loadProxy.SafeSetAt(
                    loadIndex,
                    editForm.OriginalDataItem,
                    ref editedItem))
                {
                    MessageBox.Show(
                        this,
                        "Edit failed because the original Load Item was stale.",
                        "Edit Failed");
                }
            }
            editForm.Dispose();
        }

//         private void _InsertEmptyPallet(int loadIndex)
//         {
//             LoadItem loadItem = _loadProxy.Items[loadIndex];
//             SupervisorAuthorizationAction action = loadItem.InsertEmpty
//                 ? SupervisorAuthorizationAction.CancelInsertEmpty
//                 : SupervisorAuthorizationAction.InsertEmpty;
//             AuthorizationForm form = new AuthorizationForm(action);
//             if (form.ShowDialog(this) == DialogResult.OK)
//             {
//                 LoadItem editedItem = XDataItem.Clone(loadItem);
//                 editedItem.InsertEmpty = !editedItem.InsertEmpty;
//                 if (!_loadProxy.SafeSetAt(
//                     loadIndex,
//                     loadItem,
//                     ref editedItem))
//                 {
//                     XMessageBox.Show(
//                         this,
//                         "Edit failed because the original Load Item was stale.",
//                         "Edit Failed");
//                 }
//             }
//             form.Dispose();
//         }

        private void _ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            contextMenu.Hide();
            if (ReferenceEquals(e.ClickedItem, contextMenuReprint))
            {
                _ReprintShippingLabel(_clickLoadIndex);
            }
            else if (ReferenceEquals(e.ClickedItem, contextMenuEditItem))
            {
                _EditItem(_clickLoadIndex);
            }
            else if (ReferenceEquals(e.ClickedItem, contextMenuRollback))
            {
                _Rollback(_clickLoadIndex);
            }
//             else if (ReferenceEquals(e.ClickedItem, contextMenuInsertEmptyPallet))
//             {
//                 _InsertEmptyPallet(_clickLoadIndex);
//             }
        }

        private void _flashTimer_Tick(object sender, EventArgs e)
        {
//             IEnumerable<LoadItem> delayedItems = _loadProxy.Items.Where(l => l.Level == _level && l.Delayed(_flashDelayedPalletTimeoutSeconds));
//             _flashInverted = !_flashInverted;
//             foreach (LoadItem loadItem in delayedItems)
//             {
//                 RefreshItem(loadItem, true);
//             }
        }
    }
}
