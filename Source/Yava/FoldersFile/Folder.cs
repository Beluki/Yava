
// Yava.
// A simple, portable game/rom launcher.


using System;


namespace Yava.FoldersFile
{
    internal class Folder
    {
        /// <summary>
        /// Folder name (section in the ini).
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// Folder path in the filesystem.
        /// </summary>
        public String Path;

        /// <summary>
        /// A folder specification.
        /// FoldersFileReader generates them when reading the folders ini file.
        /// </summary>
        /// <param name="name">Folder name.</param>
        public Folder(String name)
        {
            this.Name = name;

            this.Path = null;
        }
    }
}

