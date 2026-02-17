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

        public static bool ValidJobID(this string jobID)
            => !jobID.IsNullOrWhiteSpace();

        public static bool ValidSku(this string sku)
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


    }
}
