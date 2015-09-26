
// Yava.
// A simple, portable game/emulator launcher.


using System;
using System.Collections.Generic;
using System.IO;

using Yava.FoldersFile;


namespace Yava
{
    [Serializable]
    internal class YavaSettings
    {
        /// <summary>
        /// Last form width.
        /// </summary>
        public Int32 LastYavaFormWidth;

        /// <summary>
        /// Last form height.
        /// </summary>
        public Int32 LastYavaFormHeight;

        /// <summary>
        /// Last folders listview width.
        /// </summary>
        public Int32 LastFoldersListViewWidth;

        /// <summary>
        /// Last files listview width.
        /// </summary>
        public Int32 LastFilesListViewWidth;

        /// <summary>
        /// Last selected folder name in the folders listview.
        /// </summary>
        public String LastSelectedFolderName;

        /// <summary>
        /// A map from folder names to their last selected file path.
        /// </summary>
        public Dictionary<String, String> FolderNameToLastSelectedFilePath;

        /// <summary>
        /// Stores program settings.
        /// </summary>
        public YavaSettings()
        {
            LastYavaFormWidth = 640;
            LastYavaFormHeight = 480;
            LastFoldersListViewWidth = 200;
            LastFilesListViewWidth = 400;
            LastSelectedFolderName = null;
            FolderNameToLastSelectedFilePath = new Dictionary<String, String>();
        }
    }
}

