using CommandCenter.Commands.FileSystem;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class DirectoryCopyContentsCommandTests {
        [TestMethod]
        public void itShouldFailIfSourceDirectoryDoesNotExist() {
            var fileSysCommand = new MockFileSystemCommand();
            var sourceDir = @"c:\sourceDir";
            var targetDir = @"c:\targetDir";
            var dirCopyContentsCommand = new DirectoryCopyContentsCommand(sourceDir, targetDir, fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            dirCopyContentsCommand.Do();

            Assert.IsFalse(dirCopyContentsCommand.DidCommandSucceed);
        }


        [TestMethod]
        public void itShouldFailIfAnyOfTheTargetFilesAlreadyExist() {
            var fileSysCommand = new MockFileSystemCommand();
            var sourceDir = @"c:\sourceDir";
            var sourceDirFile1 = @"c:\sourceDir\file1.txt";
            var targetDir = @"c:\targetDir";
            var targetDirFile1 = @"c:\targetDir\file1.txt";
            var dirCopyContentsCommand = new DirectoryCopyContentsCommand(sourceDir, targetDir, fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddDirectory(sourceDir);
            fakeFileSystem.AddFiles(sourceDirFile1);
            fakeFileSystem.AddDirectory(targetDir);
            fakeFileSystem.AddFiles(targetDirFile1);

            dirCopyContentsCommand.Do();

            Assert.IsFalse(dirCopyContentsCommand.DidCommandSucceed);
        }

        [TestMethod]
        public void itShouldDeleteOnlyCopiedFilesOnUndo() {
            var fileSysCommand = new MockFileSystemCommand();
            var sourceDir = @"c:\sourceDir";
            var sourceDirFile1 = @"c:\sourceDir\file1.txt";
            var sourceDirFile2 = @"c:\sourceDir\file2.txt";

            var targetDir = @"c:\targetDir";
            var targetDirFile1 = @"c:\targetDir\file2.txt";
            var dirCopyContentsCommand = new DirectoryCopyContentsCommand(sourceDir, targetDir, fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);
            fakeFileSystem.AddDirectory(sourceDir);
            fakeFileSystem.AddFiles(sourceDirFile1);
            fakeFileSystem.AddFiles(sourceDirFile2);

            fakeFileSystem.AddDirectory(targetDir);
            fakeFileSystem.AddFiles(targetDirFile1);

            dirCopyContentsCommand.Do();

            Assert.IsFalse(dirCopyContentsCommand.DidCommandSucceed);
            Assert.IsTrue(fakeFileSystem.FileExists(targetDirFile1));

            dirCopyContentsCommand.Undo();

            Assert.IsFalse(fakeFileSystem.FileExists(@"c:\targetDir\file1.txt"));
            Assert.IsTrue(fakeFileSystem.FileExists(targetDirFile1));
        }

    }
}
