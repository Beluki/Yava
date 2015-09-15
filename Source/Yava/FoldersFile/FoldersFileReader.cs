
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
                // name is required:
                if (currentFolder.Name == null)
                {
                    throw ReadError("Folder without name.");
                }

                // path is required:
                if (currentFolder.Path == null)
                {
                    throw ReadError("Folder without path: " + currentFolder.Name);
                }

                // extensions are optional:
                if (currentFolder.Extensions == null)
                {
                    currentFolder.Extensions = new HashSet<String>();
                }

                // executable is required:
                if (currentFolder.Executable == null)
                {
                    throw ReadError("Folder without executable: " + currentFolder.Name);
                }

                // parameters are optional:
                if (currentFolder.Parameters == null)
                {
                    currentFolder.Parameters = "%FILEPATH%";
                }

                // working directory is optional:
                if (currentFolder.WorkingDirectory == null)
                {
                    currentFolder.WorkingDirectory = Path.GetDirectoryName(currentFolder.Executable);
                }

                Folder folder = new Folder(
                    currentFolder.Name,
                    currentFolder.Path,
                    currentFolder.Extensions,
                    currentFolder.Executable,
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
        /// On a new section, add the current folder to the collection
        /// and create the next one.
        /// </summary>
        protected override void OnSection(String section)
        {
            AddCurrentFolder();

            currentFolder = new FoldersFileReaderFolder();
            currentFolder.Name = section;
        }

        /// <summary>
        /// Try to parse a folder path.
        /// Relative paths are converted to absolute.
        /// Environment variables are expanded (e.g. %APPDATA%).
        /// </summary>
        /// <param name="value">Path to read.</param>
        private String ReadFolderPath(String value)
        {
            try
            {
                return Util.ToAbsoluteExpandedPath(value);
            }
            catch (Exception exception)
            {
                throw ReadError(exception.Message);
            }
        }

        /// <summary>
        /// Try to parse folder extensions.
        /// Spaces are trimmed around each extension.
        /// Leading dots are added where needed.
        /// </summary>
        /// <param name="value">Comma separated extensions.</param>
        private HashSet<String> ReadFolderExtensions(String value)
        {
            String[] extensions = value.Split(',');

            for (int i = 0; i < extensions.Length; i++)
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
        /// Try to parse a folder executable.
        /// Relative paths are converted to absolute.
        /// Environment variables are expanded (e.g. %APPDATA%).
        /// </summary>
        /// <param name="value">Path to read.</param>
        private String ReadFolderExecutable(String value)
        {
            try
            {
                return Util.ToAbsoluteExpandedPath(value);
            }
            catch (Exception exception)
            {
                throw ReadError(exception.Message);
            }
        }

        /// <summary>
        /// Try to parse folder parameters.
        /// </summary>
        /// <param name="value">Parameters to read.</param>
        private String ReadFolderParameters(String value)
        {
            return value;
        }

        /// <summary>
        /// Try to parse a folder working directory.
        /// Relative paths are converted to absolute.
        /// Environment variables are expanded (e.g. %APPDATA%).
        /// </summary>
        /// <param name="value">Path to read.</param>
        private String ReadFolderWorkingDirectory(String value)
        {
            try
            {
                return Util.ToAbsoluteExpandedPath(value);
            }
            catch (Exception exception)
            {
                throw ReadError(exception.Message);
            }
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

                    case "extensions":
                        currentFolder.Extensions = ReadFolderExtensions(value);
                        break;

                    case "executable":
                        currentFolder.Executable = ReadFolderExecutable(value);
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

