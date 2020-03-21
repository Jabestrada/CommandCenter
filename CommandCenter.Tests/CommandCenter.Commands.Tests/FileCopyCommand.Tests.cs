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
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy, targetFileCopy);

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
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy, targetFileCopy);

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
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy, targetFileCopy);

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
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy, targetFileCopy);

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
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy, targetFileCopy);

            fileCopyCommand.Do();

            Assert.IsFalse(fileCopyCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldAllowMultipleFileCopyPairs() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToCopy1 = @"c:\dummysourcefile.txt";
            var targetFileCopy1 = @"c:\somefolder\dummytargetfile.doc";
            var fileToCopy2 = @"c:\dummysourcefile2.txt";
            var targetFileCopy2 = @"c:\somefolder\dummytargetfile2.doc";

            fakeFileSystem.AddFiles(fileToCopy1, fileToCopy2);
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy1, targetFileCopy1, fileToCopy2, targetFileCopy2);


            Assert.IsTrue(!fakeFileSystem.FileExists(targetFileCopy1));
            Assert.IsTrue(!fakeFileSystem.FileExists(targetFileCopy2));

            fileCopyCommand.Do();

            Assert.IsTrue(fakeFileSystem.FileExists(targetFileCopy1));
            Assert.IsTrue(fakeFileSystem.FileExists(targetFileCopy2));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void itShouldFailIfFileCopyPairCountIsOdd() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            var fileToCopy1 = @"c:\dummysourcefile.txt";
            var targetFileCopy1 = @"c:\somefolder\dummytargetfile.doc";
            var fileToCopy2 = @"c:\dummysourcefile2.txt";

            fakeFileSystem.AddFiles(fileToCopy1, fileToCopy2);
            var fileCopyCommand = new FileCopyCommand(@"c:\dummybackupdir", fileSysCommand, fileToCopy1, targetFileCopy1, fileToCopy2);

            fileCopyCommand.Do();

        }
    }
}
