using CSharpFunctionalExtensions;
using System.IO;

namespace Scripting.Js.v1
{
    public static partial class FileIO
    {
        /// <summary>
        /// Return the path of a folder searching the folder in the current execution folder and up in the folder tree. 
        /// Return failure if the file is not found.
        /// </summary>
        public static Result<string> SearchAFolderAboveTheCurrentDirectoryOfTheApplication(string folderToSearchWithoutPath)
        {
            string currentPath = Directory.GetCurrentDirectory();  // read the current directory (the execution folder of the program)
            string fullPath = Path.GetFullPath(Path.Combine(currentPath, folderToSearchWithoutPath));  // build the full path to search
            if (Directory.Exists(fullPath)) { return Result.Ok(fullPath); }  // if the path is found, return it

            // if the execution is not already ended, loop until the folder is found or until the root folder is reached
            while (true)
            {
                string tempPath = Path.GetFullPath(Path.Combine(currentPath, "..")); // build a path up one level
                if (tempPath == currentPath) { break; }  // if the root folder is reached, end the search
                currentPath = tempPath;  // save the current path
                fullPath = Path.GetFullPath(Path.Combine(currentPath, folderToSearchWithoutPath));  // build the full path to search
                if (Directory.Exists(fullPath)) { return Result.Ok(fullPath); }  // if the path is found, return it
            }

            return Result.Fail<string>("not found");  // return failure if the execution reached this point
        }
    }
}
