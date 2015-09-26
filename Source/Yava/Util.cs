
// Yava.
// A simple, portable game/emulator launcher.


using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;


namespace Yava
{
    internal static class Util
    {
        ///
        /// IO
        ///

        /// <summary>
        /// Serialize an object to a binary file.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        /// <param name="filepath">Destination path.</param>
        public static void Serialize(Object value, String filepath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                formatter.Serialize(fs, value);
            }
        }

        /// <summary>
        /// Deserialize an object from a binary file.
        /// </summary>
        /// <param name="filepath">File path.</param>
        public static Object Deserialize(String filepath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                return formatter.Deserialize(fs);
            }
        }

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

        ///
        /// Resources
        ///

        /// <summary>
        /// Load an embedded resource as an icon.
        /// </summary>
        /// <param name="resource">Resource name, including namespace.</param>
        public static Icon ResourceAsIcon(String resource)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                return new Icon(stream);
            }
        }
    }
}

