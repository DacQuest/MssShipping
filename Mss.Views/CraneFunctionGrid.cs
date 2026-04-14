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
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Cells.Models;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using DacQuest.DFX.Core.DataItems;
using System.Threading;

namespace Mss.Views
{
    public partial class CraneFunctionGrid : Grid
    {
        public const int MasterSettingColumnIndex = 0;

        public const int Crane1ColumnIndex = 1;
        public const int Crane2ColumnIndex = 2;
        public const int Crane3ColumnIndex = 3;
        public const int Crane4ColumnIndex = 4;
        public const int ColumnCount = 5;

        public const int PalletCountRowIndex = 1;
        public const int EmptyLargeBinCountRowIndex = 2;
        public const int EmptySmallBinCountRowIndex = 3;
        public const int FrontStackCountRowIndex = 4;
        public const int RearStackCountRowIndex = 5;
        public const int CraneModeRowIndex = 6;
        public const int UpperInboundsRowIndex = 7;
        public const int LowerInboundsRowIndex = 8;
        public const int UpperOutboundsRowIndex = 9;
        public const int LowerOutboundsRowIndex = 10;
        public const int PrioritizeAuditPicksRowIndex = 11;
        public const int AuditPicksRowIndex = 12;
        public const int AutoCompactRowIndex = 13;
        public const int StoresRowIndex = 14;
        public const int LoadPicksRowIndex = 15;
        public const int PurgePicksRowIndex = 16;
        public const int StackPicksRowIndex = 17;
        public const int RowCount = 18;

        private SystemSettingsProxy _systemSettingsProxy = null;
        private StorageProxy _storageProxy = null;

        private bool _allowEdits = false;
        private bool _confirmEdits = true;

        private int _headerColumnWidth = 128;
        private int _dataColumnWidth = 80;
        private int _headerRowHeight = 25;
        private int _dataRowHeight = 25;

        public CraneFunctionGrid()
        {
            InitializeComponent();

        }

        public void Initialize(
            bool allowEdits,
            bool confirmEdits)
        {
            _allowEdits = allowEdits;
            _confirmEdits = confirmEdits;

            XProxyCache.Acquire(Constant.SystemSettingsName, out _systemSettingsProxy);
            _systemSettingsProxy.DataItemChanged += _SystemSettings_DataItemChanged;
            _systemSettingsProxy.CollectionRefreshed += _SystemSettings_CollectionRefreshed;

            XProxyCache.Acquire(Constant.StorageName, out _storageProxy);
            _storageProxy.DataItemChanged += _Storage_DataItemChanged;
            _storageProxy.CollectionRefreshed += _Storage_CollectionRefreshed;

            BorderStyle = BorderStyle.FixedSingle;

            // Set up header attributes
            SourceGrid.Cells.Views.Header boldHeader = new SourceGrid.Cells.Views.Header
            {
                Font = new Font(Font, FontStyle.Bold),
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
            };

            ColumnsCount = ColumnCount;
            FixedColumns = 1;

            Rows.Insert(0);
            Rows[0].Height = _headerRowHeight;

            ICell cell;
            for (int columnNumber = 0; columnNumber < ColumnCount; columnNumber++)
            {
                Columns[columnNumber].AutoSizeMode = SourceGrid.AutoSizeMode.None;
                string text = "???";
                int columnWidth = _dataColumnWidth;
                if (columnNumber == 0)
                {
                    text = "Master";
                    columnWidth = _headerColumnWidth;
                }
                else
                {
                    text = $"Crane {columnNumber}";
                }
                Columns[columnNumber].Width = columnWidth;
                cell = new SourceGrid.Cells.ColumnHeader(text)
                {
                    View = boldHeader
                };
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[0, columnNumber] = cell;
            }

            // Set up rows
            FixedRows = 1;
            for (int rowNumber = 1; rowNumber < RowCount; rowNumber++)
            {
                Rows.Insert(rowNumber);

                GridRow row = Rows[rowNumber];
                row.Height = _dataRowHeight;
                row.AutoSizeMode = SourceGrid.AutoSizeMode.None;

                // Row header
                if (rowNumber <= CraneModeRowIndex)
                {
                    string rowName = string.Empty;
                    switch (rowNumber)
                    {
                        case PalletCountRowIndex:
                            rowName = "Pallet Count";
                            break;
                        case EmptyLargeBinCountRowIndex:
                            rowName = "Large Empty Bins";
                            break;
                        case EmptySmallBinCountRowIndex:
                            rowName = "Small Empty Bins";
                            break;
                        case FrontStackCountRowIndex:
                            rowName = "Front Stacks";
                            break;
                        case RearStackCountRowIndex:
                            rowName = "Rear Stacks";
                            break;
                        case CraneModeRowIndex:
                            rowName = "Crane Mode";
                            break;
                    }

                    cell = new Header(rowName)
                    {
                        View = boldHeader
                    };
                    cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                    cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                    this[rowNumber, 0] = cell;

                    cell = new Cell();
                    cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                    cell.View = new SourceGrid.Cells.Views.Cell();
                    this[rowNumber, Crane1ColumnIndex] = cell;

                    cell = new Cell();
                    cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                    cell.View = new SourceGrid.Cells.Views.Cell();
                    this[rowNumber, Crane2ColumnIndex] = cell;

                    cell = new Cell();
                    cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                    cell.View = new SourceGrid.Cells.Views.Cell();
                    this[rowNumber, Crane3ColumnIndex] = cell;

                    cell = new Cell();
                    cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                    cell.View = new SourceGrid.Cells.Views.Cell();
                    this[rowNumber, Crane4ColumnIndex] = cell;
                }
                else
                {
                    string rowName = string.Empty;
                    Color foreColor;
                    Color backColor;
                    SystemSettingsItem item = _systemSettingsProxy.GetItem();
                    bool masterSettingEnabled;
                    cell = new Cell();
                    cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                    cell.View = new SourceGrid.Cells.Views.Cell();
                    switch (rowNumber)
                    {
                        case UpperInboundsRowIndex:
                            cell.Value = "Upper Inbounds";
                            masterSettingEnabled = item.UpperInboundsEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case LowerInboundsRowIndex:
                            cell.Value = "Lower Inbounds";
                            masterSettingEnabled = item.LowerInboundsEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case UpperOutboundsRowIndex:
                            cell.Value = "Upper Outbounds";
                            masterSettingEnabled = item.UpperOutboundsEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case LowerOutboundsRowIndex:
                            cell.Value = "Lower Outbounds";
                            masterSettingEnabled = item.LowerOutboundsEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case PrioritizeAuditPicksRowIndex:
                            cell.Value = "Prioritize Audits";
                            masterSettingEnabled = item.PrioritizeAuditPicks[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case AuditPicksRowIndex:
                            cell.Value = "Audits";
                            masterSettingEnabled = item.AuditPicksEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case AutoCompactRowIndex:
                            cell.Value = "Auto Compact";
                            masterSettingEnabled = item.AutoCompactStorageEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case StoresRowIndex:
                            cell.Value = "Stores";
                            masterSettingEnabled = item.StoresEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case LoadPicksRowIndex:
                            cell.Value = "Load Picks";
                            masterSettingEnabled = item.LoadPicksEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case PurgePicksRowIndex:
                            cell.Value = "Purge Picks";
                            masterSettingEnabled = item.PurgePicksEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                        case StackPicksRowIndex:
                            cell.Value = "Stack Picks";
                            masterSettingEnabled = item.StackPicksEnabled[Constant.MasterSettingArrayIndex];
                            (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                            cell.View.ForeColor = foreColor;
                            cell.View.BackColor = backColor;
                            cell.View.Font = new Font(Font, FontStyle.Bold);
                            cell.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                            break;
                    }

                    this[rowNumber, 0] = cell;

                    for (int columnNumber = 1; columnNumber < ColumnCount; columnNumber++)
                    {
                        cell = new Cell();
                        cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
                        cell.View = new SourceGrid.Cells.Views.Cell();
                        switch (rowNumber)
                        {
                            case UpperInboundsRowIndex:
                                bool settingEnabled = item.UpperInboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.UpperInboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LowerInboundsRowIndex:
                                settingEnabled = item.LowerInboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.LowerInboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case UpperOutboundsRowIndex:
                                settingEnabled = item.UpperOutboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.UpperOutboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LowerOutboundsRowIndex:
                                settingEnabled = item.LowerOutboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.LowerOutboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case PrioritizeAuditPicksRowIndex:
                                settingEnabled = item.PrioritizeAuditPicks[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.PrioritizeAuditPicks[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case AuditPicksRowIndex:
                                settingEnabled = item.AuditPicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.AuditPicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case AutoCompactRowIndex:
                                settingEnabled = item.AutoCompactStorageEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.AutoCompactStorageEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case StoresRowIndex:
                                settingEnabled = item.StoresEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.StoresEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LoadPicksRowIndex:
                                settingEnabled = item.LoadPicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.LoadPicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case PurgePicksRowIndex:
                                settingEnabled = item.PurgePicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.PurgePicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case StackPicksRowIndex:
                                settingEnabled = item.StackPicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.StackPicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                        }
                        this[rowNumber, columnNumber] = cell;
                    }
                }
            }
            _Update();
        }

        public (Color foreColor, Color backColor) GetCellColor(bool craneSettingEnable, bool masterSettingEnabled)
        {
            Color foreColor;
            Color backColor;
            if (craneSettingEnable && masterSettingEnabled)
            {
                backColor = Color.Lime;
                foreColor = Color.Black;
            }
            else if (craneSettingEnable && !masterSettingEnabled)
            {
                backColor = Color.ForestGreen;
                foreColor = Color.White;
            }
            else if (!craneSettingEnable && masterSettingEnabled)
            {
                backColor = Color.LightGray;
                foreColor = Color.Black;
            }
            else //(!craneSettingEnable && !masterSettingEnabled)
            {
                backColor = Color.Gray;
                foreColor = Color.Black;
            }
            return (foreColor, backColor);
        }

        public void RefreshData()
        {
            _systemSettingsProxy.Refresh();
            _storageProxy.Refresh();
        }

        private void _Update()
        {
            ICell cell;

            _storageProxy.GetPalletCountPerCrane(
                out int crane1Count,
                out int crane2Count,
                out int crane3Count,
                out int crane4Count);

            cell = this[PalletCountRowIndex, Crane1ColumnIndex];
            cell.Value = crane1Count;
            this[PalletCountRowIndex, Crane1ColumnIndex] = cell;

            cell = this[PalletCountRowIndex, Crane2ColumnIndex];
            cell.Value = crane2Count;
            this[PalletCountRowIndex, Crane2ColumnIndex] = cell;

            cell = this[PalletCountRowIndex, Crane3ColumnIndex];
            cell.Value = crane3Count;
            this[PalletCountRowIndex, Crane3ColumnIndex] = cell;

            cell = this[PalletCountRowIndex, Crane4ColumnIndex];
            cell.Value = crane4Count;
            this[PalletCountRowIndex, Crane4ColumnIndex] = cell;

            _storageProxy.GetEmptyLargeBinCountPerCrane(
                out crane1Count,
                out crane2Count,
                out crane3Count,
                out crane4Count);

            cell = this[EmptyLargeBinCountRowIndex, Crane1ColumnIndex];
            cell.Value = crane1Count;
            this[EmptyLargeBinCountRowIndex, Crane1ColumnIndex] = cell;

            cell = this[EmptyLargeBinCountRowIndex, Crane2ColumnIndex];
            cell.Value = crane2Count;
            this[EmptyLargeBinCountRowIndex, Crane2ColumnIndex] = cell;

            cell = this[EmptyLargeBinCountRowIndex, Crane3ColumnIndex];
            cell.Value = crane3Count;
            this[EmptyLargeBinCountRowIndex, Crane3ColumnIndex] = cell;

            cell = this[EmptyLargeBinCountRowIndex, Crane4ColumnIndex];
            cell.Value = crane4Count;
            this[EmptyLargeBinCountRowIndex, Crane4ColumnIndex] = cell;

            _storageProxy.GetEmptySmallBinCountPerCrane(
                out crane1Count,
                out crane2Count,
                out crane3Count,
                out crane4Count);

            cell = this[EmptySmallBinCountRowIndex, Crane1ColumnIndex];
            cell.Value = crane1Count;
            this[EmptySmallBinCountRowIndex, Crane1ColumnIndex] = cell;

            cell = this[EmptySmallBinCountRowIndex, Crane2ColumnIndex];
            cell.Value = crane2Count;
            this[EmptySmallBinCountRowIndex, Crane2ColumnIndex] = cell;

            cell = this[EmptySmallBinCountRowIndex, Crane3ColumnIndex];
            cell.Value = crane3Count;
            this[EmptySmallBinCountRowIndex, Crane3ColumnIndex] = cell;

            cell = this[EmptySmallBinCountRowIndex, Crane4ColumnIndex];
            cell.Value = crane4Count;
            this[EmptySmallBinCountRowIndex, Crane4ColumnIndex] = cell;

            _storageProxy.GetStackCountPerCrane(
                Constant.StackSku1,
                out crane1Count,
                out crane2Count,
                out crane3Count,
                out crane4Count);

            cell = this[FrontStackCountRowIndex, Crane1ColumnIndex];
            cell.Value = crane1Count;
            this[FrontStackCountRowIndex, Crane1ColumnIndex] = cell;

            cell = this[FrontStackCountRowIndex, Crane2ColumnIndex];
            cell.Value = crane2Count;
            this[FrontStackCountRowIndex, Crane2ColumnIndex] = cell;

            cell = this[FrontStackCountRowIndex, Crane3ColumnIndex];
            cell.Value = crane3Count;
            this[FrontStackCountRowIndex, Crane3ColumnIndex] = cell;

            cell = this[FrontStackCountRowIndex, Crane4ColumnIndex];
            cell.Value = crane4Count;
            this[FrontStackCountRowIndex, Crane4ColumnIndex] = cell;

            _storageProxy.GetStackCountPerCrane(
                Constant.StackSku2,
                out crane1Count,
                out crane2Count,
                out crane3Count,
                out crane4Count);

            cell = this[RearStackCountRowIndex, Crane1ColumnIndex];
            cell.Value = crane1Count;
            this[RearStackCountRowIndex, Crane1ColumnIndex] = cell;

            cell = this[RearStackCountRowIndex, Crane2ColumnIndex];
            cell.Value = crane2Count;
            this[RearStackCountRowIndex, Crane2ColumnIndex] = cell;

            cell = this[RearStackCountRowIndex, Crane3ColumnIndex];
            cell.Value = crane3Count;
            this[RearStackCountRowIndex, Crane3ColumnIndex] = cell;

            cell = this[RearStackCountRowIndex, Crane4ColumnIndex];
            cell.Value = crane4Count;
            this[RearStackCountRowIndex, Crane4ColumnIndex] = cell;

            SystemSettingsItem item = _systemSettingsProxy.GetItem();

            cell = this[CraneModeRowIndex, Crane1ColumnIndex];
            CraneMode mode = item.CraneModes[Crane1ColumnIndex - 1];
            cell.Value = mode.ToText();
            cell.View.BackColor = mode == CraneMode.Manual
                ? Color.LightGray
                : mode == CraneMode.SemiAuto
                    ? Color.Yellow 
                    : Color.Lime;
            this[CraneModeRowIndex, Crane1ColumnIndex] = cell;

            cell = this[CraneModeRowIndex, Crane2ColumnIndex];
            mode = item.CraneModes[Crane2ColumnIndex - 1];
            cell.Value = mode.ToText();
            cell.View.BackColor = mode == CraneMode.Manual
                ? Color.LightGray
                : mode == CraneMode.SemiAuto
                    ? Color.Yellow
                    : Color.Lime;
            this[CraneModeRowIndex, Crane2ColumnIndex] = cell;

            cell = this[CraneModeRowIndex, Crane3ColumnIndex];
            mode = item.CraneModes[Crane3ColumnIndex - 1];
            cell.Value = mode.ToText();
            cell.View.BackColor = mode == CraneMode.Manual
                ? Color.LightGray
                : mode == CraneMode.SemiAuto
                    ? Color.Yellow
                    : Color.Lime;
            this[CraneModeRowIndex, Crane3ColumnIndex] = cell;

            cell = this[CraneModeRowIndex, Crane4ColumnIndex];
            mode = item.CraneModes[Crane4ColumnIndex - 1];
            cell.Value = mode.ToText();
            cell.View.BackColor = mode == CraneMode.Manual
                ? Color.LightGray
                : mode == CraneMode.SemiAuto
                    ? Color.Yellow
                    : Color.Lime;
            this[CraneModeRowIndex, Crane4ColumnIndex] = cell;

            Color foreColor;
            Color backColor;
            bool masterSettingEnabled;
            for (int columnNumber = 0; columnNumber < ColumnCount; columnNumber++)
            {
                for (int rowNumber = 3; rowNumber < RowCount; rowNumber++)
                {
                    cell = this[rowNumber, columnNumber];
                    if (columnNumber == 0)
                    {
                        switch (rowNumber)
                        {
                            case UpperInboundsRowIndex:
                                masterSettingEnabled = item.UpperInboundsEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LowerInboundsRowIndex:
                                masterSettingEnabled = item.LowerInboundsEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case UpperOutboundsRowIndex:
                                masterSettingEnabled = item.UpperOutboundsEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LowerOutboundsRowIndex:
                                masterSettingEnabled = item.LowerOutboundsEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case PrioritizeAuditPicksRowIndex:
                                masterSettingEnabled = item.PrioritizeAuditPicks[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case AuditPicksRowIndex:
                                masterSettingEnabled = item.AuditPicksEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case AutoCompactRowIndex:
                                masterSettingEnabled = item.AutoCompactStorageEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case StoresRowIndex:
                                masterSettingEnabled = item.StoresEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LoadPicksRowIndex:
                                masterSettingEnabled = item.LoadPicksEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case PurgePicksRowIndex:
                                masterSettingEnabled = item.PurgePicksEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case StackPicksRowIndex:
                                masterSettingEnabled = item.StackPicksEnabled[Constant.MasterSettingArrayIndex];
                                (foreColor, backColor) = GetCellColor(masterSettingEnabled, masterSettingEnabled);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                        }
                    }
                    else
                    {
                        switch (rowNumber)
                        {
                            case UpperInboundsRowIndex:
                                bool settingEnabled = item.UpperInboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.UpperInboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LowerInboundsRowIndex:
                                settingEnabled = item.LowerInboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.LowerInboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case UpperOutboundsRowIndex:
                                settingEnabled = item.UpperOutboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.UpperOutboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LowerOutboundsRowIndex:
                                settingEnabled = item.LowerOutboundsEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.LowerOutboundsEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case PrioritizeAuditPicksRowIndex:
                                settingEnabled = item.PrioritizeAuditPicks[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.PrioritizeAuditPicks[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case AuditPicksRowIndex:
                                settingEnabled = item.AuditPicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.AuditPicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case AutoCompactRowIndex:
                                settingEnabled = item.AutoCompactStorageEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.AutoCompactStorageEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case StoresRowIndex:
                                settingEnabled = item.StoresEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.StoresEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case LoadPicksRowIndex:
                                settingEnabled = item.LoadPicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.LoadPicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case PurgePicksRowIndex:
                                settingEnabled = item.PurgePicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.PurgePicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                            case StackPicksRowIndex:
                                settingEnabled = item.StackPicksEnabled[columnNumber];
                                cell.Value = settingEnabled == true ? "Enabled" : "Disabled";
                                (foreColor, backColor) = GetCellColor(settingEnabled, item.StackPicksEnabled[Constant.MasterSettingArrayIndex]);
                                cell.View.ForeColor = foreColor;
                                cell.View.BackColor = backColor;
                                break;
                        }
                    }
                    this[rowNumber, columnNumber] = cell;
                }
            }
            Invalidate();
            Update();
        }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _Update();
        }

        private void _SystemSettings_CollectionRefreshed(object sender, EventArgs e)
        {
            _Update();
        }

        private void _Storage_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _Update();
        }

        private void _Storage_CollectionRefreshed(object sender, EventArgs e)
        {
            _Update();
        }

        private string _GetCraneSettingFromRowNumber(int rowNumber)
        {
            switch (rowNumber)
            {
                case UpperInboundsRowIndex:
                    return "Upper Inbounds";
                case LowerInboundsRowIndex:
                    return "Lower Inbounds";
                case UpperOutboundsRowIndex:
                    return "Upper Outbounds";
                case LowerOutboundsRowIndex:
                    return "Lower Outbounds";
                case PrioritizeAuditPicksRowIndex:
                    return "Prioritize Audits";
                case AuditPicksRowIndex:
                    return "Audits";
                case AutoCompactRowIndex:
                    return "Auto Compact";
                case StoresRowIndex:
                    return "Stores";
                case LoadPicksRowIndex:
                    return "Load Picks";
                case PurgePicksRowIndex:
                    return "Purge Picks";
                case StackPicksRowIndex:
                    return "Stack Picks";
                default:
                    return string.Empty;
            }
        }

        private (string settingName, bool settingValue) _GetSettingInfo(int rowNumber, int columnNumber)
        {
            string settingName = string.Empty;
            bool settingValue = false;

            switch (rowNumber)
            {
                case UpperInboundsRowIndex:
                    return (nameof(SystemSettingsItem.UpperInboundsEnabled), _systemSettingsProxy.Item.UpperInboundsEnabled[columnNumber]);
                case LowerInboundsRowIndex:
                    return (nameof(SystemSettingsItem.LowerInboundsEnabled), _systemSettingsProxy.Item.LowerInboundsEnabled[columnNumber]);
                case UpperOutboundsRowIndex:
                    return (nameof(SystemSettingsItem.UpperOutboundsEnabled), _systemSettingsProxy.Item.UpperOutboundsEnabled[columnNumber]);
                case LowerOutboundsRowIndex:
                    return (nameof(SystemSettingsItem.LowerOutboundsEnabled), _systemSettingsProxy.Item.LowerOutboundsEnabled[columnNumber]);
                case PrioritizeAuditPicksRowIndex:
                    return (nameof(SystemSettingsItem.PrioritizeAuditPicks), _systemSettingsProxy.Item.PrioritizeAuditPicks[columnNumber]);
                case AuditPicksRowIndex:
                    return (nameof(SystemSettingsItem.AuditPicksEnabled), _systemSettingsProxy.Item.AuditPicksEnabled[columnNumber]);
                case AutoCompactRowIndex:
                    return (nameof(SystemSettingsItem.AutoCompactStorageEnabled), _systemSettingsProxy.Item.AutoCompactStorageEnabled[columnNumber]);
                case StoresRowIndex:
                    return (nameof(SystemSettingsItem.StoresEnabled), _systemSettingsProxy.Item.StoresEnabled[columnNumber]);
                case LoadPicksRowIndex:
                    return (nameof(SystemSettingsItem.LoadPicksEnabled), _systemSettingsProxy.Item.LoadPicksEnabled[columnNumber]);
                case PurgePicksRowIndex:
                    return (nameof(SystemSettingsItem.PurgePicksEnabled), _systemSettingsProxy.Item.PurgePicksEnabled[columnNumber]);
                case StackPicksRowIndex:
                    return (nameof(SystemSettingsItem.StackPicksEnabled), _systemSettingsProxy.Item.StackPicksEnabled[columnNumber]);
            }
            return (settingName, settingValue);
        }

        private string _GetOtherActionName(bool currentSetting) => currentSetting ? "Disable" : "Enable";

        private void _CraneFunctionGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //             if (!_allowEdits)
            //             {
            //                 return;
            //             }
            //             if (e.Button == MouseButtons.Left
            //                 && m_MouseCellPosition.Row > 2)
            //             {
            //                 int rowNumber = m_MouseCellPosition.Row;
            //                 int columnNumber = m_MouseCellPosition.Column;
            //                 SystemSettingsItem item = _systemSettingsProxy.GetItem();
            //                 Cell cell = (Cell)this[rowNumber, columnNumber];
            //                 string cellValue = (string)cell.Value;
            //
            //                 (string settingName, bool settingValue) = GetSettingInfo(rowNumber, columnNumber);
            //                 if (columnNumber == 0)
            //                 {
            //                     if (_confirmEdits
            //                         && XMessageBox.Show(
            //                             this,
            //                             $"Do you want to {GetOtherActionName(settingValue)} the {GetCraneSettingFromRowNumber(rowNumber)} Master setting?",
            //                             "Confirm Change",
            //                             MessageBoxButtons.YesNo,
            //                             MessageBoxIcon.Question,
            //                             MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            //                     {
            //                         return;
            //                     }
            //                     _systemSettingsProxy.SetItemProperty(settingName, columnNumber, !settingValue, this);
            //                 }
            //                 else
            //                 {
            //                     if (_confirmEdits
            //                         && XMessageBox.Show(
            //                             this,
            //                            $"Do you want to {GetOtherActionName(settingValue)} the {GetCraneSettingFromRowNumber(rowNumber)} setting for Crane {columnNumber}?",
            //                             "Confirm Change",
            //                             MessageBoxButtons.YesNo,
            //                             MessageBoxIcon.Question,
            //                             MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            //                     {
            //                         return;
            //                     }
            //                     _systemSettingsProxy.SetItemProperty(settingName, columnNumber, !settingValue, this);
            //                 }
            //             }
            //             _Update();
        }

        private void _CraneFunctionGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (!_allowEdits)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                if (m_MouseCellPosition.Row == 0)
                {
                    string message;
                    int columnNumber = m_MouseCellPosition.Column;
                    CraneNumber craneNumber = (CraneNumber)columnNumber;
                    switch (craneNumber)
                    {
                        case CraneNumber.Crane1:
                        case CraneNumber.Crane2:
                        case CraneNumber.Crane3:
                        case CraneNumber.Crane4:
                            message = $"What would you like to do for all {craneNumber.ToText()} settings?";
                            break;
                        case CraneNumber.None:
                        default:
                            message = $"What would you like to do for all the Master settings?";
                            break;
                    }
                    bool newSettingValue;
                    using (BulkSystemSettingsEditForm form = new BulkSystemSettingsEditForm(message))
                    {
                        switch (form.ShowDialog(this))
                        {
                            case DialogResult.Yes:
                                newSettingValue = true;
                                break;
                            case DialogResult.No:
                                newSettingValue = false;
                                break;
                            case DialogResult.Cancel:
                            default:
                                return;
                        }
                    }
                    Parent.UseWaitCursor = true;
                    for (int rowNumber = UpperInboundsRowIndex;rowNumber < RowCount; rowNumber++)
                    {
                        (string settingName, bool settingValue) = _GetSettingInfo(rowNumber, columnNumber);
                        _systemSettingsProxy.SetItemProperty(settingName, columnNumber, newSettingValue, this);
                    }
                    Parent.UseWaitCursor = false;
                }
                else if (m_MouseCellPosition.Row > CraneModeRowIndex)
                {
                    int rowNumber = m_MouseCellPosition.Row;
                    int columnNumber = m_MouseCellPosition.Column;
                    SystemSettingsItem item = _systemSettingsProxy.GetItem();
                    Cell cell = (Cell)this[rowNumber, columnNumber];
//                     string cellValue = (string)cell.Value;

                    (string settingName, bool settingValue) = _GetSettingInfo(rowNumber, columnNumber);
                    if (columnNumber == 0)
                    {
                        if (_confirmEdits
                            && XMessageBox.Show(
                                this,
                                $"Do you want to {_GetOtherActionName(settingValue)} the {_GetCraneSettingFromRowNumber(rowNumber)} Master setting?",
                                "Confirm Change",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        {
                            return;
                        }
                        _systemSettingsProxy.SetItemProperty(settingName, columnNumber, !settingValue, this);
                    }
                    else
                    {
                        if (_confirmEdits
                            && XMessageBox.Show(
                                this,
                               $"Do you want to {_GetOtherActionName(settingValue)} the {_GetCraneSettingFromRowNumber(rowNumber)} setting for Crane {columnNumber}?",
                                "Confirm Change",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        {
                            return;
                        }
                        _systemSettingsProxy.SetItemProperty(settingName, columnNumber, !settingValue, this);
                    }
                }
            }
            _Update();
        }

        private void _ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //             contextMenu.Hide();
            //             if (Object.ReferenceEquals(e.ClickedItem, contextMenuReprint))
            //             {
            // //                 _ReprintShippingLabel(_clickLoadIndex);
            //             }
            //             else if (Object.ReferenceEquals(e.ClickedItem, contextMenuEditItem))
            //             {
            // //                 _EditItem(_clickLoadIndex);
            //             }
            //             else if (Object.ReferenceEquals(e.ClickedItem, contextMenuInsertEmpty))
            //             {
            // //                 _InsertEmpty(_clickLoadIndex);
            //             }
        }
    }
}
