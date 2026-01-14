using DacQuest.DFX.Core.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mss.ShippingAgent
{
    public class Program : XTrayApplication
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Run(
                typeof(Program),
                typeof(XAgentApplicationContext),
                Properties.Resources.TempIcon,
                false);
        }
    }
}
