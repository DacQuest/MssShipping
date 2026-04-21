using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_StatusChangeQueue")]
    public class StatusChangeQueue
    {
        [Key]
        [Identity]
        public int QueueID { get; set; }

        public int ChangeID { get; set; }

        public bool Processed { get; set; }

        [Column("ProcessedDTTM")]
        public DateTime? ProcessedOn { get; set; }

        public string Error { get; set; }

//         [Column("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [Column("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public short? Period { get; set; }

        // ChangeID in SHIP_StatusChangeQueue -> ChangeID in SHIP_StatusChange
        [LeftJoin("SHIP_StatusChange", "ChangeID", "ChangeID")]
        public StatusChange StatusChange { get; set; }
    }
}
