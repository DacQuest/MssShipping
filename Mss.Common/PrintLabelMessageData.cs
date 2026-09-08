using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    [Serializable]
    public class PrintLabelMessageData : XMessageData
    {
        public const string PrintLoadLabelRequest = "PrintLoadLabelRequest";
        public const string ReprintLoadLabelRequest = "ReprintLoadLabelRequest";
        public const string ReprintShippingLabelRequest = "ReprintShippingLabelRequest";

        public PrintLabelMessageData(
            SlugLetter slugLetter,
            Levels slugLevel,
            int loadIndex)
        {
            PrintLoadLabel = false;
            SlugLetter = slugLetter;
            SlugLevel = slugLevel;
            LoadIndex = loadIndex;
        }

        public PrintLabelMessageData(
            SlugLetter slugLetter,
            string trailerNumber,
            string smallestRotation,
            string largestRotation,
            int palletCount)
        {
            PrintLoadLabel = true;
            SlugLetter = slugLetter;
            TrailerNumber = trailerNumber;
            SmallestRotation = smallestRotation;
            LargestRotation = largestRotation;
            PalletCount = palletCount;
        }

        public PrintLabelMessageData(
            SlugLetter slugLetter,
            string smallestRotation,
            string largestRotation,
            int palletCount)
        {
            PrintLoadLabel = true;
            SlugLetter = slugLetter;
            SmallestRotation = smallestRotation;
            LargestRotation = largestRotation;
            PalletCount = palletCount;
        }

        public bool PrintLoadLabel { get; } = false;

        public SlugLetter SlugLetter { get; } = SlugLetter.None;

        public Levels SlugLevel { get; } = Levels.None;

        public int LoadIndex { get; } = -1;

        public string TrailerNumber { get; } = Constant.NoTrailerNumber;

        public string SmallestRotation { get; } = string.Empty;

        public string LargestRotation { get; } = string.Empty;

        public int PalletCount { get; } = 0;

    }
}
