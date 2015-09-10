
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Collections.Generic;
using System.IO;


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
        public readonly String Path;

        /// <summary>
        /// Extensions to include when searching for files.
        /// </summary>
        public readonly HashSet<String> Extensions;

        /// <summary>
        /// A folder specification.
        /// FoldersFileReader generates them when reading the folders ini file.
        /// </summary>
        /// <param name="name">Folder name.</param>
        /// <param name="path">Folder path in the filesystem.</param>
        /// <param name="extensions">Extensions to include when searching for files.</param>
        public Folder(String name, String path, HashSet<String> extensions)
        {
            this.Name = name;
            this.Path = path;
            this.Extensions = extensions;
        }

        /// <summary>
        /// Search this folder path and return the matching files.
        /// </summary>
        public IEnumerable<FolderFile> GetFiles()
        {
            List<FolderFile> result = new List<FolderFile>();

            // no extensions specified:
            if (Extensions.Count == 0)
            {
                foreach (String filepath in Directory.EnumerateFiles(Path))
                {
                    result.Add(new FolderFile(this, filepath));
                } 
            }

            // filter by the specified extensions:
            else
            {
                foreach (String filepath in Directory.EnumerateFiles(Path))
                {
                    String extension = System.IO.Path.GetExtension(filepath);
                    if (Extensions.Contains(extension))
                    {
                        result.Add(new FolderFile(this, filepath));
                    }
                }
            }

            return result;
        }
    }
}

