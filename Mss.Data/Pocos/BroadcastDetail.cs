using Mss.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_BroadcastDtl")]
    public class BroadcastDetail
    {
        [Key]
        [Column("DTL_DATA_ID")]
        public int DetailID { get; set; }

        [Column("HDR_DATA_ID")]
        public int HeaderID { get; set; }

        public VehicleRow VehicleRow { get; set; }

        [Column("PalletSKU")]
        public string Sku { get; set; }

        [Column("PickModeStatus")]
        public PickMode PickMode { get; set; }

        [Column("PickModeValue")]
        public string PickModeValue { get; set; }
    }
}
