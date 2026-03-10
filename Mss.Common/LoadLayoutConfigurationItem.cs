using DacQuest.DFX.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Serializable]
    public class LoadLayoutConfigurationItem : XConfigurationItem
    {
        [XConfigurationProperty(
            @"",
            true)]
        public XConfigurationValueList LayoutOrder { get; private set; }

        public int[] LoadLayoutOrder { get; private set; }

        public override bool ValidateConfigurationItem(
            string sectionName,
            ref XConfigurationException exception)
        {
            HashSet<int> usedValues = new HashSet<int>();
            foreach (string layoutOrder in LayoutOrder)
            {
                if (!int.TryParse(layoutOrder, out int layoutOrderInt))
                {
                    //!!! Append to exception!
                    continue;
                }
                if (layoutOrderInt < 0 || layoutOrderInt > 59)
                {
                    //!!! Append to exception!
                    continue;
                }
                if (usedValues.Contains(layoutOrderInt))
                {
                    //!!! Append to exception! Duplicate Value
                    continue;
                }
                _ = usedValues.Add(layoutOrderInt);
            }
            if (usedValues.Count != Constant.LoadSize)
            {
                //!!! Append to exception! Duplicate Value.
            }
            else
            {
                LoadLayoutOrder = LayoutOrder
                    .Select(l => int.Parse(l))
                    .ToArray();
            }
            return true;
        }

    }
}
