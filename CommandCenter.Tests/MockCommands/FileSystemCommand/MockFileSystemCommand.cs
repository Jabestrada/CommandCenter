using CommandCenter.Commands.FileSystem;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockFileSystemCommand : IFileSystemCommandsStrategy {

        public Func<string, bool> FileExistsFunc { get; set; }
        public Func<string, bool> DirectoryExistsFunc { get; set; }

        public Action<string, string> FileCopyFunc { get; set; }
        public Action<string> FileDeleteFunc { get; set; }
        public Action<string, string> FileMoveFunc { get; set; }

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


    }
}
