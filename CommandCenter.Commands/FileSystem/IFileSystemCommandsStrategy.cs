﻿using System;

namespace CommandCenter.Commands.FileSystem {
    public interface IFileSystemCommandsStrategy {
        bool FileExists(string filename);
        void FileCopy(string sourceFilename, string destinationFilename);
        void FileDelete(string filename);
        void FileMove(string sourceFilename, string destinationFilename);

        bool DirectoryExists(string dirName);
        void DirectoryCopy(string sourceDirName, string destinationDirName);
        void DirectoryDelete(string dirName);
        void DirectoryDeleteContentsOnly(string sourceDirectory, Action<string, FileSystemItemType> progressCallback);
        void DirectoryMove(string sourceDir, string targetDir);
    }

    public enum FileSystemItemType { 
        File,
        Directory
    }
}
