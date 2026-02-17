using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Mss.Data
{
    public class Search
    {
        private SearchType _type = SearchType.None;
        public SearchType Type
        {
            get => _type;
            set
            {
                _type = value;
                BinItems = null;
                CraneNumber = CraneNumber.None;
                Sku = null;
                Skus = null;
                PalletID = Constant.NoPalletID;
                SeatID = string.Empty;
                PalletStatuses = PalletStatus.Invalid;
                BinStatuses = BinStatus.Invalid;
                VehicleRows = null;
                Start = Constant.BeginningOfTime;
                End = Constant.BeginningOfTime;
                //                 StartJobID = Constant.NoJobID;
                //                 EndJobID = Constant.NoJobID;
            }
        }

        public List<BinItem> BinItems { get; set; }
        public CraneNumber CraneNumber { get; set; }
        public string Sku { get; set; }
        public List<string> Skus { get; set; }
        public string PalletID { get; set; }
        public string SeatID { get; set; }
        public PalletStatus PalletStatuses { get; set; }
        public BinStatus BinStatuses { get; set; }
        public List<VehicleRow> VehicleRows { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public List<BinItem> PerformSearch()
        {
            List<BinItem> searchResults = new List<BinItem>();
            switch (Type)
            {
                case SearchType.ByCrane:
                    return Searches.ByCrane(BinItems, CraneNumber);
                case SearchType.BySku:
                    return Searches.BySku(BinItems, Sku);
                case SearchType.BySkus:
                    return Searches.BySkus(BinItems, Skus);
                case SearchType.ByPalletID:
                    return Searches.ByPalletID(BinItems, PalletID);
                case SearchType.Disabled:
                    return Searches.Disabled(BinItems);
                case SearchType.Audits:
                    return Searches.Audits(BinItems);
                case SearchType.PickOnly:
                    return Searches.PickOnly(BinItems);
                case SearchType.DuplicatePallets:
                    return Searches.DuplicatePallets(BinItems);
                case SearchType.ByPalletStatus:
                    return Searches.ByPalletStatus(BinItems, PalletStatuses);
                case SearchType.ByBinStatus:
                    return Searches.ByBinStatus(BinItems, BinStatuses);
                case SearchType.ByVehicleRow:
                    return Searches.ByVehicleRow(BinItems, VehicleRows);
                case SearchType.ByBuiltOn:
                    return Searches.ByBuiltOn(BinItems, Start, End);
                case SearchType.AdvancedSearch:
                    break;
                    //return PerformAdvancedSearch(BinItems);
            }
            return searchResults;
        }

        public static List<BinItem> PerformAdvancedSearch(
            List<BinItem> binItems,
            bool chkEmpty,
            bool chkApplyCrane,
            bool chkApplyVehicleRow,
            bool chkApplySkus,
            bool chkApplyPalletStatus,
            bool chkApplyBinAttributes,
            bool chkApplyBinStatus,
            bool chkApplyDateTime,
            List<string> SearchSkus,
            PalletStatus SearchPalletStatuses,
            BinStatus SearchBinStatuses,
            int SearchCrane1,
            int SearchCrane2,
            int SearchCrane3,
            int SearchCrane4,
            int SearchCrane5,
            int SearchCranes,
            int SearchFront,
            int SearchMid,
            int SearchRear,
            int SearchVehicleRows,
            bool Audit,
            bool PickOnly,
            bool Disabled,
            DateTime SearchStartDateTime,
            DateTime SearchEndDateTime,
            string OrderBy,
            string ThenBy1,
            string ThenBy2,
            bool OrderByDescending,
            bool ThenBy1Descending,
            bool ThenBy2Descending)
        {
            bool atLeastOneConditionApplied = false;
            List<BinItem> searchResults = binItems
                .Where(b =>
                {
                    if (chkApplyBinStatus && chkEmpty)
                    {
                        return b.BinStatus != BinStatus.Invalid;
                    }
                    else
                    {
                        return b.BinStatus != BinStatus.Empty
                        && b.BinStatus != BinStatus.Invalid;
                    }
                })
                .ToList();
            if (chkApplyCrane && SearchCranes > 0)
            {
                List<BinItem> cranesearchResults = new List<BinItem>();
                if ((SearchCranes & SearchCrane1) == SearchCrane1)
                {
                    cranesearchResults.AddRange(Searches.ByCrane(searchResults, CraneNumber.Crane1));
                }
                if ((SearchCranes & SearchCrane2) == SearchCrane2)
                {
                    cranesearchResults.AddRange(Searches.ByCrane(searchResults, CraneNumber.Crane2));
                }
                if ((SearchCranes & SearchCrane3) == SearchCrane3)
                {
                    cranesearchResults.AddRange(Searches.ByCrane(searchResults, CraneNumber.Crane3));
                }
                if ((SearchCranes & SearchCrane4) == SearchCrane4)
                {
                    cranesearchResults.AddRange(Searches.ByCrane(searchResults, CraneNumber.Crane4));
                }

                searchResults = cranesearchResults;
                atLeastOneConditionApplied = true;
            }
            if (chkApplyVehicleRow && SearchVehicleRows > 0)
            {
                List<BinItem> rowSearchResults = new List<BinItem>();
                if ((SearchVehicleRows & SearchFront) == SearchFront)
                {
                    rowSearchResults.AddRange(Searches.ByVehicleRow(searchResults, new List<VehicleRow>() { VehicleRow.Row1 }));
                }
                if ((SearchVehicleRows & SearchMid) == SearchMid)
                {
                    rowSearchResults.AddRange(Searches.ByVehicleRow(searchResults, new List<VehicleRow>() { VehicleRow.Row2 }));
                }
                searchResults = rowSearchResults;
                atLeastOneConditionApplied = true;
            }
            if (chkApplySkus
                && SearchSkus != null
                && SearchSkus.Count() > 0)
            {
                searchResults = Searches.BySkus(searchResults, SearchSkus);
                atLeastOneConditionApplied = true;
            }
            if (chkApplyPalletStatus
                && SearchPalletStatuses != PalletStatus.Invalid)
            {
                searchResults = Searches.ByPalletStatus(searchResults, SearchPalletStatuses);
                atLeastOneConditionApplied = true;
            }
            if (chkApplyBinAttributes)
            {
                if (Audit)
                {
                    searchResults = Searches.Audits(searchResults);
                    atLeastOneConditionApplied = true;
                }
                if (PickOnly)
                {
                    searchResults = Searches.PickOnly(searchResults);
                    atLeastOneConditionApplied = true;
                }
                if (Disabled)
                {
                    searchResults = Searches.Disabled(searchResults);
                    atLeastOneConditionApplied = true;
                }
            }
            if (chkApplyBinStatus
                && SearchBinStatuses != BinStatus.Invalid)
            {
                searchResults = Searches.ByBinStatus(searchResults, SearchBinStatuses);
                atLeastOneConditionApplied = true;
            }
            if (chkApplyDateTime
                && SearchEndDateTime > SearchStartDateTime)
            {
                searchResults = Searches.ByBuiltOn(searchResults, SearchStartDateTime, SearchEndDateTime);
                atLeastOneConditionApplied = true;
            }
            if (!atLeastOneConditionApplied)
            {
                searchResults.Clear();
            }
            searchResults = _ShowSearchResults(
                searchResults,
                OrderBy,
                ThenBy1,
                ThenBy2,
                OrderByDescending,
                ThenBy1Descending,
                ThenBy2Descending);
            return searchResults;
        }

        //public string OrderBy { get; set; } = string.Empty;
        //public string ThenBy1 { get; set; } = null;
        //public string ThenBy2 { get; set; } = null;

        private static Func<BinItem, object> _GetSortingFunction(string orderbyType)
        {
            switch (orderbyType)
            {
                case "Bin Number":
                    return (r) => r.NodeIndex + 1;
                case "Crane":
                    return (r) => r.CraneNumber;
                case "Side":
                    return (r) => r.BinSideNumber;
                case "Horizontal":
                    return (r) => r.BinHorizontalNumber;
                case "Vertical":
                    return (r) => r.BinVerticalNumber;
                case "Pallet ID":
                    return (r) => r.Pallet.PalletID;
                case "Pallet Status":
                    return (r) => r.Pallet.Status;
                case "SKU":
                    return (r) => r.Pallet.Sku;
                case "Built On":
                    return (r) => r.Pallet.BuiltOn;
                case "Bin Status":
                    return (r) => r.BinStatus;
                case "Vehicle Row":
                    return (r) => r.Pallet.VehicleRow;
            }
            return null;
        }

        private static ThenByDelegate getThenByFunction(bool orderByDescending)
        {
            if (orderByDescending)
            {
                return ThenByDescendingDelegateFunction;
            }
            else
            {
                return ThenByAscendingDelegateFunction;
            }
        }


        private static OrderByDelegate getOrderingFunction(bool orderByDescending)
        {
            if (orderByDescending)
            {
                return OrderByDescendingDelegateFunction;
            }
            else
            {
                return OrderByAscendingDelegateFunction;
            }
        }

        private delegate IOrderedEnumerable<BinItem> OrderByDelegate(
            IEnumerable<BinItem> collection,
            Func<BinItem, object> sortingFunction);
        private delegate IOrderedEnumerable<BinItem> ThenByDelegate(
            IOrderedEnumerable<BinItem> collection,
            Func<BinItem, object> sortingFunction);

        private static IOrderedEnumerable<BinItem> OrderByAscendingDelegateFunction(
            IEnumerable<BinItem> collection,
            Func<BinItem, object> sortingFunction)
        {
            return collection.OrderBy(sortingFunction);
        }
        private static IOrderedEnumerable<BinItem> ThenByAscendingDelegateFunction(
            IOrderedEnumerable<BinItem> collection,
            Func<BinItem, object> sortingFunction)
        {
            return collection.ThenBy(sortingFunction);
        }
        private static IOrderedEnumerable<BinItem> OrderByDescendingDelegateFunction(
            IEnumerable<BinItem> collection,
            Func<BinItem, object> sortingFunction)
        {
            return collection.OrderByDescending(sortingFunction);
        }
        private static IOrderedEnumerable<BinItem> ThenByDescendingDelegateFunction(
            IOrderedEnumerable<BinItem> collection,
            Func<BinItem, object> sortingFunction)
        {
            return collection.ThenByDescending(sortingFunction);
        }

        private static List<BinItem> _ShowSearchResults(
            List<BinItem> searchResults,
            string OrderBy,
            string ThenBy1,
            string ThenBy2,
            bool OrderByDescending,
            bool ThenBy1Descending,
            bool ThenBy2Descending)
        {
            if (searchResults.Count > 0)
            {
                Func<BinItem, object> funcOrderBy = (r) => r.NodeIndex;
                Func<BinItem, object> funcThenBy1 = null;
                Func<BinItem, object> funcThenBy2 = null;

                funcOrderBy = _GetSortingFunction(OrderBy);
                funcThenBy1 = _GetSortingFunction(ThenBy1);
                funcThenBy2 = _GetSortingFunction(ThenBy2);

                IOrderedEnumerable<BinItem> tempSearchResults;
                Func<BinItem, object> defaultSortingFunction = new Func<BinItem, object>((r) => r.NodeIndex + 1);
                tempSearchResults = getOrderingFunction(OrderByDescending)(searchResults, funcOrderBy ?? defaultSortingFunction);
                tempSearchResults = getThenByFunction(ThenBy1Descending)(tempSearchResults, funcThenBy1 ?? defaultSortingFunction);
                tempSearchResults = getThenByFunction(ThenBy2Descending)(tempSearchResults, funcThenBy2 ?? defaultSortingFunction);
                searchResults = tempSearchResults.ToList();
            }
            return searchResults;
        }
    }
}
