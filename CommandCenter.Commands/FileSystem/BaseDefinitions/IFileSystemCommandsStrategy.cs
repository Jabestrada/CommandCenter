using System;
using System.Collections.Generic;

namespace CommandCenter.Commands.FileSystem.BaseDefinitions {
    public interface IFileSystemCommandsStrategy {
        bool FileExists(string filename);
        void FileCopy(string sourceFilename, string destinationFilename);
        void FileDelete(string filename);
        void FileMove(string sourceFilename, string destinationFilename);
        void FileCreate(string fileName);
        bool DirectoryExists(string dirName);
        void DirectoryCopyContents(string sourceDirName, string destinationDirName, Func<string, string, bool> preCopyCallback, Action<string, string> postCopyCallback);
        void DirectoryDelete(string dirName);
        void DirectoryDeleteContentsOnly(string sourceDirectory, Action<string, FileSystemItemType> progressCallback);
        void DirectoryMove(string sourceDir, string targetDir);

        void DirectoryMoveContents(string sourceDir, string targetDir);
        IEnumerable<string> DirectoryGetFiles(string sourceDir);
    }

    public enum FileSystemItemType {
        File,
        Directory
    }
}
