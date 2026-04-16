using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_BroadcastQueue")]
    public class BroadcastQueue
    {
        [Key]
        [Identity]
        public int QueueID { get; set; }

        public int HeaderID { get; set; }

        public bool Processed { get; set; }

        [Column("ProcessedDTTM")]
        public DateTime? ProcessedOn { get; set; }

        [LeftJoin("SHIP_BroadcastHeader", "HeaderID", "HeaderID")]
        public BroadcastHeader BroadcastHeader { get; set; }
    }
}
