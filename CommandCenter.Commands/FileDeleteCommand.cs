using CommandCenter.Commands.FileSystem;
using CommandCenter.Infrastructure;
using System;
using System.IO;

namespace CommandCenter.Commands {
    public class FileDeleteCommand : BaseCommand {
        private readonly string _sourceFileName;
        private readonly string _backupDir;
        private string _backupFilename;
        private IFileSystemCommandsStrategy _fileSystemCommand = new FileSystemCommands();
        private bool _didDeleteSucceed = false;

        public string BackupFilename => _backupFilename;

        public FileDeleteCommand(string sourceFileName, string backupDir, IFileSystemCommandsStrategy fileSystemCommand) :
            this(sourceFileName, backupDir) {
            _fileSystemCommand = fileSystemCommand;
        }
        public FileDeleteCommand(string sourceFileName, string backupDir) {
            _sourceFileName = sourceFileName;
            _backupDir = backupDir;
        }
        public override bool IsUndoable => true;

        public override void Do() {
            if (!fileSourceFileExists() || !backupSucceeded()) return;

            deleteFile();
        }

        public override void Undo() {
            if (_didDeleteSucceed) {
                _fileSystemCommand.FileMove(_backupFilename, _sourceFileName);
            }
        }
        public override void Cleanup() {
            if (_fileSystemCommand.FileExists(_backupFilename)) {
                _fileSystemCommand.FileDelete(_backupFilename);
            }
        }

        #region private methods
        private void deleteFile() {
            try {
                SendReport($"Deleting file {_sourceFileName}...", ReportType.Progress);
                _fileSystemCommand.FileDelete(_sourceFileName);
                SendReport($"Deleted file {_sourceFileName}", ReportType.DoneTaskWithSuccess);
                _didDeleteSucceed = true;
            }
            catch (Exception exc) {
                _didDeleteSucceed = false;
                SendReport($"Failed to delete {_sourceFileName}: {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        private bool backupSucceeded() {
            var fileNameOnly = Path.GetFileName(_sourceFileName);
            _backupFilename = Path.Combine(_backupDir, $"{fileNameOnly}.backup.{Id}");
            try {
                _fileSystemCommand.FileCopy(_sourceFileName, _backupFilename);
                SendReport($"Created backup of file {_sourceFileName} to {_backupFilename}", ReportType.Progress);
                return true;
            }
            catch (Exception exc) {
                SendReport($"File delete aborted because attempt to backup file {_sourceFileName} to {_backupFilename} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                return false;
            }

        }

        private bool fileSourceFileExists() {
            if (!_fileSystemCommand.FileExists(_sourceFileName)) {
                SendReport($"Did not delete {_sourceFileName} because it doesn't exist", ReportType.DoneTaskWithSuccess);
                return false;
            }
            return true;
        }
        #endregion

    }
}
