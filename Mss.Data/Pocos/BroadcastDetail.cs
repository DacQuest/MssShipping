using Mss.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_BroadcastDtl")]
    public class BroadcastDetail
    {
        [Key]
        [Identity]
        public int DetailID { get; set; }

        public int HeaderID { get; set; }

        public VehicleRow VehicleRow { get; set; }

        [Column("PalletSKU")]
        public string Sku { get; set; }

        [Column("PickModeStatus")]
        public PickMode PickMode { get; set; }

        [Column("PickModeValue")]
        public string PickModeKey { get; set; }
    }
}
