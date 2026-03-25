using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_BroadcastQueue")]
    public class BroadcastQueue
    {
        [Key]
        [Column("KeyID")]
        public int ID { get; set; }

        [Column("HDR_DATA_ID")]
        public int HeaderID { get; set; }

        public bool Processed { get; set; }
        [Column("ProcessedDTTM")]
        public DateTime ProcessedOn { get; set; }

        [LeftJoin("SHIP_BroadcastHdr", "HDR_DATA_ID", "HDR_DATA_ID")]
        public BroadcastHeader BroadcastHeader { get; set; }
    }
}
