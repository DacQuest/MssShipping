using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_StatusChange")]
    public class StatusChange
    {
        [Key]
        [Identity]
        public int ChangeID { get; set; }

        public string PalletID { get; set; }

        public string JobID { get; set; }

        public PalletStatus PalletStatus { get; set; }

        public int HoldCode { get; set; }

        public string Comment { get; set; }

//         [Column("EventDTTM")]
//         public DateTime? EventTimestamp { get; set; }
// 
//         [Column("ProdDate")]
//         public DateTime? ProductionTimestamp { get; set; }
// 
//         public string Shift { get; set; }
// 
//         public short? Period { get; set; }

    }
}
