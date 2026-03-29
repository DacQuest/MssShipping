using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using DacQuest.DFX.Core;
using Mss.Collections;
using Mss.Data.Pocos;
using MicroOrm.Dapper.Repositories;

namespace Mss.Data.Repositories
{
    public class HoldCodesRepository : ReadOnlyDapperRepository<HoldCode>
    {
        public HoldCodesRepository(IDbConnection connection)
            : base(connection)
        {
        }

        public bool TryFetchRawHoldCodes(out IEnumerable<HoldCode> rawHoldCodes)
        {
            try
            {
                rawHoldCodes = FindAllAsync()
                    .GetAwaiter()
                    .GetResult();
                if (!rawHoldCodes.Any())
                {
                    rawHoldCodes = null;
                    return false;
                }
                return true;
            }
            catch (Exception x)
            {
                rawHoldCodes = null;
                x.PublishSystemEvent(nameof(TryFetchRawHoldCodes));
                return false;
            }
        }

    }
}
