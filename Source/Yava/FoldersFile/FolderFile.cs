
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Collections.Generic;
using System.IO;


namespace Yava.FoldersFile
{
    internal class FolderFile
    {
        /// <summary>
        /// The folder this file belongs to.
        /// </summary>
        public readonly Folder folder;

        /// <summary>
        /// File path in the filesystem.
        /// </summary>
        public readonly String Path;

        /// <summary>
        /// A file specification.
        /// Folder.GetFiles() generates them from the filesystem.
        /// </summary>
        /// <param name="folder">The folder this file belongs to.</param>
        /// <param name="path">File path in the filesystem.</param>
        public FolderFile(Folder folder, String path)
        {
            this.folder = folder;
            this.Path = path;
        }
    }
}

