
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Collections.Generic;
using System.IO;

using mINI;


namespace Yava.FoldersFile
{
    internal class FoldersFileReader : INIReader
    {
        private String filepath;
        private IList<Folder> folders;

        private Folder currentFolder;

        private Int32 currentLineNumber;
        private String currentLine;

        /// <summary>
        /// An INIReader that reads lines from a folders file
        /// adding sections and key=value pairs to a collection
        /// as folder names/options.
        /// </summary>
        public FoldersFileReader()
        {
            filepath = null;
            folders = null;

            currentFolder = null;

            currentLineNumber = 0;
            currentLine = String.Empty;
        }

        /// <summary>
        /// Clear internal state.
        /// </summary>
        private void ResetState()
        {
            filepath = null;
            folders = null;

            currentFolder = null;

            currentLineNumber = 0;
            currentLine = String.Empty;
        }

        /// <summary>
        /// Concise helper to create FoldersFileReadError exceptions.
        /// </summary>
        /// <param name="message">Error message.</param>
        private FoldersFileReadError ReadError(String message)
        {
            return new FoldersFileReadError(
                message,
                filepath,
                currentLine,
                currentLineNumber
            );
        }

        /// <summary>
        /// Add the current folder to the list and move on to the next.
        /// </summary>
        private void AddCurrentFolder()
        {
            if (currentFolder != null)
            {
                // validate required folder settings:
                if (currentFolder.Path == null)
                {
                    throw ReadError("Folder without path: " + currentFolder.Name);
                }

                folders.Add(currentFolder);
                currentFolder = null;
            }
        }

        /// <summary>
        /// Do not accept folders with no name.
        /// </summary>
        protected override void OnSectionEmpty()
        {
            throw ReadError("Empty folder name.");
        }

        /// <summary>
        /// Do not accept options with no name.
        /// </summary>
        protected override void OnKeyEmpty(String value)
        {
            throw ReadError("Empty folder option name.");
        }

        /// <summary>
        /// Do not accept options with no value.
        /// </summary>
        protected override void OnValueEmpty(String key)
        {
            throw ReadError("Empty folder option value.");
        }

        /// <summary>
        /// Syntax errors.
        /// </summary>
        protected override void OnUnknown(String line)
        {
            throw ReadError("Invalid syntax.");
        }

        /// <summary>
        /// On an empty line, add the current folder to the collection
        /// and move on to the next one.
        /// </summary>
        protected override void OnEmpty()
        {
            AddCurrentFolder();
        }

        /// <summary>
        /// On a new section, add  the current folder to the collection
        /// and create the next one.
        /// </summary>
        protected override void OnSection(String section)
        {
            AddCurrentFolder();
            currentFolder = new Folder(section);
        }

        /// <summary>
        /// Set key=value pairs as options to the current folder.
        /// </summary>
        protected override void OnKeyValue(String key, String value)
        {
            if (currentFolder != null)
            {
                switch (key.ToLower())
                {
                    case "path":
                        String path = value;
                        
                        path = Environment.ExpandEnvironmentVariables(value);
                        path = Path.GetFullPath(path);

                        currentFolder.Path = path;
                        break;

                    case "extensions":
                        String[] extensions = value.Split(',');

                        for (int i = 0; i < extensions.Length; i++)
                        {
                            // trim spaces and add leading dot where needed:
                            String extension = extensions[i].Trim();

                            if (!extension.StartsWith("."))
                            {
                                extension = "." + extension;
                            }

                            extensions[i] = extension;
                        }

                        currentFolder.Extensions = new HashSet<String>(extensions);
                        break;

                    default:
                        throw ReadError("Unknown folder option: " + key);
                }
            }
        }

        /// <summary>
        /// Read a folders file adding each folder with its options to a collection.
        /// </summary>
        /// <param name="filepath">Path to the file to read lines from.</param>
        /// <param name="folders">Target list to add folders to.</param>
        public void Read(String filepath, IList<Folder> folders)
        {
            this.filepath = filepath;
            this.folders = folders;

            try
            {
                foreach (String line in File.ReadLines(filepath))
                {
                    currentLineNumber++;
                    currentLine = line;
                    ReadLine(line);
                }

                // add the last folder:
                AddCurrentFolder();
            }
            finally
            {
                ResetState();
            }
        }
    }
}

