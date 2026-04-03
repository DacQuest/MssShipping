using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MicroOrm.Dapper.Repositories;

namespace Mss.Data.LoadArchive
{
    public class LoadArchiveRepository : DapperRepository<LoadArchive>
    {
        public LoadArchiveRepository(IDbConnection connection) : base(connection)
        {

        }
    }
}
