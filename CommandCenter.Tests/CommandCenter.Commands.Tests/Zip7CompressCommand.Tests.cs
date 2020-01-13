using CommandCenter.Commands.FileZip;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using CommandCenter.Tests.MockCommands.FileZip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class Zip7CompressCommandTests {
        [TestMethod]
        public void itShouldDeleteTargetZipOnUndoIfCommandSucceeded() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldNotDeleteTargetZipOnUndoIfCommandFailed() {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldInterpretZeroReturnValueFromFileCompressionStrategyAsSuccess() {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void itShouldInterpretNonZeroReturnValueFromFileCompressionStrategyAsFailure() {
            throw new NotImplementedException();
        }
    }
}
