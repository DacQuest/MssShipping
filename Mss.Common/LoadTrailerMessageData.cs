using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Serializable]
    public class LoadTrailerMessageData : XMessageData
    {
        public static readonly string LoadTrailerMessageTopic = "LoadTrailerMessageTopic";

        public LoadTrailerMessageData(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public LoadTrailerMessageData(
            SlugLetter slugLetter,
//             string trailerID,
            XSystemEvent systemEvent)
        {
            SlugLetter = slugLetter;
//             TrailerID = trailerID;
            SystemEvent = systemEvent;
        }

        public SlugLetter SlugLetter { get; } = SlugLetter.None;

        public XSystemEvent SystemEvent { get; } = null;

        public string ErrorMessage { get; set; } = string.Empty;

//         public string TrailerID { get; set; } = string.Empty;

    }
}
