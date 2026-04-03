using DacQuest.DFX.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Views
{
    [Serializable]
    public class PlcTagsViewParameterSetWrapper : XConfigurationParameterSet
    {
        private bool _allowTagWrites = false;
        private int _dataGridEditIndex = 0;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool AllowTagWrites => _allowTagWrites;

        [XConfigurationProperty(
            @"",
            false,
            DefaultValue = "0")]
        public int DataGridEditIndex => _dataGridEditIndex;
    }
}
