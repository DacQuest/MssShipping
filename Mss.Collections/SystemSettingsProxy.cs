using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Proxy;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Collections
{
    public class SystemSettingsProxy : XSharedSingleItemProxy<SystemSettingsItem>
    {
//         public List<SystemSettingsItem> Items => ProxyList.ToList();
        public bool SlugAEnabled => GetItem().SlugAEnabled;
        public bool SlugBEnabled => GetItem().SlugBEnabled;
        public SlugLetter PreferredSlug => GetItem().PreferredSlug;
    }
}
