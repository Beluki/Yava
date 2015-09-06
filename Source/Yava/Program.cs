
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Windows.Forms;


namespace Yava
{
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Yava());
        }
    }
}

