using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;
using System;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_BroadcastHeader")]
    public class BroadcastHeader
    {
        [Key]
        [Identity]
        public int HeaderID { get; set; }

        [Column("RotationNo")]
        public int Rotation { get; set; }

        [Column("VehicleSKU")]
        public string VehicleSku { get; set; }

        [Column("VIN")]
        public string Vin { get; set; }

        [Column("PalletCount")]
        public int RowCount { get; set; }

//         [Column("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [Column("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public int? Period { get; set; }

        [LeftJoin("SHIP_BroadcastDetail", "HeaderID", "HeaderID")]
        public List<BroadcastDetail> BroadcastDetails { get; set; }
    }
}
