
// Yava.
// A simple, portable game/emulator launcher.


using System;
using System.IO;
using System.Windows.Forms;


namespace Yava
{
    internal static class Util
    {
        ///
        /// MessageBoxes
        ///

        /// <summary>
        /// Show a MessageBox with Yes and No buttons.
        /// Return true when Yes is clicked, false otherwise.
        /// </summary>
        /// <param name="text">MessageBox text.</param>
        /// <param name="caption">MessageBox caption.</param>
        public static Boolean MessageBoxYesNo(String text, String caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        ///
        /// OS information
        ///

        /// <summary>
        /// Get the path for the directory that contains
        /// the current application executable.
        /// </summary>
        public static String ApplicationFolder
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath);
            }
        }
    }
}

