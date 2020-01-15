using CommandCenter.Commands.FileSystem;
using CommandCenter.Tests.MockCommands;
using CommandCenter.Tests.MockCommands.FileSystemCommand;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandCenter.Tests.Commands {
    [TestClass]
    public class DirectoryDeleteContentsOnlyTests {

        [TestMethod]
        public void itMustDeleteDirectoryContentsOnly() {
            var fileSysCommand = new MockFileSystemCommand();
            var dirToDeleteContents = @"c:\maindir\subdir";
            var dirDeleteDirContentsOnlyCommand = new DirectoryDeleteContentsOnlyCommand(dirToDeleteContents, @"c:\dummybackupdir", fileSysCommand);
            var fakeFileSystem = new FakeFileSystem(fileSysCommand);

            fakeFileSystem.AddDirectory(dirToDeleteContents);
            var file1 = dirToDeleteContents +  @"\dummyfile1.txt";
            var file2 = dirToDeleteContents +  @"\dummyfile2.txt";

            fakeFileSystem.AddFile(file1);
            fakeFileSystem.AddFile(file2);

            dirDeleteDirContentsOnlyCommand.Do();

            Assert.IsTrue(dirDeleteDirContentsOnlyCommand.DidCommandSucceed);
            Assert.IsTrue(fakeFileSystem.DirectoryExists(dirToDeleteContents));
            Assert.IsFalse(fakeFileSystem.FileExists(file1));
            Assert.IsFalse(fakeFileSystem.FileExists(file2));
        }
    }
}
