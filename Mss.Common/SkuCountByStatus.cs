using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public class SkuCountsByStatus
    {
        public string Sku { get; private set; }
        public int OKCount { get; set; } = 0;
        public int HoldCount { get; set; } = 0;
        public int PurgeCount { get; set; } = 0;
        public int QAPickCount { get; set; } = 0;
        public int UnknownCount { get; set; } = 0;

        public SkuCountsByStatus(string sku)
        {
            Sku = sku;
        }

    }
}
