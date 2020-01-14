using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class FileSystemCommands : IFileSystemCommandsStrategy {
        public void FileCopy(string source, string destination) {
            File.Copy(source, destination);
        }

        public void FileDelete(string filename) {
            File.Delete(filename);
        }

        public bool FileExists(string filename) {
            return File.Exists(filename);
        }

        public void FileMove(string sourceFilename, string destinationFilename) {
            File.Move(sourceFilename, sourceFilename);
        }

        public bool DirectoryExists(string dirName) {
            return Directory.Exists(dirName);
        }
    }
}
