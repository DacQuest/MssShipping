using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using DacQuest.DFX.Core;
using Mss.Data;
using Mss.Data.Pocos;
using MicroOrm.Dapper.Repositories;

namespace Mss.Data.Repositories
{
//     public class BroadcastHeaderRepository : DapperRepository<BroadcastHeader>
//     {
//         public BroadcastHeaderRepository(IDbConnection connection)
//             : base(connection)
//         {
//         }
// 
//         internal bool FetchBroadcastHeaders(
//             List<BroadcastQueue> broadcastQueue,
//             out List<BroadcastHeader> broadcastHeaders)
//         {
//             try
//             {
//                 IEnumerable<int> headerIDs = broadcastQueue.Select(q => q.HeaderID);
//                 broadcastHeaders = FindAll(h => headerIDs.Contains(h.HeaderID)).ToList();
//                 return broadcastHeaders.Any();
//             }
//             catch (SqlException x)
//             {
//                 x.PublishSystemEvent("BroadcastHeaderRepository.FetchBroadcastHeaders()");
//                 broadcastHeaders = null;
//                 return false;
//             }
//             catch (Exception x)
//             {
//                 x.PublishSystemEvent("BroadcastHeaderRepository.FetchBroadcastHeaders()");
//                 broadcastHeaders = null;
//                 return false;
//             }
//         }
// 
//     }
}
