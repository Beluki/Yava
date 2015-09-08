
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
        public String Path;

        /// <summary>
        /// Extensions to include when searching for files.
        /// </summary>
        public HashSet<String> Extensions;

        /// <summary>
        /// A folder specification.
        /// FoldersFileReader generates them when reading the folders ini file.
        /// </summary>
        /// <param name="name">Folder name.</param>
        public Folder(String name)
        {
            this.Name = name;

            this.Path = null;
            this.Extensions = new HashSet<String>();
        }

        /// <summary>
        /// Search this folder path and return the matching files.
        /// </summary>
        public IEnumerable<String> GetFiles()
        {
            // no extensions to filter, return everything:
            if (Extensions.Count == 0)
            {
                return Directory.GetFiles(Path);
            }

            List<String> result = new List<String>();

            foreach (String filepath in Directory.GetFiles(Path))
            {
                String extension = System.IO.Path.GetExtension(filepath);

                if (Extensions.Contains(extension))
                {
                    result.Add(filepath);
                }
            }

            return result;
        }
    }
}

