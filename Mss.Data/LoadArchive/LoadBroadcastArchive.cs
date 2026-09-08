using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Common;

namespace Mss.Data.LoadArchive
{
    [Table("LoadBroadcastArchive")]
    public class LoadBroadcastArchive
    {
        [Key, Identity]
        public int ID { get; set; }
        public int LoadItemArchiveID { get; set; }
        public BroadcastStatus Status { get; set; }
        public int Rotation { get; set; }
        public string Csn { get; set; }
        public string Vin { get; set; }
        public string VehicleSku { get; set; }
        public string Sku { get; set; }
        public PickMode PickMode { get; set; }
        public string PickModeKey { get; set; }
        public DateTime ReceivedOn { get; set; }
        public int VehicleRowCount { get; set; }

    }
}
