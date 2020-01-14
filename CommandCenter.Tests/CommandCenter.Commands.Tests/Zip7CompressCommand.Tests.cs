using CommandCenter.Commands.FileZip;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using CommandCenter.Tests.MockCommands.FileZip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class Zip7CompressCommandTests {
        [TestMethod]
        public void itShouldDeleteTargetZipOnUndoIfCommandSucceeded() {
            var zipExe = "dummy7zip.exe";
            var targetZipFile = @"dummytargetZip.7z";
            var source1 = "file1.txt";
            var source2 = "file2.txt";
            var zip7FileCompressionCommand = new Zip7CompressCommand(zipExe, targetZipFile, source1, source2);
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy(0));
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);
            fakeFileSystem.AddFile(source1);
            fakeFileSystem.AddFile(source2);
            fakeFileSystem.AddFile(zipExe);

            zip7FileCompressionCommand.Do();
            Assert.IsTrue(zip7FileCompressionCommand.DidCommandSucceed);

            fakeFileSystem.AddFile(targetZipFile);

            zip7FileCompressionCommand.Undo();
            Assert.IsFalse(fakeFileSystem.FileExists(targetZipFile));
        }

        [TestMethod]
        public void itShouldNotDeleteTargetZipOnUndoIfCommandFailed() {
            var targetZipFile = @"dummytargetZip.7z";
            var zip7FileCompressionCommand = new Zip7CompressCommand("dummy7zip.exe", targetZipFile, "file1.txt", "file2.txt");
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy(1));
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);

            zip7FileCompressionCommand.Do();
            Assert.IsFalse(zip7FileCompressionCommand.DidCommandSucceed);

            fakeFileSystem.AddFile(targetZipFile);

            zip7FileCompressionCommand.Undo();

            Assert.IsTrue(fakeFileSystem.FileExists(targetZipFile));
        }

        [TestMethod]
        public void itShouldFailIfTargetZipAlreadyExists() {
            var targetZipFile = @"dummytargetZip.7z";
            var zip7FileCompressionCommand = new Zip7CompressCommand("dummy7zip.exe", targetZipFile, "file1.txt", "file2.txt");
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy());
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);
            fakeFileSystem.AddFile(targetZipFile);

            zip7FileCompressionCommand.Do();

            Assert.IsFalse(zip7FileCompressionCommand.DidCommandSucceed);
        }


        [TestMethod]
        public void itShouldFailIfOneOrMoreSourcesDoNotExist() {
            var zipExe = "dummy7zip.exe";
            var targetZipFile = @"dummytargetZip.7z";
            var sourceFile1 = "file1.txt";
            var sourceFile2 = "file2.txt";
            var zip7FileCompressionCommand = new Zip7CompressCommand(zipExe, targetZipFile, sourceFile1, sourceFile2);
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy());
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);
            fakeFileSystem.AddFile(zipExe);

            zip7FileCompressionCommand.Do();

            Assert.IsFalse(zip7FileCompressionCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldInterpretZeroReturnValueFromFileCompressionStrategyAsSuccess() {
            var zipExe = "dummy7zip.exe";
            var targetZipFile = @"dummytargetZip.7z";
            var source1 = "file1.txt";
            var source2 = "file2.txt";
            var zip7FileCompressionCommand = new Zip7CompressCommand(zipExe, targetZipFile, source1, source2);
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy(0));
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);
            fakeFileSystem.AddFile(zipExe);
            fakeFileSystem.AddFile(source1);
            fakeFileSystem.AddFile(source2);

            zip7FileCompressionCommand.Do();

            Assert.IsTrue(zip7FileCompressionCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldInterpretNonZeroReturnValueFromFileCompressionStrategyAsFailure() {
            var targetZipFile = @"dummytargetZip.7z";
            var zip7FileCompressionCommand = new Zip7CompressCommand("dummy7zip.exe", targetZipFile, "file1.txt", "file2.txt");
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy(1));
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);

            zip7FileCompressionCommand.Do();

            Assert.IsFalse(zip7FileCompressionCommand.DidCommandSucceed);

            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy(-1));
            Assert.IsFalse(zip7FileCompressionCommand.DidCommandSucceed);

        }

        [TestMethod]
        public void itShouldFailIfZipExeIsMissing() {
            var zipExeFile = "dummy7zip.exe";
            var targetZipFile = @"dummytargetZip.7z";
            var source1 = "file1.txt";
            var zip7FileCompressionCommand = new Zip7CompressCommand(zipExeFile, targetZipFile, source1);
            zip7FileCompressionCommand.setFileCompressionStrategy(new MockFileCompressionStrategy(0));
            var mockFileSysCommand = new MockFileSystemCommand();
            zip7FileCompressionCommand.setFileSystemCommandsStrategy(mockFileSysCommand);
            var fakeFileSystem = new FakeFileSystem(mockFileSysCommand);
            fakeFileSystem.AddFile(source1);

            zip7FileCompressionCommand.Do();

            Assert.IsFalse(zip7FileCompressionCommand.DidCommandSucceed);

        }
    }
}
