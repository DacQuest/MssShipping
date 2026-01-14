using DacQuest.DFX.Core;
using DacQuest.DFX.Forms;
using DacQuest.DFX.WindowsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mss.ShippingService
{
    public class Program : XWindowsServiceApplication
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
            RunDebug(typeof(Program), typeof(ServiceHostForm), true);
#else
            Run(typeof(Program), true);
#endif
        }
    }
}
