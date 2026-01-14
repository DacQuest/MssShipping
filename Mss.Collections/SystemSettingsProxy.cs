using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Collections
{
    public class SystemSettingsProxy : XSharedArrayProxy<SystemSettingsItem>
    {
        public List<SystemSettingsItem> Items => ProxyList.ToList();
    }
}
