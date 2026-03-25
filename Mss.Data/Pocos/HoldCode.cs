using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_HoldCode")]
    public class HoldCode
    {
        [Column("DATA_ID")]
        public int ID { get; set; }

        [Column("HoldCode")]
        public int Code { get; set; }

        public string Description { get; set; }
    }
}
