using Dapper.Contrib.Extensions;
using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data.Pocos
{
    [Dapper.Contrib.Extensions.Table("SHIP_HoldCode")]
    public class HoldCode
    {
        [Key]
        [Identity]
        public int ID { get; set; }

        [Column("HoldCode")]
        public int Code { get; set; }

        public string Description { get; set; }
    }
}
