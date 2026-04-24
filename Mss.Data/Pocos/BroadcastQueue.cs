using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;

namespace Mss.Data.Pocos
{
    [System.ComponentModel.DataAnnotations.Schema.Table("SHIP_BroadcastQueue")]
    public class BroadcastQueue
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Identity]
        public int QueueID { get; set; }

        public int HeaderID { get; set; }

        public bool Processed { get; set; }

        [Column("ProcessedDTTM")]
        public DateTime? ProcessedOn { get; set; }

//         [Column("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [Column("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public short? Period { get; set; }

        // HeaderID in SHIP_BroadcastQueue -> HeaderID in SHIP_BroadcastHeader
        [LeftJoin("SHIP_BroadcastHeader", "HeaderID", "HeaderID")]
        public BroadcastHeader BroadcastHeader { get; set; }
    }
}
