using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class FileDeleteCommand : BaseFileCommand {

        public FileDeleteCommand(string sourceFileName, string backupDir, IFileSystemCommandsStrategy fileSystemCommands) :
            this(sourceFileName, backupDir) {
            FileSystemCommands = fileSystemCommands;
        }
        public FileDeleteCommand(string sourceFileName, string backupDir) {
            SourceFilename = sourceFileName;
            BackupFolder = backupDir;
        }

        public override bool IsUndoable => true;
        public override void Do() {
            if (!sourceFileExists() || !createBackup()) return;

            deleteFile();
        }

        public override void Undo() {
            doUndo();
        }

        public override void Cleanup() {
            doCleanup();
        }

        #region private methods
        private void deleteFile() {
            try {
                SendReport($"Deleting file {SourceFilename}...", ReportType.Progress);
                FileDelete(SourceFilename);
                SendReport($"Deleted file {SourceFilename}", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"Failed to delete {SourceFilename}: {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        private void doUndo() {
            if (DidCommandSucceed) {
                try {
                    FileMove(BackupFilename, SourceFilename);
                    SendReport($"File {SourceFilename} restored from backup {BackupFilename}", ReportType.UndoneTaskWithSuccess);
                }
                catch (Exception exc) {
                    SendReport($"Failed to restore file {SourceFilename} from backup {BackupFilename}. {exc.Message}", ReportType.UndoneTaskWithFailure);
                }
            }
        }

        private void doCleanup() {
            if (!FileExists(BackupFilename)) return;

            try {
                FileDelete(BackupFilename);
                SendReport($"Deleted backup file {BackupFilename}", ReportType.DoneCleanupWithSuccess);
            }
            catch (Exception exc) { 
                SendReport($"Failed to delete backup file {BackupFilename}. {exc.Message}", ReportType.DoneCleanupWithFailure);
            }
        }

        private bool createBackup() {
            var fileNameOnly = Path.GetFileName(SourceFilename);
            BackupFilename = Path.Combine(BackupFolder, $"{fileNameOnly}.backup.{Id}");
            try {
                FileCopy(SourceFilename, BackupFilename);
                SendReport($"Created backup of file {SourceFilename} to {BackupFilename}", ReportType.Progress);
                return true;
            }
            catch (Exception exc) {
                SendReport($"File delete aborted because attempt to backup file {SourceFilename} to {BackupFilename} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                return false;
            }

        }

        private bool sourceFileExists() {
            if (!FileExists(SourceFilename)) {
                SendReport($"Cannot delete {SourceFilename} because it doesn't exist", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
                return false;
            }
            return true;
        }
        #endregion

    }
}
