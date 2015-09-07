
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.IO;
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
            // default path for the settings and the folders file:
            String currentFolder = Util.ApplicationFolder;

            String settingsFilepath = Path.Combine(currentFolder, "Yava.dat");
            String foldersFilepath = Path.Combine(currentFolder, "Folders.ini");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Yava(settingsFilepath, foldersFilepath));
        }
    }
}

