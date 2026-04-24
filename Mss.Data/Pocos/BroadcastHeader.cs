using System.Collections.Generic;
using MicroOrmTable = System.ComponentModel.DataAnnotations.Schema.TableAttribute;
using MicroOrmKey = System.ComponentModel.DataAnnotations.KeyAttribute;
using MicroOrmIdentity = MicroOrm.Dapper.Repositories.Attributes.IdentityAttribute;
using MicroOrmColumn = System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;
using MicroOrmLeftJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.LeftJoinAttribute;
using MicroOrmInnerJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.InnerJoinAttribute;
using MicroOrmRightJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.RightJoinAttribute;
using MicroOrmCrossJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.CrossJoinAttribute;
using MicroOrmNotMapped = System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute;

namespace Mss.Data.Pocos
{
    [MicroOrmTable("SHIP_BroadcastHeader")]
    public class BroadcastHeader
    {
        [MicroOrmKey]
        [MicroOrmIdentity]
        public int HeaderID { get; set; }

        [MicroOrmColumn("RotationNo")]
        public int Rotation { get; set; }

        [MicroOrmColumn("VehicleSKU")]
        public string VehicleSku { get; set; }

        [MicroOrmColumn("VIN")]
        public string Vin { get; set; }

        [MicroOrmColumn("PalletCount")]
        public int DetailCount { get; set; }

//         [MicroOrmColumn("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [MicroOrmColumn("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public int? Period { get; set; }

        [MicroOrmLeftJoin("SHIP_BroadcastDetail", "HeaderID", "HeaderID")]
        public List<BroadcastDetail> BroadcastDetails { get; set; }

    }

}

