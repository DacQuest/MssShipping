using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_BroadcastHeader")]
    public class BroadcastHeader
    {
        [Key]
        [Column("HDR_DATA_ID")]
        public int HeaderID { get; set; }

        [Column("RotationNo")]
        public int Rotation { get; set; }

        [Column("VehicleSKU")]
        public string VehicleSku { get; set; }

        [Column("VIN")]
        public string Vin { get; set; }

        [Column("PalletCount")]
        public int RowCount { get; set; }

        [LeftJoin("SHIP_BroadcastDtl", "HDR_DATA_ID", "HDR_DATA_ID")]
        public List<BroadcastDetail> BroadcastDetails { get; set; }
    }
}
