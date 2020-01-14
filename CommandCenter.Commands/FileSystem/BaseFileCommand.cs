using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Commands.FileSystem {
    public abstract class BaseFileCommand : BaseCommand {
        protected IFileSystemCommandsStrategy FileSystemCommands { get; set; }

        public string BackupFolder { get; protected set; }
        public string BackupFilename { get; protected set; }
        public string SourceFilename { get; protected set; }

        public BaseFileCommand() {
            FileSystemCommands = new FileSystemCommands();
        }

        public bool FileExists(string filename) {
            if (FileSystemCommands != null) {
                return FileSystemCommands.FileExists(filename);
            }
            throw new NullFileSystemCommandsStrategyException();
        }

        public void FileDelete(string filename) {
            if (FileSystemCommands != null) {
                FileSystemCommands.FileDelete(filename);
                return;
            }
            throw new NullFileSystemCommandsStrategyException();
        }

        public void FileMove(string sourceFile, string destinationFile) {
            if (FileSystemCommands != null) {
                FileSystemCommands.FileMove(sourceFile, destinationFile);
                return;
            }
            throw new NullFileSystemCommandsStrategyException();
        }

        public void FileCopy(string sourceFile, string destinationFile) {
            if (FileSystemCommands != null) {
                FileSystemCommands.FileCopy(sourceFile, destinationFile);
                return;
            }
            throw new NullFileSystemCommandsStrategyException();
        }
    }

    public class NullFileSystemCommandsStrategyException : ApplicationException {
        public NullFileSystemCommandsStrategyException() :
            base("FileSystemCommandsStrategy is null") {

        }
    }
}
