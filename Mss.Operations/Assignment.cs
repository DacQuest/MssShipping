using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public class Assignment : LineOperationBase
    {

        private AssignmentParameterSetWrapper _parameters;

        protected int Number => _parameters.Number;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Number)
                {
                    case 1:
                        return OperationCode.AS1;
                    case 2:
                        return OperationCode.AS2;
                    case 3:
                        return OperationCode.AS3;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        protected override bool DoProcessPallet(out int moveCommand, out string extendedState)
        {
            throw new NotImplementedException("Assignment Operation is not implemented!");
        }

    }
}
