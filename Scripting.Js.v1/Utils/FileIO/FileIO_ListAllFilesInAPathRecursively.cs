using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Scripting.Js.v1
{
    public static partial class FileIO
    {
        /// <summary>
        /// List all files in a directory in a recursive way (list all directory levels).
        /// Return full paths.
        /// Return empty list if the directory is not found or if the directory is empty.
        /// </summary>
        public static ImmutableList<string> ListAllFilesInAPathRecursively(string pathToList)
        {
            List<string> filesFullPath = new List<string>();

            if (File.Exists(pathToList)) { filesFullPath.Add(pathToList); }  // if 'pathToList' is a file, add it as fullpath
            else if (Directory.Exists(pathToList)) { ProcessDirectory(pathToList); }  // if 'pathToList' is a directory, process it
            else { return ImmutableList<string>.Empty; }  // return empty list  

            return filesFullPath.ToImmutableList();

            #region local functions
            // Process all files in the directory 'targetDirectory', recurse on any found directories and process the contained files
            void ProcessDirectory(string targetDirectory)
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                    filesFullPath.Add(fileName);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory);
            }
            #endregion local functions
        }
    }
}
