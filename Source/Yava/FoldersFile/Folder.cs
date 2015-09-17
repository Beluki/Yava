
// Yava.
// A simple, portable game/emulator launcher.


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
        /// Application to start.
        /// </summary>
        public readonly String Executable;

        /// <summary>
        /// Extensions to include when searching for files.
        /// Can be null when unspecified in the ini file.
        /// </summary>
        public readonly HashSet<String> Extensions;

        /// <summary>
        /// Command-line arguments to use when launching the application.
        /// Can be null when unspecified in the ini file.
        /// </summary>
        public readonly String Parameters;

        /// <summary>
        /// Initial directory for the application to be started.
        /// Can be null when unspecified in the ini file.
        /// </summary>
        public readonly String WorkingDirectory;

        /// <summary>
        /// A folder specification.
        /// FoldersFileReader generates them when reading the folders ini file.
        /// </summary>
        /// <param name="name">Folder name.</param>
        /// <param name="path">Folder path in the filesystem.</param>
        /// <param name="extensions">Extensions to include when searching for files.</param>
        /// <param name="executable">Application to start.</param>
        /// <param name="parameters">Command-line arguments to use when launching the application.</param>
        /// <param name="workingdirectory">Initial directory for the application to be started.</param>
        public Folder(String name, String path, HashSet<String> extensions, String executable, String parameters, String workingdirectory)
        {
            this.Name = name;
            this.Path = path;
            this.Extensions = extensions;
            this.Executable = executable;
            this.Parameters = parameters;
            this.WorkingDirectory = workingdirectory;
        }

        /// <summary>
        /// Search this folder path and return the matching files.
        /// </summary>
        public IEnumerable<FolderFile> EnumerateFiles()
        {
            // no extensions specified:
            if (Extensions == null)
            {
                foreach (String filepath in Directory.EnumerateFiles(Path))
                {
                    yield return new FolderFile(this, filepath);
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
                        yield return new FolderFile(this, filepath);
                    }
                }
            }
        }
    }
}

