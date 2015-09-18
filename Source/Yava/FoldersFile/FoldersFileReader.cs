
// Yava.
// A simple, portable game/emulator launcher.


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

        private HashSet<String> seenFolderNames;
        private FoldersFileReaderFolder currentFolder;

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

            seenFolderNames = new HashSet<String>();
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

            seenFolderNames.Clear();
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
                // check required options:
                if (currentFolder.Name == null)
                {
                    throw ReadError("Folder without name.");
                }

                if (currentFolder.Path == null)
                {
                    throw ReadError("Expected 'path = value' option for folder: " + currentFolder.Name);
                }

                if (currentFolder.Executable == null)
                {
                    throw ReadError("Expected 'executable = value' option for folder: " + currentFolder.Name);
                }

                // create and add the new folder:
                Folder folder = new Folder(
                    currentFolder.Name,
                    currentFolder.Path,
                    currentFolder.Executable,
                    currentFolder.Extensions,
                    currentFolder.Parameters,
                    currentFolder.WorkingDirectory
                );

                folders.Add(folder);
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
        /// Options without value are fine.
        /// (each option validates its own value in OnKeyValue)
        /// </summary>
        protected override void OnValueEmpty(String key)
        {

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
        /// On a new section, add the current folder to the collection
        /// and create the next one.
        /// </summary>
        protected override void OnSection(String section)
        {
            AddCurrentFolder();

            // folder names must be unique:
            String name = section;

            if (seenFolderNames.Contains(name))
            {
                throw ReadError("Duplicate folder name: " + name);
            }

            seenFolderNames.Add(name);
            currentFolder = new FoldersFileReaderFolder();
            currentFolder.Name = name;
        }

        /// <summary>
        /// Try to parse a folder path option.
        /// The path cannot be empty and must be syntactically valid.
        /// </summary>
        /// <param name="value">Path to parse.</param>
        private String ReadFolderPath(String value)
        {
            if (value == String.Empty)
            {
                throw ReadError("Option 'path' cannot be empty for folder: " + currentFolder.Name);
            }

            try
            {
                Path.GetFullPath(value);
                return value;
            }
            catch (Exception exception)
            {
                throw ReadError("Unable to read 'path' option: " + exception.Message);
            }
        }

        /// <summary>
        /// Try to parse a folder executable option.
        /// The executable cannot be empty.
        /// </summary>
        /// <param name="value">Path to parse.</param>
        private String ReadFolderExecutable(String value)
        {
            if (value == String.Empty)
            {
                throw ReadError("Option 'executable' cannot be empty for folder: " + currentFolder.Name);
            }

            return value;
        }

        /// <summary>
        /// Try to parse a folder extensions option.
        /// Spaces are trimmed around each extension.
        /// Leading dots are added where needed.
        /// </summary>
        /// <param name="value">Comma separated extensions.</param>
        private HashSet<String> ReadFolderExtensions(String value)
        {
            if (value == String.Empty)
            {
                throw ReadError("Option 'extensions' cannot be empty for folder: " + currentFolder.Name);
            }

            String[] extensions = value.Split(',');

            for (Int32 i = 0; i < extensions.Length; i++)
            {
                String extension = extensions[i].Trim();

                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                extensions[i] = extension;
            }

            return new HashSet<String>(extensions);
        }

        /// <summary>
        /// Try to parse folder parameters option.
        /// Currently does nothing and returns the value as is.
        /// </summary>
        /// <param name="value">Parameters to parse.</param>
        private String ReadFolderParameters(String value)
        {
            return value;
        }

        /// <summary>
        /// Try to parse folder workingdirectory option.
        /// It cannot be empty.
        /// </summary>
        /// <param name="value">Path to parse.</param>
        private String ReadFolderWorkingDirectory(String value)
        {
            if (value == String.Empty)
            {
                throw ReadError("Option 'workingdirectory' cannot be empty for folder: " + currentFolder.Name);
            }

            return value;
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
                        currentFolder.Path = ReadFolderPath(value);
                        break;

                    case "executable":
                        currentFolder.Executable = ReadFolderExecutable(value);
                        break;

                    case "extensions":
                        currentFolder.Extensions = ReadFolderExtensions(value);
                        break;

                    case "parameters":
                        currentFolder.Parameters = ReadFolderParameters(value);
                        break;

                    case "workingdirectory":
                        currentFolder.WorkingDirectory = ReadFolderWorkingDirectory(value);
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

