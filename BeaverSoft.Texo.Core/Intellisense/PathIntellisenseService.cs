using BeaverSoft.Texo.Core.Path;
using BeaverSoft.Texo.Core.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeaverSoft.Texo.Core.Intellisense
{
    public class PathIntellisenseProvider : ITokenIntellisenseProvider
    {
        public IEnumerable<IItem> Help(string currentPath)
        {
            string filter = PathConstants.SEARCH_TERM_ALL;

            if (string.IsNullOrWhiteSpace(currentPath)
                || currentPath == PathConstants.RELATIVE_CURRENT_DIRECTORY)
            {
                return BuildFromDirectory(PathConstants.RELATIVE_CURRENT_DIRECTORY, filter);
            }

            if (!currentPath.IsValidPath())
            {
                return new IItem[0];
            }         
            
            if (currentPath[currentPath.Length-1].IsDirectorySeparator()
                && currentPath.GetPathType() == PathTypeEnum.Directory)
            {
                return BuildFromDirectory(currentPath, filter);
            }

            string parentDirectory = currentPath.GetParentDirectoryPath();

            if (string.IsNullOrEmpty(parentDirectory))
            {
                if (currentPath.IsDriveRelativePath())
                {
                    parentDirectory = $"{currentPath}.";
                }
                else
                {
                    parentDirectory = PathConstants.RELATIVE_CURRENT_DIRECTORY;
                    filter = currentPath + PathConstants.WILDCARD_ANY_CHARACTER;
                }
            }
            else
            {
                TexoPath texoPath = new TexoPath(currentPath);
                string lastSegment = texoPath.Segments.Last().Value;

               if (lastSegment != PathConstants.RELATIVE_CURRENT_DIRECTORY)
                {
                    filter = lastSegment + PathConstants.WILDCARD_ANY_CHARACTER;
                }
            }
 
            return BuildFromDirectory(parentDirectory, filter);
        }

        private IEnumerable<IItem> BuildFromDirectory(string directory, string filter)
        {
            foreach (string directoryName in TexoDirectory.GetDirectories(directory, filter))
            {
                string justName = directoryName.GetFileNameOrDirectoryName();
                yield return Item.AsIntellisense(justName, directoryName + System.IO.Path.DirectorySeparatorChar, "directory", null);
            }

            foreach (string fileName in TexoDirectory.GetFiles(directory, filter))
            {
                string justName = fileName.GetFileNameOrDirectoryName();
                yield return Item.AsIntellisense(justName, fileName, "file", null);
            }
        }
    }
}
