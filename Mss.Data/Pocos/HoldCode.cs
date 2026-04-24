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
    [MicroOrmTable("SHIP_HoldCode")]
    public class HoldCode
    {
        [MicroOrmKey]
        [MicroOrmIdentity]
        public int ID { get; set; }

        [MicroOrmColumn("HoldCode")]
        public int Code { get; set; }

        public string Description { get; set; }
    }
}
