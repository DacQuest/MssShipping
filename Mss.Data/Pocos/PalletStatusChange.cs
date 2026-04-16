using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_StatusChange")]
    public class PalletStatusChange
    {
        [Key]
        [Column("ChangeID")]
        public int ChangeID { get; set; }

        [Column("PalletID")]
        public string PalletID { get; set; }

        [Column("JobID")]
        public string JobID { get; set; }

        [Column("PalletStatus")]
        public PalletStatus PalletStatus { get; set; }

        [Column("HoldCode")]
        public int HoldCode { get; set; }

        [Column("Comment")]
        public string Comment { get; set; }







        [Column("RotationNo")]
        public int Rotation { get; set; }

        [Column("VIN")]
        public string Vin { get; set; }

        [Column("PalletCount")]
        public int RowCount { get; set; }

        [LeftJoin("SHIP_BroadcastDtl", "HDR_DATA_ID", "HDR_DATA_ID")]
        public List<BroadcastDetail> BroadcastDetails { get; set; }
    }
}
