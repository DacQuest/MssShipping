using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Mss.Data.Pocos;

namespace Mss.Data.Repositories
{
//     public interface IBroadcastQueueRepository : IDapperRepository<BroadcastQueue>
//     {
//         // ---- Synchronous ----
//         // Fetches BroadcastQueue with its joined BroadcastHeader (and its Details) populated.
//         BroadcastQueue Find(Expression<Func<BroadcastQueue, bool>> predicate);
//         IEnumerable<BroadcastQueue> FindAll();
//         IEnumerable<BroadcastQueue> FindAll(Expression<Func<BroadcastQueue, bool>> predicate);
//         bool Insert(BroadcastQueue instance);
//         bool Update(BroadcastQueue instance);
//         bool Delete(BroadcastQueue instance);
//         bool Delete(Expression<Func<BroadcastQueue, bool>> predicate);
//         int Count();
//         int Count(Expression<Func<BroadcastQueue, bool>> predicate);
//         bool BulkInsert(IEnumerable<BroadcastQueue> instances);
// 
//         // ---- Asynchronous ----
//         Task<BroadcastQueue> FindAsync(Expression<Func<BroadcastQueue, bool>> predicate);
//         Task<IEnumerable<BroadcastQueue>> FindAllAsync();
//         Task<IEnumerable<BroadcastQueue>> FindAllAsync(Expression<Func<BroadcastQueue, bool>> predicate);
//         Task<bool> InsertAsync(BroadcastQueue instance);
//         Task<bool> UpdateAsync(BroadcastQueue instance);
//         Task<bool> DeleteAsync(BroadcastQueue instance);
//         Task<bool> DeleteAsync(Expression<Func<BroadcastQueue, bool>> predicate);
//         Task<int> CountAsync();
//         Task<int> CountAsync(Expression<Func<BroadcastQueue, bool>> predicate);
//         Task<bool> BulkInsertAsync(IEnumerable<BroadcastQueue> instances);
//     }

    public class BroadcastQueueRepository : DapperRepository<BroadcastQueue> //, IBroadcastQueueRepository
    {
        public BroadcastQueueRepository(IDbConnection connection)
            : base(connection)
        {
        }

        public BroadcastQueueRepository(IDbConnection connection, ISqlGenerator<BroadcastQueue> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }
    }
}
