using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandCenter.Tests.MockCommands.FileSystemCommand {
    public class FakeFileSystem {
        private List<string> _files = new List<string>();
        private List<string> _dirs = new List<string>();
        public FakeFileSystem(MockFileSystemCommand fileSysCommand) {
            registerMockFileSystemCommand(fileSysCommand);
        }

        private void registerMockFileSystemCommand(MockFileSystemCommand fileSysCommand) {
            fileSysCommand.DirectoryCopyContentsFunc = (sourceDir, targetDir, preCopyCallback, postCopyCallback) => {
                //var targetFiles = _files.Where(f => f.StartsWith(targetDir) && f != targetDir);
                var sourceFiles = _files.Where(f => f.StartsWith(sourceDir) && f != sourceDir).ToList();
                foreach (var sourceFile in sourceFiles) {
                    var targetFile = Path.Combine(targetDir, Path.GetFileName(sourceFile));
                    if (preCopyCallback != null) {
                        if (!preCopyCallback(sourceDir, targetFile)) return;
                    }
                    
                    _files.Add(targetFile);

                    if (postCopyCallback != null) postCopyCallback(sourceDir, targetFile);
                }
            };

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
            fileSysCommand.DirectoryExistsFunc = (dirName) => {
                return _dirs.Contains(dirName);
            };
            fileSysCommand.DirectoryCopyFunc = (sourceDir, targetDir) => {
                _dirs.Add(targetDir);
            };
            fileSysCommand.DirectoryDeleteFunc = (dir) => {
                _dirs.Remove(dir);
            };
            fileSysCommand.DirectoryMoveFunc = (sourceDir, targetDir) => {
                _dirs.Remove(sourceDir);
                _dirs.Add(targetDir);
            };
            fileSysCommand.DirectoryDeleteContentsOnlyFunc = (sourceDir, progresCallback) => {
                var removeList = new List<string>();

                foreach (var file in _files) {
                    if (file.StartsWith(sourceDir)) {
                        removeList.Add(file);
                    }
                }
                foreach (var item in removeList) {
                    _files.Remove(item);
                }

                removeList.Clear();
                foreach (var dir in _dirs) {
                    if (dir != sourceDir && dir.StartsWith(sourceDir)) {
                        removeList.Add(sourceDir);
                    }
                }

                foreach (var item in removeList) {
                    _dirs.Remove(item);
                }
            };



            fileSysCommand.DirectoryMoveContentsFunc = (sourceDir, targetDir) => {
                var removeList = new List<string>();
                var addList = new List<string>();

                foreach (var file in _files) {
                    if (file.StartsWith(sourceDir)) {
                        removeList.Add(file);
                        addList.Add(file.Replace(sourceDir, targetDir));
                    }

                }

                foreach (var item in removeList) {
                    _files.Remove(item);
                }
                foreach (var item in addList) {
                    _files.Add(item);
                }

                removeList.Clear();
                addList.Clear();
                foreach (var dir in _dirs) {
                    if (dir != sourceDir && dir.StartsWith(sourceDir)) {
                        removeList.Add(sourceDir);
                        addList.Add(sourceDir.Replace(sourceDir, targetDir));
                    }
                }

                foreach (var item in removeList) {
                    _dirs.Remove(item);
                }

                foreach (var item in addList) {
                    _dirs.Add(item);
                }

            };
        }

        public bool FileExists(string filename) {
            return _files.Contains(filename);
        }
        public void AddFiles(params string[] filenames) {
            foreach (var file in filenames) { 
               _files.Add(file);
            }
        }

   

        public void AddDirectory(string dirName) {
            _dirs.Add(dirName);
        }

        public bool DirectoryExists(string dirName) {
            return _dirs.Contains(dirName);
        }
    }
}
