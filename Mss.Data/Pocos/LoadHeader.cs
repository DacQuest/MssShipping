using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_LoadHeader")]
    public class LoadHeader
    {
        [Key]
        [Identity]
        public int HeaderID { get; set; }
        public int LoadNo { get; set; }
        public string Slug { get; set; }
        public DateTime? StartedDTTM { get; set; }
        public DateTime? FinishedDTTM { get; set; }     // Nullable - may not have finished yet
        public int PalletCount { get; set; }
        public string StartCSN { get; set; }
        public string StopCSN { get; set; }
        public string TrailerID { get; set; }
    }
}
