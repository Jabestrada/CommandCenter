namespace CommandCenter.Commands.FileSystem {
    public interface IFileSystemCommandsStrategy {
        bool FileExists(string filename);

        bool DirectoryExists(string dirName);
        void FileCopy(string sourceFilename, string destinationFilename);
        void FileDelete(string filename);
        void FileMove(string sourceFilename, string destinationFilename);

    }
}
