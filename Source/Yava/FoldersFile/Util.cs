
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.IO;


namespace Yava.FoldersFile
{
    internal static class Util
    {
        /// <summary>
        /// Convert a path to an absolute path, with all the environment variables
        /// expanded (e.g. %APPDATA%).
        /// </summary>
        /// <param name="path">Input path.</param>
        /// <returns></returns>
        public static String ToAbsoluteExpandedPath(String path)
        {
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
        }
    }
}

