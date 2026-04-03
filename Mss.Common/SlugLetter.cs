using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum SlugLetter
    {
        None = 0,
        A    = 1,
        B    = 2,
    }

    public static class SlugLetterExtensions
    {
        public static string Letter(this SlugLetter slugLetter)
        {
            string letter = slugLetter.ToString();
            if (letter.Length > 1)
            {
                letter = "?";
            }
            return letter;
        }
        public static string SlugName(this SlugLetter slugLetter) => $"Slug{slugLetter.Letter()}";
        public static string SlugDisplayName(this SlugLetter slugLetter) => $"Slug {slugLetter.Letter()}";

//         public static int DockNumber(this SlugLetter slugLetter) => 30 + (int)slugLetter;
    }

}
