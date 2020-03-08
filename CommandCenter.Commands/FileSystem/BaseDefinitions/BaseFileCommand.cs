using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.IO;
using System.Linq;

namespace CommandCenter.Commands.FileSystem.BaseDefinitions {
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

        protected bool TryReadAccessFromDirectory(string sourceDirectory) {
            FileSystemCommands.DirectoryGetFiles(sourceDirectory);
            return true;
        }

        protected bool TryWriteAccessToDirectory(string destinationDirectory) {
            int counter = 0;
            var tempFileName = Path.Combine(destinationDirectory, $"cc_test_file{counter}.tmp");
            while (FileExists(tempFileName)) {
                counter++;
                tempFileName = Path.Combine(destinationDirectory, $"cc_test_file{counter}.tmp");
            }
            FileSystemCommands.FileCreate(tempFileName);
            FileSystemCommands.FileDelete(tempFileName);
            return true;
        }

        protected bool PreflightCheckDirectoryReadWriteAccess(string inputDirectory) {
            if (!PreflightCheckReadAccessFromDirectory(inputDirectory)) return false;
            return PreflightCheckWriteAccessToDirectory(inputDirectory);
        }

        protected bool PreflightCheckReadAccessFromDirectory(string sourceDirectory) {
            try {
                TryReadAccessFromDirectory(sourceDirectory);
            }
            catch (Exception exc) {
                SendReport(this, $"{ShortName} will likely FAIL because application lack write permissions to {sourceDirectory}: {exc.Message}",
                                ReportType.DonePreFlightWithFailure);
                return false;
            }
            return true;
        }

        protected bool PreflightCheckWriteAccessToDirectory(string targetDirectory) {
            try {
                TryWriteAccessToDirectory(targetDirectory);
            }
            catch (Exception exc) {
                SendReport(this, $"{ShortName} will likely FAIL because application lack write permissions to {targetDirectory}: {exc.Message}",
                                ReportType.DonePreFlightWithFailure);
                return false;
            }
            return true;
        }
    }

    public class NullFileSystemCommandsStrategyException : ApplicationException {
        public NullFileSystemCommandsStrategyException() :
            base("FileSystemCommandsStrategy is null") {

        }
    }
}
