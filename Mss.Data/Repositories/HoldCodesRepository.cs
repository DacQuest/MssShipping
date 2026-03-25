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

        public bool FetchHoldCodes(out List<HoldCodeItem> holdCodes)
        {
            holdCodes = new List<HoldCodeItem>();
            try
            {
                foreach (HoldCode rawHoldCode in FindAll())
                {
                    if (rawHoldCode.Active)
                    {
                        HoldCodeItem holdCodeItem = new HoldCodeItem
                        {
                            BitPosition = rawHoldCode.BitPosition,
                            HoldCode = rawHoldCode.Code,
                            Description = rawHoldCode.Description,
                            CreatedOn = rawHoldCode.Inserted_DT
                        };
                        holdCodes.Add(holdCodeItem);
                    }
                }
                return true;
            }
            catch (SqlException x)
            {
                holdCodes = null;
                x.PublishSystemEvent("HoldCodesRepository");
                return false;
            }
            catch (Exception x)
            {
                holdCodes = null;
                x.PublishSystemEvent("HoldCodesRepository");
                return false;
            }
        }

    }
}
