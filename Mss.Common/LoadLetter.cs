using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public enum LoadLetter
    {
        None = 0,
        A    = 1,
        B    = 2,
    }

    public static class LoadLetterExtensions
    {
        public static string Letter(this LoadLetter loadLetter)
        {
            string letter = loadLetter.ToString();
            if (letter.Length > 1)
            {
                letter = "?";
            }
            return letter;
        }
        public static string LoadName(this LoadLetter loadLetter) => $"Slug{loadLetter.Letter()}";
        public static string LoadDisplayName(this LoadLetter loadLetter) => $"Load {loadLetter.Letter()}";

//         public static int DockNumber(this LoadLetter loadLetter) => 30 + (int)loadLetter;
    }

}
