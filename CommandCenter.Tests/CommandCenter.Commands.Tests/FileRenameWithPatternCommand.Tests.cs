using CommandCenter.Commands.FileSystem;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class FileRenameWithPatternCommandTests {
        [TestMethod]
        public void itShouldFailIfTargetFileAlreadyExists() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText.txt";
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);
            var outputFile = @"C:\someDir\preText-someFile-postText.txt";

            fakeFileSystem.AddFile(inputFile);
            fakeFileSystem.AddFile(outputFile);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldFailIfSourceFileDoesNotExist() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText.txt";
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldRenameUsingFilenameToken() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var renameFileCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            renameFileCommand.Do();

            var outputFile = @"C:\someDir\preText-someFile-postText.txt";
            Assert.AreEqual(renameFileCommand.ComputedNewName, outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldRenameUsingMultipleFilenameTokens() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText-[n].txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var renameFileCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            renameFileCommand.Do();

            var targetFile = @"C:\someDir\preText-someFile-postText-someFile.txt";
            Assert.AreEqual(renameFileCommand.ComputedNewName, targetFile);
            Assert.IsTrue(fakeFileSystem.FileExists(targetFile));
        }

        [TestMethod]
        public void itShouldRenameUsingDateTimeToken() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[d:MMdd]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var renameFileCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            renameFileCommand.Do();

            var computedReplacement = renameFileCommand.DateTimeReference.ToString("MMdd");
            var targetFile = $@"C:\someDir\preText-{computedReplacement}-postText.txt";
            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            Assert.AreEqual(renameFileCommand.ComputedNewName, targetFile);
            Assert.IsTrue(fakeFileSystem.FileExists(targetFile));
        }

        [TestMethod]
        public void itShouldRenameUsingMultipleDateTimeTokens() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[d:MMdd]-postText-[d:yyyy].txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var renameFileCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            renameFileCommand.Do();

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            var computedReplacement1 = renameFileCommand.DateTimeReference.ToString("MMdd");
            var computedReplacement2 = renameFileCommand.DateTimeReference.ToString("yyyy");
            var outputFile = $@"C:\someDir\preText-{computedReplacement1}-postText-{computedReplacement2}.txt";
            Assert.AreEqual(renameFileCommand.ComputedNewName, outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldRenameUsingFilenameAndDateTimeToken() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText-[d:MMdd].txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var renameFileCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            renameFileCommand.Do();

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            var computedReplacement = renameFileCommand.DateTimeReference.ToString("MMdd");
            var outputFile = $@"C:\someDir\preText-someFile-postText-{computedReplacement}.txt";
            Assert.AreEqual(renameFileCommand.ComputedNewName, outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldFailIfReplacementTokenIsUnrecognized() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[X:abcd]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldFailIfTokenClosingBracketsAreMismatched() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldFailIfTokenOpeningBracketsAreMismatched() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldSucceedWhenThereAreNoReplacementTokens() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "newFileName.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            fileRenameCommand.Do();

            Assert.IsTrue(fileRenameCommand.DidCommandSucceed);
            var outputFile = @"C:\someDir\newFileName.txt";
            Assert.AreEqual(fileRenameCommand.ComputedNewName, outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldRevertNameOnUndo() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);

            fileRenameCommand.Do();

            Assert.IsTrue(fileRenameCommand.DidCommandSucceed);
            var outputFile = @"C:\someDir\preText-someFile-postText.txt";
            Assert.AreEqual(fileRenameCommand.ComputedNewName, outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
            Assert.IsFalse(fakeFileSystem.FileExists(inputFile));

            fileRenameCommand.Undo();

            Assert.IsFalse(fakeFileSystem.FileExists(outputFile));
            Assert.IsTrue(fakeFileSystem.FileExists(inputFile));
        }

        [TestMethod]
        public void itShouldRevertNameOnUndoButOnlyIfCommandSucceeded() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFile(inputFile);
            var fileRenameCommand = new FileRenameWithPatternCommand(inputFile, pattern, fileSysCommand);
            var outputFile = @"C:\someDir\preText-someFile-postText.txt";
            fakeFileSystem.AddFile(outputFile);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);

            fileRenameCommand.Undo();

            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));

        }
    }
}
