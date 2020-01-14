using CommandCenter.Infrastructure;
using System;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class DirectoryDeleteCommand : BaseCommand {
        public string BackedUpDirectory { get; protected set; }
        public string SourceDirectory { get; protected set; }
        private string BackupDirectory;

        private IFileSystemCommandsStrategy _fileSysCommand = new FileSystemCommands();

        public DirectoryDeleteCommand(string dirToDelete, string backupDir, IFileSystemCommandsStrategy fileSysCommand) 
            : this(dirToDelete, backupDir) {
            _fileSysCommand = fileSysCommand;
        }

        public DirectoryDeleteCommand(string dirToDelete, string backupDir) {
            SourceDirectory = dirToDelete;
            BackupDirectory = backupDir;
        }

        public override bool IsUndoable => true;

        public override void Do() {
            if (!sourceDirExists() || !createBackup()) return;

            deleteDirectory();
        }

        private bool createBackup() {
            BackedUpDirectory = Path.Combine(BackupDirectory, $"{new DirectoryInfo(SourceDirectory).Name}.backup.{Id}");
            try {
                _fileSysCommand.DirectoryCopy(SourceDirectory, BackedUpDirectory);
                SendReport($"Created backup of folder {SourceDirectory} to {BackedUpDirectory}", ReportType.Progress);
                return true;
            }
            catch (Exception exc) {
                SendReport($"Directory delete aborted because attempt to backup folder {SourceDirectory} to {BackedUpDirectory} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                return false;
            }
        }

        private void deleteDirectory() {
            try {
                SendReport($"Deleting directory {SourceDirectory} ...", ReportType.Progress);
                _fileSysCommand.DirectoryDelete(SourceDirectory);
                SendReport($"Directory {SourceDirectory} deleted", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"Failed to delete directory {SourceDirectory}. {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            if (DidCommandSucceed) {
                try {
                    _fileSysCommand.DirectoryMove(BackedUpDirectory, SourceDirectory);
                    SendReport($"Directory {SourceDirectory} restored from backup {BackedUpDirectory}", ReportType.UndoneTaskWithSuccess);
                }
                catch (Exception exc) {
                    SendReport($"Failed to restore directory {SourceDirectory} from backup {BackedUpDirectory}. {exc.Message}", ReportType.UndoneTaskWithFailure);
                }
            }
        }

        public override void Cleanup() {
            if (!_fileSysCommand.DirectoryExists(BackedUpDirectory)) return;

            try {
                _fileSysCommand.DirectoryDelete(BackedUpDirectory);
                SendReport($"Deleted backup folder {BackedUpDirectory}", ReportType.DoneCleanupWithSuccess);
            }
            catch (Exception exc) {
                SendReport($"Failed to delete backup folder {BackedUpDirectory}. {exc.Message}", ReportType.DoneCleanupWithFailure);
            }
        }

        private bool sourceDirExists() {
            if (!_fileSysCommand.DirectoryExists(SourceDirectory)) {
                SendReport($"Cannot delete directory {SourceDirectory} because it doesn't exist", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
                return false;
            }
            return true;
        }

    }
}
