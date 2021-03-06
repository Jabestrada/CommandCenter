﻿using CommandCenter.Commands.FileSystem;
using CommandCenter.Commands.FileSystem.BaseDefinitions;
using System;
using System.Collections.Generic;

namespace CommandCenter.Tests.MockCommands {
    public class MockFileSystemCommand : IFileSystemCommandsStrategy {

        public Func<string, bool> FileExistsFunc { get; set; }
        public Func<string, bool> DirectoryExistsFunc { get; set; }

        public Action<string, string> FileCopyFunc { get; set; }
        public Action<string> FileDeleteFunc { get; set; }
        public Action<string, string> FileMoveFunc { get; set; }
        public Action<string> DirectoryDeleteFunc { get; set; }
        public Action<string, string> DirectoryCopyFunc { get; set; }
        public Action<string, string> DirectoryMoveFunc { get; set; }
        public Action<string, string> DirectoryMoveContentsFunc { get; set; }
        public Action<string, Action<string, FileSystemItemType>> DirectoryDeleteContentsOnlyFunc { get; set; }

        public Action<string, string, Func<string, string, bool>, Action<string, string>> DirectoryCopyContentsFunc { get; set; }

        public void DirectoryCopyContents(string sourceDirName, string destinationDirName, Func<string, string, bool> preCopyCallback, Action<string, string> progressCallback) {
            if (DirectoryCopyContentsFunc != null) {
                DirectoryCopyContentsFunc(sourceDirName, destinationDirName, preCopyCallback, progressCallback);
                return;
            }
            throw new NotImplementedException();
        }

        public void DirectoryDelete(string dirName) {
            if (DirectoryDeleteFunc != null) {
                DirectoryDeleteFunc(dirName);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.DirectoryDelete not set");
        }

        public void DirectoryCopy(string sourceDirName, string destinationDirName) {
            if (DirectoryCopyFunc != null) {
                DirectoryCopyFunc(sourceDirName, destinationDirName);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.DirectoryCopy not set");
        }

        public bool DirectoryExists(string dirName) {
            if (DirectoryExistsFunc != null) {
                return DirectoryExistsFunc(dirName);
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.DirectoryExists not set");

        }

        public void FileCopy(string sourceFilename, string destinationFilename) {
            if (FileCopyFunc != null) {
                FileCopyFunc(sourceFilename, destinationFilename);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.FileCopy not set");

        }

        public void FileDelete(string filename) {
            if (FileDeleteFunc != null) {
                FileDeleteFunc(filename);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.FileDelete not set");

        }

        public bool FileExists(string filename) {
            if (FileExistsFunc != null) {
                return FileExistsFunc(filename);
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.FileExists not set");

        }

        public void FileMove(string sourceFilename, string destinationFilename) {
            if (FileMoveFunc != null) {
                FileMoveFunc(sourceFilename, destinationFilename);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.FileMove not set");

        }

        public void DirectoryMove(string sourceDir, string targetDir) {
            if (DirectoryMoveFunc != null) {
                DirectoryMoveFunc(sourceDir, targetDir);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.DirectoryMove not set");
        }

        public void DirectoryDeleteContentsOnly(string sourceDirectory, Action<string, FileSystemItemType> progressCallback) {
            if (DirectoryDeleteContentsOnlyFunc != null) {
                DirectoryDeleteContentsOnlyFunc(sourceDirectory, progressCallback);
                return;
            }
            throw new NotImplementedException("MockFileSystemCommand.IFileSystemCommandsStrategy.DirectoryDeleteContentsOnly not set");
        }

        public void DirectoryMoveContents(string sourceDir, string targetDir) {
            if (DirectoryMoveContentsFunc != null) {
                DirectoryMoveContentsFunc(sourceDir, targetDir);
                return;
            }
            throw new NotImplementedException();
        }

        public IEnumerable<string> DirectoryGetFiles(string sourceDir) {
            throw new NotImplementedException();
        }

        public void FileCreate(string fileName) {
            throw new NotImplementedException();
        }
    }
}
