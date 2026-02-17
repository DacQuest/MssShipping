using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems.Proxy;
using DacQuest.DFX.DataItemEditors;
using SourceGrid;
using SourceGrid.Cells;
using SourceGrid.Cells.Models;
using Mss.Common;
using Mss.Collections;
using Mss.Views;

namespace Mss.Views
{
    public partial class SearchResultsGrid : Grid
    {

        //         private HoldCodesProxy _holdCodesProxy;

        public const int BinNumberColumnIndex = 1;
        public const int BinLocationColumnIndex = 2;
        //public const int CraneColumnIndex = 3;
        //public const int CraneSideColumnIndex = 4;
        //public const int HorizontalColumnIndex = 5;
        //public const int VerticalColumnIndex = 6;
        public const int BinStatusColumnIndex = 3;
        public const int VehicleRowColumnIndex = 4;
        public const int PalletIDColumnIndex = 5;
        public const int PalletStatusColumnIndex = 6;
        public const int SkuColumnIndex = 7;
        public const int KitSerialColumnIndex = 9;
        public const int KitCodeColumnIndex = 8;
        public const int BuiltOnColumnIndex = 10;
        public const int ColumnCount = 11;

        public const int BinNumberColumnWidth = 50;
        public const int RowColumnWidth = 32;
        //public const int SideColumnWidth = 32;
        //public const int HorizontalColumnWidth = 32;
        //public const int VerticalColumnWidth = 32;
        public const int BinStatusColumnWidth = 120;
        public const int VehicleRowColumnWidth = 50;
        public const int PalletIDColumnWidth = 180;
        public const int PalletStatusColumnWidth = 70;
        public const int KitSerialColumnWidth = 75;
        public const int SkuColumnWidth = 75;
        public const int BinLocationColumnWidth = 74;
        public const int KitCodeColumnWidth = 74;
        public const int BuiltOnColumnWidth = 150;

        private List<BinItem> _searchResults = new List<BinItem>();
        private ISearchResultsGridParent _parent = null;

        private int _headerColumnWidth = 34;
        private int _headerRowHeight = 22;
        private int _dataRowHeight = 22;
        private SourceGrid.Cells.Controllers.ToolTipText _toolTipController;

        public SearchResultsGrid()
        {
            InitializeComponent();
        }

        //        protected override void OnPaint(PaintEventArgs pe)
        //        {
        //            base.OnPaint(pe);
        //        }

        public void Initialize(ISearchResultsGridParent parent, bool allowBulkEditing)
        {
            _parent = parent;

//             XProxyCache.Acquire(Constant.HoldCodesName, out _holdCodesProxy);

            BorderStyle = BorderStyle.FixedSingle;

            _toolTipController = new SourceGrid.Cells.Controllers.ToolTipText
            {
                BackColor = Color.PaleGoldenrod,
                ForeColor = Color.Black,
                IsBalloon = true
            };

            SelectionMode = GridSelectionMode.Row;
            Selection.EnableMultiSelection = allowBulkEditing;
            Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(_Selection_SelectionChanged);
            MouseClick += _SearchResultsGrid_MouseClick;

            _SetUpGrid();
        }

        public BinItem LastClickedSearchResult { get; private set; } = null;

        public List<BinItem> SelectedSearchResults { get; private set; } = new List<BinItem>();

        private void _SetUpGrid()
        {
            RowsCount = 0;
            var headerView = new SourceGrid.Cells.Views.Cell
            {
                Font = new Font(Font, FontStyle.Bold),
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter,
                BackColor = Color.LightGray,
                ForeColor = Color.Black
            };

            headerView.Border = new DevAge.Drawing.RectangleBorder(
                new DevAge.Drawing.BorderLine(Color.Black, 0),
                new DevAge.Drawing.BorderLine(Color.DarkGray, 2),
                new DevAge.Drawing.BorderLine(Color.Black, 0),
                new DevAge.Drawing.BorderLine(Color.DarkGray, 1)
            );

            ColumnsCount = ColumnCount;
            FixedColumns = 1;

            Rows.Insert(0);
            Rows[0].Height = _headerRowHeight;

            ICell cell;
            for (int index = 0; index < ColumnsCount; index++)
            {
                Columns[index].AutoSizeMode = SourceGrid.AutoSizeMode.None;
                cell = new SourceGrid.Cells.ColumnHeader();
                //cell.View = boldHeader;
                cell.View = headerView;
                //cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                //cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                //this[0, index] = cell;


                switch (index)
                {
                    case 0:
                        Columns[0].Width = _headerColumnWidth;
                        break;
                    case BinNumberColumnIndex:
                        Columns[index].Width = BinNumberColumnWidth;
                        cell.Value = "Bin";
                        break;
                    case BinLocationColumnIndex:
                        Columns[index].Width = BinLocationColumnWidth;
                        cell.Value = "Location";
                        break;
                    //case CraneColumnIndex:
                    //    Columns[index].Width = RowColumnWidth;
                    //    cell.Value = "C";
                    //    break;
                    //case CraneSideColumnIndex:
                    //    Columns[index].Width = RowColumnWidth;
                    //    cell.Value = "S";
                    //    break;
                    //case HorizontalColumnIndex:
                    //    Columns[index].Width = HorizontalColumnWidth;
                    //    cell.Value = "H";
                    //    break;
                    //case VerticalColumnIndex:
                    //    Columns[index].Width = VerticalColumnWidth;
                    //    cell.Value = "V";
                    //    break;
                    case BinStatusColumnIndex:
                        Columns[index].Width = BinStatusColumnWidth;
                        cell.Value = "Bin Status";
                        break;
                    case VehicleRowColumnIndex:
                        Columns[index].Width = VehicleRowColumnWidth;
                        cell.Value = "Row";
                        break;
                    case PalletIDColumnIndex:
                        Columns[index].Width = PalletIDColumnWidth;
                        cell.Value = "Pallet ID";
                        break;
                    case PalletStatusColumnIndex:
                        Columns[index].Width = PalletStatusColumnWidth;
                        cell.Value = "Status";
                        break;
                    case KitSerialColumnIndex:
                        Columns[index].Width = KitSerialColumnWidth;
                        cell.Value = "Kit Serial";
                        break;
                    case SkuColumnIndex:
                        Columns[index].Width = SkuColumnWidth;
                        cell.Value = "SKU";
                        break;
                    case KitCodeColumnIndex:
                        Columns[index].Width = KitCodeColumnWidth;
                        cell.Value = "Kit Code";
                        break;
                    case BuiltOnColumnIndex:
                        Columns[index].Width = BuiltOnColumnWidth;
                        cell.Value = "Built On";
                        break;
                }
                cell.View = headerView;
                //cell.View = boldHeader;
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[0, index] = cell;
            }

            FixedRows = 1;
            int rowNumber = 0;
            foreach (BinItem searchResult in _searchResults)
            {
                Rows.Insert(++rowNumber);
                GridRow row = Rows[rowNumber];
                row.Height = _dataRowHeight;
                row.AutoSizeMode = SourceGrid.AutoSizeMode.None;

                cell = new Header(rowNumber.ToString())
                {
                    //View = boldHeader
                    View = headerView
                };
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.SortableHeader)));
                cell.Controller.RemoveController(cell.Controller.FindController(typeof(SourceGrid.Cells.Controllers.Resizable)));
                this[rowNumber, 0] = cell;

                PalletItem palletItem = searchResult.Pallet;

                for (int columnNumber = 1; columnNumber < ColumnsCount; columnNumber++)
                {
                    SourceGrid.Cells.Views.Cell cellView = new SourceGrid.Cells.Views.Cell
                    {
                        TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
                    };
                    cell = new Cell
                    {
                        View = cellView
                    };

                    switch (columnNumber)
                    {
                        case BinNumberColumnIndex:
                            cell.Value = searchResult.BinNumber.ToString();
                            break;
                        case BinLocationColumnIndex:
                            cell.Value = searchResult.LocationText;
                            break;
                        //case CraneColumnIndex:
                        //    cell.Value = (int)searchResult.CraneNumber;
                        //    break;
                        //case CraneSideColumnIndex:
                        //    cell.Value = searchResult.BinSideNumber;
                        //    break;
                        //case HorizontalColumnIndex:
                        //    cell.Value = searchResult.BinHorizontalNumber.ToString("00");
                        //    break;
                        //case VerticalColumnIndex:
                        //    cell.Value = searchResult.BinVerticalNumber.ToString();
                        //    break;
                        case BinStatusColumnIndex:
                            if (searchResult.BinStatus == BinStatus.Invalid)
                            {
                                cell.Value = string.Empty;
                            }
                            else
                            {
                                cell.Value = searchResult.BinStatus.ToText();
                            }
                            //if (searchResult.BinStatus > BinStatus.Pickable)
                            //{
                            //    cell.ToolTipText = searchResult.BinStatus.ToText();
                            //    cell.AddController(_toolTipController);
                            //}
                            //else
                            //{
                            //    cell.ToolTipText = string.Empty;
                            //    cell.RemoveController(_toolTipController);
                            //}
                            break;
                        case PalletIDColumnIndex:
                            cell.Value = palletItem.PalletID == Constant.NoPalletID
                                ? string.Empty
                                : palletItem.PalletID;
                            break;
                        case VehicleRowColumnIndex:
                            cell.Value = palletItem.Sku == Constant.StackSku
                                ? string.Empty
                                : palletItem.VehicleRow.ToString().Left(1);
                            break;
                        case PalletStatusColumnIndex:
                            if ((palletItem.Status == PalletStatus.Unknown && searchResult.BinStatus == BinStatus.Empty)
                                || palletItem.Status == PalletStatus.Invalid)
                            {
                                cell.Value = string.Empty;
                            }
                            else
                            {
                                cell.Value = palletItem.Status.ToText();
                            }
                            break;
                        //                         case HoldCodeColumnIndex:
                        //                             int palletHoldCode = palletItem.HoldCode;
                        //                             if (palletHoldCode == Constant.NoHoldCode)
                        //                             {
                        //                                 searchResult.SearchResultsHoldCodeText = string.Empty;
                        //                             }
                        //                             else if (_holdCodesProxy.ContainsKey(palletHoldCode))
                        //                             {
                        //                                 searchResult.SearchResultsHoldCodeText = _holdCodesProxy[palletHoldCode].Description;
                        //                             }
                        //                             else
                        //                             {
                        //                                 searchResult.SearchResultsHoldCodeText = palletHoldCode.ToString();
                        //                             }
                        //                             cell.Value = searchResult.SearchResultsHoldCodeText;
                        //                             break;
                        case SkuColumnIndex:
                            cell.Value = palletItem.Sku;
                            break;
                        //                         case JobIDColumnIndex:
                        //                             if (palletItem.JobID == Constant.NoJobID)
                        //                             {
                        //                                 cell.Value = string.Empty;
                        //                             }
                        //                             else
                        //                             {
                        //                                 cell.Value = palletItem.JobIDText;
                        //                             }
                        //                             break;
                        case BuiltOnColumnIndex:
                            if (palletItem.BuiltOn > Constant.BeginningOfTime)
                            {
                                cell.Value = palletItem.BuiltOn.ToString("G");
                            }
                            else
                            {
                                cell.Value = string.Empty;
                            }
                            break;
                    }
                    this[rowNumber, columnNumber] = cell;
                }
            }
            if (_searchResults.Count > 0)
            {
                Selection.SelectRow(1, true);
                SelectedSearchResults = new List<BinItem>
                {
                    _searchResults[0]
                };
            }
        }

        public void SetSearchResults(List<BinItem> searchResults)
        {
            _searchResults = searchResults;
            SelectedSearchResults = null;
            _SetUpGrid();
        }

        private void _SearchResultsGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MouseCellPosition.Row >= 1)
            {
                _GoToBin(MouseCellPosition.Row - 1);
            }
        }

        private void _GoToBin(int searchResultIndex)
        {
            if (_parent != null)
            {
                BinItem selected = _searchResults[searchResultIndex];

                SelectedSearchResults = new List<BinItem>() { selected };

                _parent.GoToBinIndex(selected.NodeIndex);
            }
        }

        private void _SearchResultsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseCellPosition.Row >= 1)
            {
                LastClickedSearchResult = _searchResults[MouseCellPosition.Row - 1];
            }
        }

        private void _Selection_SelectionChanged(object sender, RangeRegionChangedEventArgs e)
        {
            SelectedSearchResults = new List<BinItem>();
            RangeRegion region = Selection.GetSelectionRegion();
            int[] rowIndexes = region.GetRowsIndex();
            foreach (int rowIndex in rowIndexes)
            {
                if (rowIndex > 0)
                {
                    SelectedSearchResults.Add(_searchResults[rowIndex - 1]);
                }
            }
        }
    }
}
