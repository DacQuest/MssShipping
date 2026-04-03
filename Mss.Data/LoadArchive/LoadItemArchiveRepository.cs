using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MicroOrm.Dapper.Repositories;

namespace Mss.Data.LoadArchive
{
    public class LoadItemArchiveRepository : DapperRepository<LoadItemArchive>
    {
        public LoadItemArchiveRepository(IDbConnection connection) : base(connection)
        {

        }
    }
}
