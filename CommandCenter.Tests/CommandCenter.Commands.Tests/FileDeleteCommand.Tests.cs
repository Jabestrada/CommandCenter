using CommandCenter.Commands.FileSystem;
using CommandCenter.Infrastructure.Orchestration;
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
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummybackupdir", fileSysCommand, fileToDelete);
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
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fileSysCommand.FileCopyFunc = (sourceFile, destinationFile) => {
                throw new ApplicationException("Exception raised by MockFileSystemCommand during FileCopy");
            };
            var source = @"c:\dummysourcefile.txt";
            fakeFileSystem.AddFiles(source);

            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummybackupdir", fileSysCommand, source);
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
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fileSysCommand.FileDeleteFunc = (filename) => {
                throw new ApplicationException("Exception raised by MockFileSystemCommand during FileDelete");
            };
            var source = @"c:\dummysourcefile.txt";
            fakeFileSystem.AddFiles(source);
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummybackupdir", fileSysCommand, source);
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
            fakeFileSystem.AddFiles(fileToDelete);
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummybackupdir", fileSysCommand, fileToDelete);

            fileDeleteCommand.Do();
            fileDeleteCommand.Cleanup();

            Assert.IsFalse(fakeFileSystem.FileExists(fileDeleteCommand.BackupFilename));
        }

        [TestMethod]
        public void itShouldRestoreBackupOnUndo() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToDelete = @"c:\dummysourcefile.txt";
            fakeFileSystem.AddFiles(fileToDelete);
            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummybackupdir", fileSysCommand, fileToDelete);

            fileDeleteCommand.Do();
            Assert.IsFalse(fakeFileSystem.FileExists(fileToDelete));

            fileDeleteCommand.Undo();
            Assert.IsTrue(fakeFileSystem.FileExists(fileToDelete));
        }

        [TestMethod]
        public void itShouldDeleteMultipleFiles() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToDelete1 = @"c:\dummysourcefile1.txt";
            var fileToDelete2 = @"c:\dummysourcefile2.txt";
            var fileToDelete3 = @"c:\dummysourcefile3.txt";
            
            fakeFileSystem.AddFiles(fileToDelete1, fileToDelete2, fileToDelete3);
            Assert.IsTrue(fakeFileSystem.FileExists(fileToDelete1));
            Assert.IsTrue(fakeFileSystem.FileExists(fileToDelete2));
            Assert.IsTrue(fakeFileSystem.FileExists(fileToDelete3));

            var fileDeleteCommand = new FileDeleteCommand(@"c:\dummybackupdir", fileSysCommand, fileToDelete1, fileToDelete2, fileToDelete3);
            fileDeleteCommand.Do();

            Assert.IsFalse(fakeFileSystem.FileExists(fileToDelete1));
            Assert.IsFalse(fakeFileSystem.FileExists(fileToDelete2));
            Assert.IsFalse(fakeFileSystem.FileExists(fileToDelete3));
        }
    }
}
