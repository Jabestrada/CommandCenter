using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Commands.FileSystem {
    public class FileCopyCommand : BaseFileCommand {
        public string TargetFilename { get; protected set; }
        public FileCopyCommand(string sourceFilename, string targetFilename, string backupDir, IFileSystemCommandsStrategy fileSystemCommandsStrategy)
                : this(sourceFilename, targetFilename, backupDir) {
            FileSystemCommands = fileSystemCommandsStrategy;
        }
        public FileCopyCommand(string sourceFilename, string targetFilename, string backupDir) {
            SourceFilename = sourceFilename;
            TargetFilename = targetFilename;
            BackupFolder = backupDir;
        }

        public override bool IsUndoable => true;

        public override void Do() {
            if (!sourceFileExists() || destinationFileExists()) return;

            try {
                FileCopy(SourceFilename, TargetFilename);
                SendReport($"Copied file {SourceFilename} to {TargetFilename}", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
            }
        }

        private bool destinationFileExists() {
            if (FileExists(TargetFilename)) {
                SendReport("FileCopyCommand failed because destination file {SourceFilename} already exists", ReportType.DoneTaskWithFailure);
                DidCommandSucceed = false;
                return true;
            }
            return false;
        }

        private bool sourceFileExists() {
            if (!FileExists(SourceFilename)) {
                SendReport("FileCopyCommand failed because source file {SourceFilename} does not exist", ReportType.DoneTaskWithFailure);
                DidCommandSucceed = false;
                return false;
            }
            return true;
        }

        public override void Undo() {
            if (DidCommandSucceed) {
                FileDelete(TargetFilename);
                SendReport($"Task undone by deleting file {TargetFilename}", ReportType.DoneCleanupWithSuccess);
                return;
            }
        }

        public override void Cleanup() {
            // Nothing to do for cleanup
        }
    }
}
