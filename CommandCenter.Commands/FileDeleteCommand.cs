using CommandCenter.Commands.FileSystem;
using CommandCenter.Infrastructure;
using System;
using System.IO;

namespace CommandCenter.Commands {
    public class FileDeleteCommand : BaseCommand {
        private readonly string _sourceFileName;
        private readonly string _backupDir;
        private string _backupFileName;
        private IFileSystemCommand _fileSystemCommand = new FileSystemCommand();

        private bool _didDeleteSucceed = false;

        public FileDeleteCommand(string sourceFileName, string backupDir, IFileSystemCommand fileSystemCommand) :
            this(sourceFileName, backupDir) {
            _fileSystemCommand = fileSystemCommand;
        }
        public FileDeleteCommand(string sourceFileName, string backupDir) {
            _sourceFileName = sourceFileName;
            _backupDir = backupDir;
        }
        public override bool IsUndoable => true;

        public override void Cleanup() {
            if (_fileSystemCommand.FileExists(_backupFileName)) {
                _fileSystemCommand.FileDelete(_backupFileName);
            }
        }

        public override void Do() {
            if (!_fileSystemCommand.FileExists(_sourceFileName)) {
                SendReport($"Did not delete {_sourceFileName} because it doesn't exist", ReportType.DoneTaskWithSuccess);
                return;
            }

            var fileNameOnly = Path.GetFileName(_sourceFileName);
            _backupFileName = Path.Combine(_backupDir, $"{fileNameOnly}.backup.{Id}");

            try {
                _fileSystemCommand.FileCopy(_sourceFileName, _backupFileName);
                SendReport($"Created backup of file {_sourceFileName} to {_backupFileName}", ReportType.Progress);
            }
            catch (Exception exc) {
                SendReport($"File delete aborted because attempt to backup file {_sourceFileName} to {_backupFileName} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                return;
            }

            try {
                SendReport($"Deleting file {_sourceFileName}...", ReportType.Progress);
                _fileSystemCommand.FileDelete(_sourceFileName);
                SendReport($"Deleted file {_sourceFileName}", ReportType.DoneTaskWithSuccess);
                _didDeleteSucceed = true;
            }
            catch (Exception exc) {
                SendReport($"Failed to delete {_sourceFileName}: {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            if (_didDeleteSucceed) {
                _fileSystemCommand.FileMove(_backupFileName, _sourceFileName);
            }
        }
    }
}
