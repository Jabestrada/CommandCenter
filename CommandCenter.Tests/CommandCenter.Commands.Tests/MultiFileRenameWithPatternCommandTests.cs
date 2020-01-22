using CommandCenter.Commands.FileSystem;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class MultiFileRenameWithPatternCommandTests {
        [TestMethod]
        public void itShouldFailIfOneOfTheTargetFilesAlreadyExists() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            var inputFiles = new string[] { @"C:\someDir\someFile.txt" };
            string pattern = "preText-[n]-postText.txt";
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFiles);
            var outputFile = @"C:\someDir\preText-someFile-postText.txt";

            fakeFileSystem.AddFiles(inputFiles);
            fakeFileSystem.AddFiles(outputFile);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldFailIfOneOfTheSourceFilesDoesNotExist() {
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            var inputFiles = new string[] { @"C:\someDir\someFile.txt", @"C:\someDir\someFile2.txt" };
            string pattern = "preText-[n]-postText.txt";
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFiles);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldRenameUsingFilenameToken() {
            string inputFile1 = @"C:\someDir\someFile1.txt";
            string inputFile2 = @"C:\someDir\someFile2.txt";

            string pattern = "preText-[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile1, inputFile2);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile1, inputFile2);

            renameFileCommand.Do();

            var outputFile1 = @"C:\someDir\preText-someFile1-postText.txt";
            var outputFile2 = @"C:\someDir\preText-someFile2-postText.txt";

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);

            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile1], outputFile1);
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile2], outputFile2);

            Assert.IsTrue(fakeFileSystem.FileExists(outputFile1));
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile2));
        }

        [TestMethod]
        public void itShouldRenameUsingMultipleFilenameTokens() {
            string inputFile1 = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText-[n].txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile1);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile1);

            renameFileCommand.Do();

            var targetFile = @"C:\someDir\preText-someFile-postText-someFile.txt";
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile1], targetFile);
            Assert.IsTrue(fakeFileSystem.FileExists(targetFile));
        }

        [TestMethod]
        public void itShouldRenameUsingDateTimeToken() {
            string inputFile1 = @"C:\someDir\someFile1.txt";
            string inputFile2 = @"C:\someDir\someFile2.txt";

            string pattern = "preText-[n]-[d:MMdd]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile1, inputFile2);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile1, inputFile2);

            renameFileCommand.Do();

            var computedReplacement = renameFileCommand.DateTimeReference.ToString("MMdd");
            var targetFile1 = $@"C:\someDir\preText-someFile1-{computedReplacement}-postText.txt";
            var targetFile2 = $@"C:\someDir\preText-someFile2-{computedReplacement}-postText.txt";

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile1], targetFile1);
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile2], targetFile2);

            Assert.IsTrue(fakeFileSystem.FileExists(targetFile1));
            Assert.IsTrue(fakeFileSystem.FileExists(targetFile2));
        }

        [TestMethod]
        public void itShouldRenameUsingMultipleDateTimeTokens() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[d:MMdd]-postText-[d:yyyy].txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            renameFileCommand.Do();

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            var computedReplacement1 = renameFileCommand.DateTimeReference.ToString("MMdd");
            var computedReplacement2 = renameFileCommand.DateTimeReference.ToString("yyyy");
            var outputFile = $@"C:\someDir\preText-{computedReplacement1}-postText-{computedReplacement2}.txt";
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile], outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldRenameUsingFilenameAndDateTimeToken() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText-[d:MMdd].txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            renameFileCommand.Do();

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            var computedReplacement = renameFileCommand.DateTimeReference.ToString("MMdd");
            var outputFile = $@"C:\someDir\preText-someFile-postText-{computedReplacement}.txt";
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile], outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldFailIfReplacementTokenIsUnrecognized() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[X:abcd]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldFailIfTokenClosingBracketsAreMismatched() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldFailIfTokenOpeningBracketsAreMismatched() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldSucceedWhenThereAreNoReplacementTokens() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "newFileName.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            fileRenameCommand.Do();

            Assert.IsTrue(fileRenameCommand.DidCommandSucceed);
            var outputFile = @"C:\someDir\newFileName.txt";
            Assert.AreEqual(fileRenameCommand.RenamedFiles[inputFile], outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldRevertNameOnUndo() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "preText-[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            fileRenameCommand.Do();

            Assert.IsTrue(fileRenameCommand.DidCommandSucceed);
            var outputFile = @"C:\someDir\preText-someFile-postText.txt";
            Assert.AreEqual(fileRenameCommand.RenamedFiles[inputFile], outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
            Assert.IsFalse(fakeFileSystem.FileExists(inputFile));

            fileRenameCommand.Undo();

            Assert.IsFalse(fakeFileSystem.FileExists(outputFile));
            Assert.IsTrue(fakeFileSystem.FileExists(inputFile));
        }

        [TestMethod]
        public void itShouldRevertNameOnUndoButOnlyForThoseThatWereRenamed() {
            string inputFile1 = @"C:\someDir\someFile1.txt";
            string inputFile2 = @"C:\someDir\someFile2.txt";
            string inputFile3 = @"C:\someDir\someFile3.txt";

            string pattern = "preText-[n]-postText.txt";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile1, inputFile2, inputFile3);
            var fileRenameCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile1, inputFile2,  inputFile3);
            var outputFile1 = @"C:\someDir\preText-someFile1-postText.txt";
            var outputFile2 = @"C:\someDir\preText-someFile2-postText.txt";
            var outputFile3 = @"C:\someDir\preText-someFile3-postText.txt";
            fakeFileSystem.AddFiles(outputFile3);

            fileRenameCommand.Do();

            Assert.IsFalse(fileRenameCommand.DidCommandSucceed);

            fileRenameCommand.Undo();

            Assert.IsFalse(fakeFileSystem.FileExists(outputFile1));
            Assert.IsFalse(fakeFileSystem.FileExists(outputFile2));
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile3));
        }

        [TestMethod]
        public void itShouldRenameUsingFileExtensionToken() {
            string inputFile = @"C:\someDir\someFile.txt";
            string pattern = "renamed-[n][e]";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            renameFileCommand.Do();

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            var outputFile = $@"C:\someDir\renamed-someFile.txt";
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile], outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

        [TestMethod]
        public void itShouldRenameUsingFileExtensionTokenEvenIfSourceHasNoExtension() {
            string inputFile = @"C:\someDir\someFileNoExt";
            string pattern = "renamed-[n][e]";
            var fileSysCommand = new MockFileSystemCommand();
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddFiles(inputFile);
            var renameFileCommand = new MultiFileRenameWithPatternCommand(pattern, fileSysCommand, inputFile);

            renameFileCommand.Do();

            Assert.IsTrue(renameFileCommand.DidCommandSucceed);
            var outputFile = $@"C:\someDir\renamed-someFileNoExt";
            Assert.AreEqual(renameFileCommand.RenamedFiles[inputFile], outputFile);
            Assert.IsTrue(fakeFileSystem.FileExists(outputFile));
        }

    }
}
