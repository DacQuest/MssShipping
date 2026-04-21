using DacQuest.DFX.Core;
using Dapper;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Mss.Common;
using Mss.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mss.Data.Repositories
{
    //     public interface IBroadcastHeaderRepository : IDapperRepository<BroadcastHeader>
    //     {
    //         // ---- Synchronous ----
    //         // Fetches BroadcastHeader with its joined BroadcastDetail collection populated.
    //         BroadcastHeader Find(Expression<Func<BroadcastHeader, bool>> predicate);
    //         IEnumerable<BroadcastHeader> FindAll();
    //         IEnumerable<BroadcastHeader> FindAll(Expression<Func<BroadcastHeader, bool>> predicate);
    //         bool Insert(BroadcastHeader instance);
    //         bool Update(BroadcastHeader instance);
    //         bool Delete(BroadcastHeader instance);
    //         bool Delete(Expression<Func<BroadcastHeader, bool>> predicate);
    //         int Count();
    //         int Count(Expression<Func<BroadcastHeader, bool>> predicate);
    //         bool BulkInsert(IEnumerable<BroadcastHeader> instances);
    // 
    //         // ---- Asynchronous ----
    //         Task<BroadcastHeader> FindAsync(Expression<Func<BroadcastHeader, bool>> predicate);
    //         Task<IEnumerable<BroadcastHeader>> FindAllAsync();
    //         Task<IEnumerable<BroadcastHeader>> FindAllAsync(Expression<Func<BroadcastHeader, bool>> predicate);
    //         Task<bool> InsertAsync(BroadcastHeader instance);
    //         Task<bool> UpdateAsync(BroadcastHeader instance);
    //         Task<bool> DeleteAsync(BroadcastHeader instance);
    //         Task<bool> DeleteAsync(Expression<Func<BroadcastHeader, bool>> predicate);
    //         Task<int> CountAsync();
    //         Task<int> CountAsync(Expression<Func<BroadcastHeader, bool>> predicate);
    //         Task<bool> BulkInsertAsync(IEnumerable<BroadcastHeader> instances);
    //     }

    public class BroadcastHeaderRepository : DapperRepository<BroadcastHeader> //, IBroadcastHeaderRepository
    {
        public BroadcastHeaderRepository(IDbConnection connection)
            : base(connection)
        {
        }

        public BroadcastHeaderRepository(IDbConnection connection, ISqlGenerator<BroadcastHeader> sqlGenerator)
            : base(connection, sqlGenerator)
        {
        }

//         public bool TryFetchRawBroadcasts(out IEnumerable<BroadcastQueue> rawBroadcasts)
//         {
//             try
//             {
//                 // Step 1: Get BroadcastQueue records with BroadcastHeader populated
//                 rawBroadcasts = FindAllAsync(q => !q.Processed)
//                     .GetAwaiter()
//                     .GetResult()
//                     .OrderBy(q => q.HeaderID);
// 
//                 if (!rawBroadcasts.Any())
//                 {
//                     rawBroadcasts = null;
//                     return false;
//                 }
// 
//                 // Step 2: Separately load BroadcastDetails for each header
//                 DapperRepository<BroadcastDetail> detailRepo = new DapperRepository<BroadcastDetail>(Connection);
// 
//                 IEnumerable<int> headerIDs = rawBroadcasts
//                     .Where(r => r.BroadcastHeader != null)
//                     .OrderBy(r => r.BroadcastHeader.Rotation)
//                     .Select(r => r.BroadcastHeader.HeaderID)
//                     .Distinct();
// 
//                 IEnumerable<BroadcastDetail> details = detailRepo.FindAllAsync(d => headerIDs.Contains(d.HeaderID))
//                     .GetAwaiter()
//                     .GetResult();
// 
//                 // Step 3: Wire up BroadcastDetails onto each BroadcastHeader
//                 foreach (BroadcastQueue rawBroadcast in rawBroadcasts.Where(r => r.BroadcastHeader != null))
//                 {
//                     rawBroadcast.BroadcastHeader.BroadcastDetails = details
//                         .Where(d => d.HeaderID == rawBroadcast.BroadcastHeader.HeaderID)
//                         .OrderBy(d => d.VehicleRow)
//                         .ToList();
//                 }
// 
//                 return true;
//             }
//             catch (Exception x)
//             {
//                 rawBroadcasts = null;
//                 x.PublishSystemEvent(nameof(TryFetchRawBroadcasts));
//                 return false;
//             }
//         }

//         public bool TryMarkAsProcessed(BroadcastQueue rawBroadcast)
//         {
//             string sql = @"UPDATE [SHIP_BroadcastQueue] 
//                 SET [Processed] = 1, [ProcessedDTTM] = @ProcessedDTTM 
//                 WHERE [KeyID] = @KeyID";
// 
//             DynamicParameters parameters = new DynamicParameters();
//             parameters.Add("@KeyID", rawBroadcast.QueueID);
//             parameters.Add("@ProcessedDTTM", DateTime.Now.ToString(Constant.LongDateTimeFormat24));
// 
//             int rowsAffected = Connection.Execute(sql, parameters);
//             return rowsAffected > 0;
//         }

        //         public bool FetchRawBroadcast(out List<BroadcastQueue> broadcastQueue)
        //         {
        //             try
        //             {
        //                 broadcastQueue = FindAll(q => !q.Processed)
        //                     .OrderBy(q => q.HeaderID)
        //                     .ToList();
        //                 BroadcastHeaderRepository headerRepository = new BroadcastHeaderRepository(Connection);
        //                 if (!headerRepository.FetchBroadcastHeaders(broadcastQueue, out List<BroadcastHeader> broadcastHeaders))
        //                 {
        //                     broadcastQueue = null;
        //                     return false;
        //                 }
        //                 BroadcastDetailsRepository detailsRepository = new BroadcastDetailsRepository(Connection);
        //                 if (!detailsRepository.FetchBroadcastDetails(broadcastQueue, out List<BroadcastDetail> broadcastDetails))
        //                 {
        //                     broadcastQueue = null;
        //                     return false;
        //                 }
        //                 foreach (BroadcastQueue queue in broadcastQueue)
        //                 {
        //                     BroadcastHeader header = broadcastHeaders.SingleOrDefault(h => h.HeaderID == queue.HeaderID);
        //                     if (header != null)
        //                     {
        //                         queue.BroadcastHeader = header;
        //                         List<BroadcastDetail> details = broadcastDetails.Where(d => d.HeaderID == queue.HeaderID).ToList();
        //                         if (details.Any())
        //                         {
        //                             header.BroadcastDetails = details;
        //                         }
        //                     }
        //                 }
        //                 return true;
        //             }
        //             catch (SqlException x)
        //             {
        //                 x.PublishSystemEvent("BroadcastRepository.FetchRawBroadcast()");
        //                 broadcastQueue = null;
        //                 return false;
        //             }
        //             catch (Exception x)
        //             {
        //                 x.PublishSystemEvent("BroadcastRepository.FetchRawBroadcast()");
        //                 broadcastQueue = null;
        //                 return false;
        //             }
        //         }

    }
}
