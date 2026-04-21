using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_LoadDetail")]
    public class LoadDetail
    {
        [Key]
        [Identity]
        public int DetailID { get; set; }

        public int HeaderID { get; set; }

        [Column("VIN")]
        public string Vin { get; set; }

        public VehicleRow VehicleRow { get; set; }

        [Column("PalletSKU")]
        public string Sku { get; set; }

        public string JobID { get; set; }

        // HeaderID in SHIP_LoadDetails -> HeaderID in SHIP_LoadHeader
//         [LeftJoin("SHIP_LoadHeader", "HeaderID", "HeaderID")]
//         public LoadHeader Header { get; set; }
    }
}
