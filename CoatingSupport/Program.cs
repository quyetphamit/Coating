using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoatingSupport
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static Mutex mutex = null; // Ngăn chặn việc mở 2 chương trình
        [STAThread]
        static void Main()
        {
            const string appName = "Coating Support";
            bool createNew;
            mutex = new Mutex(true, appName, out createNew);
            if (!createNew)
            {
                // App is already running! Exiting the application
                MessageBox.Show("App is already running!");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
