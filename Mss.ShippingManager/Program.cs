using DacQuest.DFX.Core;
using DacQuest.DFX.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mss.ShippingManager
{
    public class Program : XFormApplication
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Run(typeof(Program), typeof(ViewHostForm), false);
#else
            Run(typeof(Program), true);
#endif
        }
    }
}
