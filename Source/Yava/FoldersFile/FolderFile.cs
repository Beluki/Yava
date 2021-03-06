﻿
// Yava.
// A simple, portable game/emulator launcher.


using System;


namespace Yava.FoldersFile
{
    internal class FolderFile
    {
        /// <summary>
        /// The folder this file belongs to.
        /// </summary>
        public readonly Folder Folder;

        /// <summary>
        /// File path in the filesystem.
        /// </summary>
        public readonly String Path;

        /// <summary>
        /// A file specification.
        /// Folder.EnumerateFiles() generates them from the filesystem.
        /// </summary>
        /// <param name="folder">The folder this file belongs to.</param>
        /// <param name="path">File path in the filesystem.</param>
        public FolderFile(Folder folder, String path)
        {
            this.Folder = folder;
            this.Path = path;
        }
    }
}

