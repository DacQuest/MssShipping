using System;
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
    [MicroOrmTable("SHIP_BroadcastQueue")]
    public class BroadcastQueue
    {
        [MicroOrmKey]
        [MicroOrmIdentity]
        public int QueueID { get; set; }

        public int HeaderID { get; set; }

        public bool Processed { get; set; }

        [MicroOrmColumn("ProcessedDTTM")]
        public DateTime? ProcessedOn { get; set; }

//         [MicroOrmColumn("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [MicroOrmColumn("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public short? Period { get; set; }

        // HeaderID in SHIP_BroadcastQueue -> HeaderID in SHIP_BroadcastHeader
        [MicroOrmLeftJoin("SHIP_BroadcastHeader", "HeaderID", "HeaderID")]
        public BroadcastHeader BroadcastHeader { get; set; }
    }
}
