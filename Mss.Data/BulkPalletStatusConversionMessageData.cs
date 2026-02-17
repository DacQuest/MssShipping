using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Messaging;
using Mss.Common;
using Mss.Collections;

namespace Mss.Data
{
    [Serializable]
    public class BulkPalletStatusConversionMessageData : XMessageData
    {
        public const string BulkPalletStatusConversionMessageTopic = "BulkPalletStatusConversion";

        private List<BinItem> _binItems;
        private PalletStatus _newPalletStatus;
        private int _newHoldCode;
        private string _newComment;
        private bool _overwriteComment = false;
        private bool _success = true;
        private bool _markAudit = false;
        private bool _unmarkAudit = false;

        public BulkPalletStatusConversionMessageData(
            List<BinItem> binItems,
            PalletStatus newPalletStatus,
            int newHoldCode,
            string newComment,
            bool overwriteComment,
            bool markAudit,
            bool unmarkAudit)
        {
            _binItems = binItems;
            _newPalletStatus = newPalletStatus;
            _newHoldCode = newHoldCode;
            _newComment = newComment;
            _markAudit = markAudit;
            _unmarkAudit = unmarkAudit;
            _overwriteComment = overwriteComment;
        }

        public BulkPalletStatusConversionMessageData(
            List<BinItem> binItems,
            string newPickSku)
        {
            _binItems = binItems;
        }

        public BulkPalletStatusConversionMessageData(List<BinItem> failedConversionItems)
        {
            _binItems = failedConversionItems;
            _success = _binItems == null || _binItems.Count == 0;
        }

        public List<BinItem> BinItems => _binItems;
        public PalletStatus NewPalletStatus => _newPalletStatus;
        public int NewHoldCode => _newHoldCode;
        public string NewComment => _newComment;
        public bool OverwriteComment => _overwriteComment;
        public bool Success => _success;
        public bool MarkAudit => _markAudit;
        public bool UnmarkAudit => _unmarkAudit;
    }
}
