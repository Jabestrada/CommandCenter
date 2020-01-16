using CommandCenter.Infrastructure;
using System;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public abstract class BaseDirectoryDeleteCommand : BaseDirectoryCommand {
        protected abstract void Delete();
        
        public BaseDirectoryDeleteCommand(string dirToDelete, string backupDir, IFileSystemCommandsStrategy fileSysCommand)
            : this(dirToDelete, backupDir) {
            FileSystemCommands = fileSysCommand;
        }

        public BaseDirectoryDeleteCommand(string dirToDelete, string backupDir) {
            SourceDirectory = dirToDelete;
            BackupDirectory = backupDir;
        }

        public override bool IsUndoable => true;

        public override void Do() {
            if (!sourceDirExists() || !createBackup()) return;

            Delete();
        }

        public override void Undo() {
            if (DidCommandSucceed) {
                try {
                    FileSystemCommands.DirectoryMove(BackedUpDirectory, SourceDirectory);
                    SendReport($"Directory {SourceDirectory} restored from backup {BackedUpDirectory}", ReportType.UndoneTaskWithSuccess);
                }
                catch (Exception exc) {
                    SendReport($"Failed to restore directory {SourceDirectory} from backup {BackedUpDirectory}. {exc.Message}", ReportType.UndoneTaskWithFailure);
                }
            }
        }

        public override void Cleanup() {
            if (!FileSystemCommands.DirectoryExists(BackedUpDirectory)) { 
                SendReport($"Cannot delete contents of folder {SourceDirectory} during cleanup because it does not exist", ReportType.DoneTaskWithSuccess);
                return;
            }

            try {
                FileSystemCommands.DirectoryDelete(BackedUpDirectory);
                SendReport($"Deleted backup folder {BackedUpDirectory} during cleanup", ReportType.DoneCleanupWithSuccess);
            }
            catch (Exception exc) {
                SendReport($"Failed to delete backup folder {BackedUpDirectory} during cleanup. {exc.Message}", ReportType.DoneCleanupWithFailure);
            }
        }

        #region private
        private bool createBackup() {
            BackedUpDirectory = Path.Combine(BackupDirectory, $"{new DirectoryInfo(SourceDirectory).Name}.backup.dirDelete_{Id}.{Guid.NewGuid().ToString()}");
            try {
                FileSystemCommands.DirectoryCopy(SourceDirectory, BackedUpDirectory);
                SendReport($"Created backup of folder {SourceDirectory} to {BackedUpDirectory}", ReportType.Progress);
                return true;
            }
            catch (Exception exc) {
                SendReport($"Directory delete aborted because attempt to backup folder {SourceDirectory} to {BackedUpDirectory} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                return false;
            }
        }

        private bool sourceDirExists() {
            if (!FileSystemCommands.DirectoryExists(SourceDirectory)) {
                SendReport($"Cannot delete directory {SourceDirectory} because it doesn't exist", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
                return false;
            }
            return true;
        }
        #endregion
    }
}
