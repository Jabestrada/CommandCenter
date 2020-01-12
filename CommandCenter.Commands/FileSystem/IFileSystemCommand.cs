namespace CommandCenter.Commands.FileSystem {
    public interface IFileSystemCommand {
        bool FileExists(string filename);
        void FileCopy(string sourceFilename, string destinationFilename);
        void FileDelete(string filename);
        void FileMove(string sourceFilename, string destinationFilename);
    }
}
