using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mss.Collections
{
    [Serializable]
    public partial class SpecialPickItem : XDataItem
    {
        public override Boolean Validate(out IEnumerable<String> errors)
        {
            List<String> errorMessages = null;
            if (FrontPickModeKey <= 0)
            {
                if (FrontPickMode == PickMode.ByPalletID)
                {
                    _AppendError(ref errorMessages, "A Pallet ID is required for Front.");
                }
                else if (FrontPickMode == PickMode.ByJobID)
                {
                    _AppendError(ref errorMessages, "A Job ID is required for Front");
                }
            }
            if (RearPickModeKey <= 0)
            {
                if (RearPickMode == PickMode.ByPalletID)
                {
                    _AppendError(ref errorMessages, "A Pallet ID is required for Rear.");
                }
                else if (RearPickMode == PickMode.ByJobID)
                {
                    _AppendError(ref errorMessages, "A Job ID is required for Rear");
                }
            }
            errors = errorMessages;
            return errors == null;
        }
        private void _AppendError(ref List<String> errorMessages, String errorMessage)
        {
            if (errorMessages == null)
            {
                errorMessages = new List<String>();
            }
            errorMessages.Add(errorMessage);
        }
    }

}
