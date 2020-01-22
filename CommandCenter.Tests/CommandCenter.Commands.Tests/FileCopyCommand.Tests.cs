using CommandCenter.Commands.FileSystem;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class FileCopyCommandTests {
        [TestMethod]
        public void itShouldHaveCopiedFileIfSuccessful() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            var fileToCopy = @"c:\dummysourcefile.txt";
            var targetFileCopy = @"c:\somefolder\dummysourcefile.doc";
            fakeFileSystem.AddFiles(fileToCopy);
            var fileCopyCommand = new FileCopyCommand(fileToCopy, targetFileCopy, @"c:\dummybackupdir", fileSysCommand);

            fileCopyCommand.Do();

            Assert.IsTrue(fileCopyCommand.DidCommandSucceed);
            Assert.IsTrue(fakeFileSystem.FileExists(targetFileCopy));
        }

        [TestMethod]
        public void itShouldFailIfSourceFileDoesNotExist() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToCopy = @"c:\dummysourcefile.txt";
            var targetFileCopy = @"c:\somefolder\dummysourcefile.doc";
            var fileCopyCommand = new FileCopyCommand(fileToCopy, targetFileCopy, @"c:\dummybackupdir", fileSysCommand);

            fileCopyCommand.Do();

            Assert.IsFalse(fileCopyCommand.DidCommandSucceed);
            Assert.IsFalse(fakeFileSystem.FileExists(targetFileCopy));
        }

        [TestMethod]
        public void itShouldDeleteCopiedFileOnUndoIfCommandSucceeded() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToCopy = @"c:\dummysourcefile.txt";
            var targetFileCopy = @"c:\somefolder\dummysourcefile.doc";
            fakeFileSystem.AddFiles(fileToCopy);
            var fileCopyCommand = new FileCopyCommand(fileToCopy, targetFileCopy, @"c:\dummybackupdir", fileSysCommand);

            fileCopyCommand.Do();

            Assert.IsTrue(fileCopyCommand.DidCommandSucceed);

            fileCopyCommand.Undo();

            Assert.IsTrue(!fakeFileSystem.FileExists(targetFileCopy));
        }

        [TestMethod]
        public void itShouldNotDeleteCopiedFileOnUndoIfCommandFailed() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToCopy = @"c:\dummysourcefile.txt";
            var targetFileCopy = @"c:\somefolder\dummysourcefile.doc";
            fakeFileSystem.AddFiles(fileToCopy);
            fakeFileSystem.AddFiles(targetFileCopy);
            var fileCopyCommand = new FileCopyCommand(fileToCopy, targetFileCopy, @"c:\dummybackupdir", fileSysCommand);

            fileCopyCommand.Do();

            Assert.IsFalse(fileCopyCommand.DidCommandSucceed);

            fileCopyCommand.Undo();

            Assert.IsTrue(fakeFileSystem.FileExists(targetFileCopy));
        }

        [TestMethod]
        public void itShouldFailIfDestinationFileAlreadyExists() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToCopy = @"c:\dummysourcefile.txt";
            var targetFileCopy = @"c:\somefolder\dummysourcefile.doc";
            fakeFileSystem.AddFiles(fileToCopy);
            fakeFileSystem.AddFiles(targetFileCopy);
            var fileCopyCommand = new FileCopyCommand(fileToCopy, targetFileCopy, @"c:\dummybackupdir", fileSysCommand);

            fileCopyCommand.Do();

            Assert.IsFalse(fileCopyCommand.DidCommandSucceed);
        }
    }
}
