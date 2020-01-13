using CommandCenter.Commands.FileSystem;
using CommandCenter.Infrastructure;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
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
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToDelete = @"c:\dummysourcefile.txt";
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
            Assert.IsTrue(fileDeleteCommand.DidCommandSucceed);
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

            Assert.IsFalse(fileDeleteCommand.DidCommandSucceed);
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
            Assert.IsFalse(fileDeleteCommand.DidCommandSucceed);
            Assert.IsTrue(reports.Any(r => r.ReportType == ReportType.DoneTaskWithFailure &&
                                           r.Reporter.Id == fileDeleteCommand.Id));
        }

        [TestMethod]
        public void itShouldDeleteBackupFileOnCleanup() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            var fileToDelete = @"c:\dummysourcefile.txt";
            fakeFileSystem.AddFile(fileToDelete);
            var fileDeleteCommand = new FileDeleteCommand(fileToDelete, @"c:\dummybackupdir", fileSysCommand);

            fileDeleteCommand.Do();
            fileDeleteCommand.Cleanup();

            Assert.IsFalse(fakeFileSystem.FileExists(fileDeleteCommand.BackupFilename));
        }

        [TestMethod]
        public void itShouldRestoreBackupOnUndo() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToDelete = @"c:\dummysourcefile.txt";
            fakeFileSystem.AddFile(fileToDelete);
            var fileDeleteCommand = new FileDeleteCommand(fileToDelete, @"c:\dummybackupdir", fileSysCommand);

            fileDeleteCommand.Do();
            Assert.IsFalse(fakeFileSystem.FileExists(fileToDelete));

            fileDeleteCommand.Undo();
            Assert.IsTrue(fakeFileSystem.FileExists(fileToDelete));
        }
    }
}
