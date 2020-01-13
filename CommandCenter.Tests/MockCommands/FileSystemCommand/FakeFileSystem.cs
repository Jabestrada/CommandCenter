using CommandCenter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Tests.MockCommands.FileSystemCommand {
    public class FakeFileSystem {
        private List<string> _files = new List<string>();

        public FakeFileSystem(MockFileSystemCommand fileSysCommand) {
            registerMockFileSystemCommand(fileSysCommand);
        }
        private void registerMockFileSystemCommand(MockFileSystemCommand fileSysCommand) {
            fileSysCommand.FileExistsFunc = (filename) => {
                return _files.Contains(filename);
            };
            fileSysCommand.FileDeleteFunc = (filename) => {
                _files.Remove(filename);
            };
            fileSysCommand.FileCopyFunc = (sourceFile, destinationFile) => {
                _files.Add(destinationFile);
            };
            fileSysCommand.FileMoveFunc = (sourceFile, destinationFile) => {
                _files.Remove(sourceFile);
                _files.Add(destinationFile);
            };
        }

        public bool FileExists(string filename) {
            return _files.Contains(filename);
        }
        public void AddFile(string filename) {
            _files.Add(filename);
        }
    }
}
