using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;
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
    [MicroOrmTable("SHIP_LoadDetail")]
    public class LoadDetail
    {
        [MicroOrmKey]
        [MicroOrmIdentity]
        public int DetailID { get; set; }

        public int HeaderID { get; set; }

        [MicroOrmColumn("VIN")]
        public string Vin { get; set; }

        public VehicleRow VehicleRow { get; set; }

        [MicroOrmColumn("PalletSKU")]
        public string Sku { get; set; }

        public string JobID { get; set; }

    }
}
