using Mss.Common;
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
    [MicroOrmTable("SHIP_BroadcastDetail")]
    public class BroadcastDetail
    {
        [MicroOrmKey]
        [MicroOrmIdentity]
        public int DetailID { get; set; }

        public int HeaderID { get; set; }

        public VehicleRow VehicleRow { get; set; }

        [MicroOrmColumn("PalletSKU")]
        public string Sku { get; set; }

        [MicroOrmColumn("PickModeStatus")]
        public PickMode PickMode { get; set; }

        [MicroOrmColumn("PickModeValue")]
        public string PickModeKey { get; set; }

//         [MicroOrmColumn("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [MicroOrmColumn("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public int? Period { get; set; }

    }
}
