using DacQuest.DFX.Core;
using MicroOrm.Dapper.Repositories;
using Mss.Common;
using Mss.Data.Pocos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Mss.Data.Repositories
{
    public class BroadcastRepository : DapperRepository<BroadcastQueue>
    {
//         BroadcastHeaderRepository _headerRepository = null;

        public BroadcastRepository(IDbConnection connection)
            : base(connection)
        {
        }

        public async Task<bool> FetchRawBroadcastsAsync(Out<IEnumerable<BroadcastQueue>> rawBroadcasts)
        {
            // Step 1: Get BroadcastQueue records with BroadcastHeader populated
            rawBroadcasts.Value = (await FindAllAsync(q => !q.Processed))
                .OrderBy(q => q.HeaderID);

            if (!rawBroadcasts.Value.Any())
            {
                return false;
            }

            // Step 2: Separately load BroadcastDetails for each header
            DapperRepository<BroadcastHeader> headerRepo = new DapperRepository<BroadcastHeader>(Connection);
            DapperRepository<BroadcastDetail> detailRepo = new DapperRepository<BroadcastDetail>(Connection);

            IEnumerable<int> headerIDs = rawBroadcasts.Value
                .Where(q => q.BroadcastHeader != null)
                .Select(q => q.BroadcastHeader.HeaderID)
                .Distinct();

            IEnumerable<BroadcastDetail> details = (await detailRepo.FindAllAsync(d => headerIDs.Contains(d.HeaderID)));

            // Step 3: Wire up BroadcastDetails onto each BroadcastHeader
            foreach (BroadcastQueue queue in rawBroadcasts.Value.Where(q => q.BroadcastHeader != null))
            {
                queue.BroadcastHeader.BroadcastDetails = details
                    .Where(d => d.HeaderID == queue.BroadcastHeader.HeaderID)
                    .ToList();
            }

            return true;
        }

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

        //         public void DeleteFromQueue(int broadcastHeaderID)
        //         {
        // 
        //         }

    }
}
