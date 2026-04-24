using System;
using System.Collections.Generic;
using MicroOrmTable = System.ComponentModel.DataAnnotations.Schema.TableAttribute;
using MicroOrmKey = System.ComponentModel.DataAnnotations.KeyAttribute;
using MicroOrmIdentity = MicroOrm.Dapper.Repositories.Attributes.IdentityAttribute;
using MicroOrmColumn = System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;
using MicroOrmLeftJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.LeftJoinAttribute;
using MicroOrmInnerJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.InnerJoinAttribute;
using MicroOrmRightJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.RightJoinAttribute;
using MicroOrmCrossJoin = MicroOrm.Dapper.Repositories.Attributes.Joins.CrossJoinAttribute;
using MicroOrmNotMapped = System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute;

namespace Mss.Data.Pocos
{
    [MicroOrmTable("SHIP_LoadHeader")]
    public class LoadHeader
    {
        [MicroOrmKey]
        [MicroOrmIdentity]
        public int HeaderID { get; set; }

        [MicroOrmColumn("LoadNo")]
        public int LoadNumber { get; set; }

        public char Slug { get; set; }

        [MicroOrmColumn("StartedDTTM")]
        public DateTime StartedOn { get; set; }

        [MicroOrmColumn("FinishedDTTM")]
        public DateTime CompletedOn { get; set; }

        public int PalletCount { get; set; }

        [MicroOrmColumn("StartCSN")]
        public string FirstCsn { get; set; }

        [MicroOrmColumn("StopCSN")]
        public string LastCsn { get; set; }

        [MicroOrmColumn("TrailerNo")]
        public string TrailerID { get; set; }

        [MicroOrmLeftJoin("SHIP_LoadDetail", "HeaderID", "HeaderID")]
        public List<LoadDetail> LoadDetails { get; set; }
    }
}
