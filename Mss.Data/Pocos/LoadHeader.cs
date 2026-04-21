using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Column("LoadNo")]
        public int LoadNumber { get; set; }

        public char Slug { get; set; }

        [Column("StartedDTTM")]
        public DateTime StartedOn { get; set; }

        [Column("FinishedDTTM")]
        public DateTime CompletedOn { get; set; }

        public int PalletCount { get; set; }

        [Column("StartCSN")]
        public string FirstCsn { get; set; }

        [Column("StopCSN")]
        public string LastCsn { get; set; }

        [Column("TrailerNo")]
        public string TrailerID { get; set; }

        // HeaderID in SHIP_LoadHeader -> HeaderID in SHIP_LoadDetail
        [LeftJoin("SHIP_LoadDetail", "HeaderID", "HeaderID")]
        public List<LoadDetail> LoadDetails { get; set; }
    }
}
