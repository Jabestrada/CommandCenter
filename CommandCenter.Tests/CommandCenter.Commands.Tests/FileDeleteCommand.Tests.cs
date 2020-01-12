using CommandCenter.Commands;
using CommandCenter.Infrastructure;
using CommandCenter.Tests.MockCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class FileDeleteCommandTests {

        [TestMethod]
        public void itShouldSucceedIfSourceFileDoesNotExist() {
            var fileSysCommand = new MockFileSystemCommand();
            fileSysCommand.FileExistsFunc = (filename) => false;
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummysourcefile.txt", @"c:\dummybackupdir", fileSysCommand);
            var reports = new List<CommandReport>();
            fileDeleteCommand.OnReportSent += (command, args) => {
                reports.Add(new CommandReport {
                    Reporter = command,
                    Message = args.Message,
                    ReportType = args.ReportType
                });
            };

            fileDeleteCommand.Do();

            Assert.IsTrue(reports.Any(r => r.ReportType == ReportType.DoneTaskWithSuccess &&
                                           r.Reporter.Id == fileDeleteCommand.Id));
        }

        [TestMethod]
        public void itShouldFailIfBackupFails() {
            var fileSysCommand = new MockFileSystemCommand();
            fileSysCommand.FileExistsFunc = (filename) => true;
            fileSysCommand.FileCopyFunc = (sourceFile, destinationFile) => {
                throw new ApplicationException("Exception raised by MockFileSystemCommand during FileCopy");
            };
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummysourcefile.txt", @"c:\dummybackupdir", fileSysCommand);
            var reports = new List<CommandReport>();
            fileDeleteCommand.OnReportSent += (command, args) => {
                reports.Add(new CommandReport {
                    Reporter = command,
                    Message = args.Message,
                    ReportType = args.ReportType
                });
            };

            fileDeleteCommand.Do();

            Assert.IsTrue(reports.Any(r => r.ReportType == ReportType.DoneTaskWithFailure &&
                                           r.Reporter.Id == fileDeleteCommand.Id));
        }

        [TestMethod]
        public void itShouldFailIfDeleteFails() {
            var fileSysCommand = new MockFileSystemCommand();
            fileSysCommand.FileExistsFunc = (filename) => true;
            fileSysCommand.FileCopyFunc = (sourceFile, destinationFile) => {
                // Don't throw exception to signify success.
            };
            fileSysCommand.FileDeleteFunc = (filename) => {
                throw new ApplicationException("Exception raised by MockFileSystemCommand during FileDelete");
            };
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummysourcefile.txt", @"c:\dummybackupdir", fileSysCommand);
            var reports = new List<CommandReport>();
            fileDeleteCommand.OnReportSent += (command, args) => {
                reports.Add(new CommandReport {
                    Reporter = command,
                    Message = args.Message,
                    ReportType = args.ReportType
                });
            };

            fileDeleteCommand.Do();

            Assert.IsTrue(reports.Any(r => r.ReportType == ReportType.DoneTaskWithFailure &&
                                           r.Reporter.Id == fileDeleteCommand.Id));
        }

        [TestMethod]
        public void itShouldDeleteBackupFileOnCleanup() {
            var fakeFileSystem = new List<string>();
            var fileSysCommand = new MockFileSystemCommand();
            fileSysCommand.FileExistsFunc = (filename) => true;
            fileSysCommand.FileDeleteFunc = (filename) => {
                var theFile = fakeFileSystem.FirstOrDefault(f => f == filename);
                if (theFile != null) {
                    fakeFileSystem.Remove(theFile);
                }
            };

            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummysourcefile.txt", @"c:\dummybackupdir", fileSysCommand);
            var reports = new List<CommandReport>();
            fileDeleteCommand.OnReportSent += (command, args) => {
                reports.Add(new CommandReport {
                    Reporter = command,
                    Message = args.Message,
                    ReportType = args.ReportType
                });
            };


            fileDeleteCommand.Do();
            fileDeleteCommand.Cleanup();

            Assert.IsFalse(fakeFileSystem.Contains(fileDeleteCommand.BackupFilename));
        }

        [TestMethod]
        public void itShouldRestoreBackupOnUndo() {
            var fakeFileSystem = new List<string>();
            var fileSysCommand = new MockFileSystemCommand();
            fileSysCommand.FileExistsFunc = (filename) => {
                return fakeFileSystem.Contains(filename);
            };
            fileSysCommand.FileDeleteFunc = (filename) => {
                fakeFileSystem.Remove(filename);
            };
            fileSysCommand.FileCopyFunc = (sourceFile, destinationFile) => {
                fakeFileSystem.Add(destinationFile);
            };
            fileSysCommand.FileMoveFunc = (sourceFile, destinationFile) => {
                fakeFileSystem.Remove(sourceFile);
                fakeFileSystem.Add(destinationFile);
            };
            var fileToDelete = @"c:\dummysourcefile.txt";
            fakeFileSystem.Add(fileToDelete);
            var fileDeleteCommand = new FileDeleteCommand(fileToDelete, @"c:\dummybackupdir", fileSysCommand);
            var reports = new List<CommandReport>();
            fileDeleteCommand.OnReportSent += (command, args) => {
                reports.Add(new CommandReport {
                    Reporter = command,
                    Message = args.Message,
                    ReportType = args.ReportType
                });
            };

            fileDeleteCommand.Do();
            Assert.IsFalse(fakeFileSystem.Contains(fileToDelete));

            fileDeleteCommand.Undo();
            Assert.IsTrue(fakeFileSystem.Contains(fileToDelete));
        }
    }
}
