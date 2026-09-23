using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data
{
    public class BroadcastCoverageData
    {
        public readonly int Rotation;
        public readonly string Csn;
        public readonly VehicleRow VehicleRow;
        public readonly string Sku;
        public readonly string Status;

        public BroadcastCoverageData(
            int rotation,
            string csn,
            VehicleRow vehicleRow,
            string sku,
            string status)
        {
            Rotation = rotation;
            Csn = csn;
            VehicleRow = vehicleRow;
            Sku = sku;
            Status = status;
        }
    }
}
