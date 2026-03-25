using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using DacQuest.DFX.Core;
using Mss.Data.Pocos;
using MicroOrm.Dapper.Repositories;

namespace Mss.Data.Repositories
{
//     public class BroadcastDetailsRepository : DapperRepository<BroadcastDetail>
//     {
//         public BroadcastDetailsRepository(IDbConnection connection)
//             : base(connection)
//         {
//         }
// 
//         internal bool FetchBroadcastDetails(
//             List<BroadcastQueue> broadcastQueue,
//             out List<BroadcastDetail> broadcastDetails)
//         {
//             try
//             {
//                 IEnumerable<int> headerIDs = broadcastQueue.Select(q => q.HeaderID);
//                 broadcastDetails = FindAll(h => headerIDs.Contains(h.HeaderID)).ToList();
//                 return broadcastDetails.Any();
//             }
//             catch (SqlException x)
//             {
//                 x.PublishSystemEvent("BroadcastHeaderRepository.FetchBroadcastDetails()");
//                 broadcastDetails = null;
//                 return false;
//             }
//             catch (Exception x)
//             {
//                 x.PublishSystemEvent("BroadcastHeaderRepository.FetchBroadcastDetails()");
//                 broadcastDetails = null;
//                 return false;
//             }
//         }
// 
//     }
}
