using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mss.Common
{
    public static class Utils
    {

        public const string PalletIDValidatorName = "PalletIDValidator";
        public static bool ValidPalletID(this string palletID)
            => XValueValidator.Validate(PalletIDValidatorName, palletID);

//         public static bool ValidJobID(this string jobID)
//             => !jobID.IsNullOrWhiteSpace();

        public static bool ValidSku(this string sku) //??? USE XValueValidator?
            => !sku.IsNullOrWhiteSpace();

        public static int[] CreateIntArray(
            int first,
            int last,
            int increment)
        {
            return Enumerable
                .Range(0, ((last - first) / increment) + 1)
                .Select(i => first + (i * increment))
                .ToArray();
        }

        public static void FillDropDownListWithIntegers(
            ComboBox dropDown,
            int first,
            int last,
            int increment)
        {
            dropDown.Items.Clear();
            int[] integers = CreateIntArray(first, last, increment);
            dropDown.Items.AddRange(integers.Cast<object>().ToArray());
        }

        public static bool IsGreaterThan(this string strA, string strB, bool ignoreCase)
        {
            StringComparison comparison = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            return string.Compare(strA, strB, comparison) > 0;
        }
        public static bool IsGreaterThanOrEqualTo(this string strA, string strB, bool ignoreCase)
        {
            StringComparison comparison = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            return string.Compare(strA, strB, comparison) >= 0;
        }
        public static bool IsLessThan(this string strA, string strB, bool ignoreCase)
        {
            StringComparison comparison = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            return string.Compare(strA, strB, comparison) < 0;
        }
        public static bool IsLessThanOrEqualTo(this string strA, string strB, bool ignoreCase)
        {
            StringComparison comparison = ignoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            return string.Compare(strA, strB, comparison) <= 0;
        }
    }
}
