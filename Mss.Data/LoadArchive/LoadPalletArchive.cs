using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Collections;
using Mss.Common;

namespace Mss.Data.LoadArchive
{
    [Table("LoadPalletArchive")]
    public class LoadPalletArchive
    {
        [Key, Identity]
        public int ID { get; set; }
        public int LoadItemArchiveID { get; set; }
        public string PalletID { get; set; }
        public PalletStatus Status { get; set; }
        public int HoldCode { get; set; }
        public string Sku { get; set; }
        public string JobID { get; set; }
        public DateTime BuiltOn { get; set; }
        public string Comment { get; set; }

    }

}
