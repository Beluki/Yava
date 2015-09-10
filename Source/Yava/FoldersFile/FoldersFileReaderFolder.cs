
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Collections.Generic;


namespace Yava.FoldersFile
{
    internal class FoldersFileReaderFolder
    {
        /// <summary>
        /// Folder name (section in the ini).
        /// </summary>
        public String Name;

        /// <summary>
        /// Folder path in the filesystem.
        /// </summary>
        public String Path;

        /// <summary>
        /// Extensions to include when searching for files.
        /// </summary>
        public HashSet<String> Extensions;

        /// <summary>
        /// A folder specification.
        /// Used inside FoldersFileReader to gather the data for each folder.
        /// Unlike the Folder class, this one initializes everything to null and is mutable.
        /// </summary>
        public FoldersFileReaderFolder()
        {
            this.Name = null;
            this.Path = null;
            this.Extensions = null;
        }
    }
}

