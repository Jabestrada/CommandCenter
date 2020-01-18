using CommandCenter.Commands.FileSystem;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class DirectoryDeleteCommandTests {

        [TestMethod]
        public void itShouldSucceedIfSourceDiretoryDoesNotExist() {
            var fileSysCommand = new MockFileSystemCommand();
            var dirToDelete = @"c:\dummyDir";
            var dirDeleteCommand = new DirectoryDeleteCommand(dirToDelete, @"c:\dummybackupdir", fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            dirDeleteCommand.Do();
            Assert.IsTrue(dirDeleteCommand.DidCommandSucceed);

        }

        [TestMethod]
        public void itShouldFailIfBackupFails() {
            var fileSysCommand = new MockFileSystemCommand();
            var sourceDir = @"c:\dummysourceDir";
            var dirDeleteCommand = new DirectoryDeleteCommand(sourceDir, @"c:\dummybackupdir", fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fileSysCommand.DirectoryCopyFunc = (sourceFile, destinationFile) => {
                throw new ApplicationException("Exception raised by MockFileSystemCommand during DirectoryCopy");
            };
            fakeFileSystem.AddDirectory(sourceDir);

            dirDeleteCommand.Do();

            Assert.IsFalse(dirDeleteCommand.DidCommandSucceed);
            Assert.IsTrue(fakeFileSystem.DirectoryExists(sourceDir));
        }

        [TestMethod]
        public void itShouldFailIfDeleteFails() {
            var fileSysCommand = new MockFileSystemCommand();
            var sourceDir = @"c:\dummysourcefile";
            var dirDeleteCommand = new DirectoryDeleteCommand(sourceDir, @"c:\dummybackupdir", fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fileSysCommand.DirectoryDeleteFunc = (filename) => {
                throw new ApplicationException("Exception raised by MockFileSystemCommand during FileDelete");
            };
            fakeFileSystem.AddDirectory(sourceDir);

            dirDeleteCommand.Do();
            Assert.IsFalse(dirDeleteCommand.DidCommandSucceed);
            Assert.IsTrue(fakeFileSystem.DirectoryExists(sourceDir));
        }

        [TestMethod]
        public void itShouldDeleteBackupFileOnCleanup() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            var dirToDelete = @"c:\main1\main2\dummysourceDir";
            fakeFileSystem.AddDirectory(dirToDelete);
            var dirDeleteCommand = new DirectoryDeleteCommand(dirToDelete, @"c:\dummybackupdir", fileSysCommand);

            dirDeleteCommand.Do();
            dirDeleteCommand.Cleanup();

            Assert.IsFalse(fakeFileSystem.DirectoryExists(dirDeleteCommand.BackedUpDirectory));
        }

        [TestMethod]
        public void itShouldRestoreBackupOnUndo() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var dirToDelete = @"c:\mainFolder\dummysourcedir";
            fakeFileSystem.AddDirectory(dirToDelete);
            var dirDeleteCommand = new DirectoryDeleteCommand(dirToDelete, @"c:\dummybackupdir", fileSysCommand);

            dirDeleteCommand.Do();
            Assert.IsFalse(fakeFileSystem.DirectoryExists(dirToDelete));

            dirDeleteCommand.Undo();
            Assert.IsTrue(fakeFileSystem.DirectoryExists(dirToDelete));
        }
    }
}
