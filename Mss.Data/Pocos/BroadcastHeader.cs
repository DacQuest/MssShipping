using MicroOrm.Dapper.Repositories.Attributes;

using MicroOrm.Dapper.Repositories.Attributes.Joins;

using System.Collections.Generic;

// using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;

namespace Mss.Data.Pocos

{

    [System.ComponentModel.DataAnnotations.Schema.Table("SHIP_BroadcastHeader")]

    public class BroadcastHeader

    {

        [Key]

        [Identity]

        public int HeaderID { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("RotationNo")]

        public int Rotation { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("VehicleSKU")]

        public string VehicleSku { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("VIN")]

        public string Vin { get; set; }

        //[System.ComponentModel.DataAnnotations.Schema.Column("PalletCount")]

        public int PalletCount { get; set; }

        //         [Column("EventDTTM")]

        //         public DateTime? EventTimestamp { get; set; }

        // 

        //         [Column("ProdDate")]

        //         public DateTime? ProductionTimestamp { get; set; }

        // 

        //         public string Shift { get; set; }

        // 

        //         public int? Period { get; set; }

        [LeftJoin("SHIP_BroadcastDetail", "HeaderID", "HeaderID")]

        public List<BroadcastDetail> BroadcastDetails { get; set; }

    }

}

