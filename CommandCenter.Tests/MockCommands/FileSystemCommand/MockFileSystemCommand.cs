using CommandCenter.Commands.FileSystem;
using System;

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

        public void DirectoryDelete(string dirName) {
            if (DirectoryDeleteFunc != null) {
                DirectoryDeleteFunc(dirName);
                return;
            }
            throw new NotImplementedException();
        }

        public void DirectoryCopy(string sourceDirName, string destinationDirName) {
            if (DirectoryCopyFunc != null) {
                DirectoryCopyFunc(sourceDirName, destinationDirName);
                return;
            }
            throw new NotImplementedException();
        }
        public bool DirectoryExists(string dirName) {
            if (DirectoryExistsFunc != null) {
                return DirectoryExistsFunc(dirName);
            }
            throw new NotImplementedException();
        }

        public void FileCopy(string sourceFilename, string destinationFilename) {
            if (FileCopyFunc != null) {
                FileCopyFunc(sourceFilename, destinationFilename);
                return;
            }
            throw new NotImplementedException();
        }

        public void FileDelete(string filename) {
            if (FileDeleteFunc != null) {
                FileDeleteFunc(filename);
                return;
            }
            throw new NotImplementedException();
        }

        public bool FileExists(string filename) {
            if (FileExistsFunc != null) {
                return FileExistsFunc(filename);
            }
            throw new NotImplementedException();
        }

        public void FileMove(string sourceFilename, string destinationFilename) {
            if (FileMoveFunc != null) {
                FileMoveFunc(sourceFilename, destinationFilename);
                return;
            }
            throw new NotImplementedException();
        }

        public void DirectoryMove(string sourceDir, string targetDir) {
            if (DirectoryMoveFunc != null) {
                DirectoryMoveFunc(sourceDir, targetDir);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
