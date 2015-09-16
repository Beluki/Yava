﻿
// Yava.
// A simple, portable game/emulator launcher.


using System;


namespace Yava.FoldersFile
{
    internal class FoldersFileReadError : Exception
    {
        /// <summary>
        /// Path to the file that triggered the error.
        /// </summary>
        public readonly String FilePath;

        /// <summary>
        /// Text for the incorrect line.
        /// </summary>
        public readonly String Line;

        /// <summary>
        /// Line number where the error happened.
        /// </summary>
        public readonly Int32 LineNumber;

        /// <summary>
        /// Raised by FoldersFileReader on a reading error.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="filepath">Path to the file that triggered the error.</param>
        /// <param name="line">Text for the incorrect line.</param>
        /// <param name="linenumber">Line number where the error happened.</param>
        public FoldersFileReadError(String message, String filepath, String line, Int32 linenumber) : base(message)
        {
            FilePath = filepath;
            Line = line;
            LineNumber = linenumber;
        }
    }
}

