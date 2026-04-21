using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Mss.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Data;
using DacQuest.DFX.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Mss.Data.Repositories
{
//     public interface IStatusChangeQueueRepository : IDapperRepository<StatusChangeQueue>
//     {
//         // ---- Synchronous ----
//         // Fetches StatusChangeQueue with its joined StatusChange populated.
//         StatusChangeQueue Find(Expression<Func<StatusChangeQueue, bool>> predicate);
//         IEnumerable<StatusChangeQueue> FindAll();
//         IEnumerable<StatusChangeQueue> FindAll(Expression<Func<StatusChangeQueue, bool>> predicate);
//         bool Insert(StatusChangeQueue instance);
//         bool Update(StatusChangeQueue instance);
//         bool Delete(StatusChangeQueue instance);
//         bool Delete(Expression<Func<StatusChangeQueue, bool>> predicate);
//         int Count();
//         int Count(Expression<Func<StatusChangeQueue, bool>> predicate);
//         bool BulkInsert(IEnumerable<StatusChangeQueue> instances);
// 
//         // ---- Asynchronous ----
//         Task<StatusChangeQueue> FindAsync(Expression<Func<StatusChangeQueue, bool>> predicate);
//         Task<IEnumerable<StatusChangeQueue>> FindAllAsync();
//         Task<IEnumerable<StatusChangeQueue>> FindAllAsync(Expression<Func<StatusChangeQueue, bool>> predicate);
//         Task<bool> InsertAsync(StatusChangeQueue instance);
//         Task<bool> UpdateAsync(StatusChangeQueue instance);
//         Task<bool> DeleteAsync(StatusChangeQueue instance);
//         Task<bool> DeleteAsync(Expression<Func<StatusChangeQueue, bool>> predicate);
//         Task<int> CountAsync();
//         Task<int> CountAsync(Expression<Func<StatusChangeQueue, bool>> predicate);
//         Task<bool> BulkInsertAsync(IEnumerable<StatusChangeQueue> instances);
//     }

    public class StatusChangeQueueRepository : DapperRepository<StatusChangeQueue> //, IStatusChangeQueueRepository
    {
        public StatusChangeQueueRepository(IDbConnection connection)
            : base(connection)
        {
        }

        public StatusChangeQueueRepository(IDbConnection connection, ISqlGenerator<StatusChangeQueue> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }

        public bool TryFetchStatusChangeRequests(out IEnumerable<StatusChangeQueue> pendingRequests)
        {
            try
            {
                pendingRequests = FindAll<StatusChange>(
                        q => q.Processed == false && q.Error == null,
                        q => q.StatusChange);
                if (!pendingRequests.Any())
                {
                    pendingRequests = null;
                    return false;
                }
                return true;
            }
            catch (Exception x)
            {
                pendingRequests = null;
                x.PublishSystemEvent(nameof(TryFetchStatusChangeRequests));
                return false;
            }
        }

        public bool TryUpdateProcessedStatusChangeRequests(
            IEnumerable<StatusChangeQueue> processedRequest)
        {
            try
            {
                return BulkUpdate(processedRequest);
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof(TryUpdateProcessedStatusChangeRequests));
                return false;
            }
        }





    }
}
