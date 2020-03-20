using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandCenter.Commands.FileSystem {
    public class FileDeleteCommand : BaseFileCommand {
        public Dictionary<string, string> SourceFiles = new Dictionary<string, string>();

        public FileDeleteCommand(string backupDir, params string[] sourceFiles)
            : this(backupDir, null, sourceFiles) {
        }

        public FileDeleteCommand(string backupDir, IFileSystemCommandsStrategy fileSystemCommands = null, params string[] sourceFiles) {
            BackupFolder = backupDir;
            if (fileSystemCommands != null) {
                FileSystemCommands = fileSystemCommands;
            }
            foreach (var file in sourceFiles) {
                SourceFiles.Add(file, "");
            }
        }

        public override bool IsUndoable => true;
        public override bool HasPreFlightCheck => true;
        public override void Do() {
            if (!createBackup()) return;

            deleteFile();
        }

        public override void Undo() {
            doUndo();
        }

        public override void Cleanup() {
            doCleanup();
        }

        public override bool PreFlightCheck() {
            foreach (var file in SourceFiles.Keys) {
                var sourceDir = Path.GetDirectoryName(file);
                if (FileSystemCommands.DirectoryExists(sourceDir) && !PreflightCheckWriteAccessToDirectory(sourceDir)) {
                    return false;
                }
            }
            return DefaultPreFlightCheckSuccess();
        }

        #region private methods
        private void deleteFile() {
            string currentFile = string.Empty;
            try {
                foreach (var file in SourceFiles.Keys) {
                    if (!sourceFileExists(file)) continue;

                    currentFile = file;
                    SendReport($"{ShortName} => Deleting file {file}...", ReportType.Progress);
                    FileDelete(file);
                    SendReport($"{ShortName} => Deleted file {file}", ReportType.Progress);
                }
                SendReport($"{ShortName} => Deleted all files successfully", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"{ShortName} => Failed to delete {currentFile}: {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        private void doUndo() {
            if (!DidCommandSucceed) return;

            try {
                foreach (var entry in SourceFiles) {
                    if (string.IsNullOrEmpty(entry.Value)) {
                        continue;
                    }
                    FileMove(entry.Value, entry.Key);
                    SendReport($"{ShortName} => File {entry.Key} restored from backup {entry.Value}", ReportType.Progress);
                }
                SendReport($"{ShortName} => Successfully restored deleted files from backups", ReportType.UndoneTaskWithSuccess);
            }
            catch (Exception exc) {
                SendReport($"{ShortName} => Failed to restore backups: {exc.Message}", ReportType.UndoneTaskWithFailure);
            }
        }

        private void doCleanup() {
            try {
                foreach (var backupFile in SourceFiles.Values) {
                    if (!FileExists(backupFile)) continue;
                    FileDelete(backupFile);
                    SendReport($"{ShortName} => Deleted backup file {backupFile}", ReportType.Progress);
                }
                SendReport($"{ShortName} => Deleted all backup files", ReportType.DoneCleanupWithSuccess);
            }
            catch (Exception exc) {
                SendReport($"{ShortName} => Failed to delete all backup files: {exc.Message}", ReportType.DoneCleanupWithFailure);
            }
        }

        private bool createBackup() {
            foreach (var file in SourceFiles.Keys.ToList()) {
                if (!FileExists(file)) {
                    SendReport($"{ShortName} => Skipping backup of file {file} because it doesn't exist", ReportType.Progress);
                    continue;
                }

                var fileNameOnly = Path.GetFileName(file);
                var backupFilename = Path.Combine(BackupFolder, $"{fileNameOnly}");
                int counter = 0;
                while (FileExists(backupFilename)) {
                    backupFilename = Path.Combine(BackupFolder, $"{fileNameOnly}.{counter}");
                    counter++;
                }
                try {
                    FileCopy(file, backupFilename);
                    SendReport($"{ShortName} => Created backup of file {file} to {backupFilename }", ReportType.Progress);
                    SourceFiles[file] = backupFilename;
                }
                catch (Exception exc) {
                    SendReport($"{ShortName} => File delete aborted because attempt to backup file {file} to {backupFilename} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                    return false;
                }
            }
            return true;

        }

        private bool sourceFileExists(string file) {
            if (!FileExists(file)) {
                SendReport($"{ShortName} => Cannot delete {file} because it doesn't exist", ReportType.Progress);
                return false;
            }
            return true;
        }
        #endregion

    }
}
