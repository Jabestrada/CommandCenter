using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommandCenter.Tests.CommandCenter.Commands.Tests {
    [TestClass]
    public class FileCopyCommandTests {
        [TestMethod]
        public void itShouldCopyFileIfSuccessful() {
            //var fakeFileSystem = new List<string>();
            //var fileSysCommand = new MockFileSystemCommand();
            //fileSysCommand.FileExistsFunc = (filename) => {
            //    return fakeFileSystem.Contains(filename);
            //};
            //fileSysCommand.FileDeleteFunc = (filename) => {
            //    fakeFileSystem.Remove(filename);
            //};
            //fileSysCommand.FileCopyFunc = (sourceFile, destinationFile) => {
            //    fakeFileSystem.Add(destinationFile);
            //};
            //fileSysCommand.FileMoveFunc = (sourceFile, destinationFile) => {
            //    fakeFileSystem.Remove(sourceFile);
            //    fakeFileSystem.Add(destinationFile);
            //};
            //var fileToCopy = @"c:\dummysourcefile.txt";
            //fakeFileSystem.Add(fileToCopy);
            //var fileDeleteCommand = new FileCopyCommand(fileToCopy, @"c:\dummybackupdir", fileSysCommand);

        }
    }
}
