using CommandCenter.Infrastructure;
using System;
using System.IO;

namespace CommandCenter.Commands {
    public class FileDeleteCommand : BaseCommand {
        private readonly string _sourceFileName;
        private readonly string _backupDir;
        private string _backupFileName;

        private bool _didDeleteSucceed = false;
        public FileDeleteCommand(string sourceFileName, string backupDir) {
            _sourceFileName = sourceFileName;
            _backupDir = backupDir;
        }
        public override bool IsUndoable => true;

        public override void Cleanup() {
            if (File.Exists(_backupFileName)) File.Delete(_backupFileName);
        }

        public override void Do() {
            if (!File.Exists(_sourceFileName)) {
                SendReport($"Did not delete {_sourceFileName} because it doesn't exist", ReportType.DoneTaskWithSuccess);
                return;
            }

            var fileNameOnly = Path.GetFileName(_sourceFileName);
            _backupFileName = Path.Combine(_backupDir, $"{fileNameOnly}.backup.{Id}");

            try {
                File.Copy(_sourceFileName, _backupFileName);
                SendReport($"Created backup of file {_sourceFileName} to {_backupFileName}", ReportType.Progress);
            }
            catch (Exception exc) {
                SendReport($"File delete aborted because attempt to backup file {_sourceFileName} to {_backupFileName} failed: {exc.Message}", ReportType.DoneTaskWithFailure);
                return;
            }

            try {
                SendReport($"Deleting file {_sourceFileName}...", ReportType.Progress);
                File.Delete(_sourceFileName);
                SendReport($"Deleted file {_sourceFileName}", ReportType.DoneTaskWithSuccess);
                _didDeleteSucceed = true;
            }
            catch (Exception exc) {
                SendReport($"Failed to delete {_sourceFileName}: {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            if (_didDeleteSucceed) {
                File.Move(_backupFileName, _sourceFileName);
            }
        }
    }
}
