using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;

namespace CommandCenter.Commands.FileSystem {
    public class DirectoryDeleteContentsOnlyCommand : BaseCommand {
        public List<DirectoryDeleteContentsOnlyCommandInternal> DeleteCommands = new List<DirectoryDeleteContentsOnlyCommandInternal>();
        public string BackupDirectory { get; protected set; }
        public override bool IsUndoable => true;
        public override bool HasPreFlightCheck => true;

        public IFileSystemCommandsStrategy FileSystemCommands = new FileSystemCommands();
        public DirectoryDeleteContentsOnlyCommand(string backupDir, IFileSystemCommandsStrategy fileSysCommand, params string[] targetDirectories)
                    : this(backupDir, targetDirectories) {
            FileSystemCommands = fileSysCommand;
        }

        public DirectoryDeleteContentsOnlyCommand(string backupDir, params string[] targetDirectories) {
            BackupDirectory = backupDir;
            foreach (var targetDir in targetDirectories) {
                var deleteCommand = new DirectoryDeleteContentsOnlyCommandInternal(targetDir, BackupDirectory);
                deleteCommand.OnReportSent += forwardReports;
                DeleteCommands.Add(deleteCommand);
            }
        }


        public override void Do() {
            foreach (var deleteCommand in DeleteCommands) {
                try {
                    SendReport($"Deleting contents of directory {deleteCommand.SourceDirectory}...", ReportType.Progress);
                    deleteCommand.Do();
                    SendReport($"Successfully deleted contents of directory {deleteCommand.SourceDirectory}", ReportType.Progress);
                }
                catch (Exception exc) {
                    DidCommandSucceed = false;
                    SendReport($"FAILED to delete contents of directory {deleteCommand.SourceDirectory}: {exc.Message}", ReportType.DoneTaskWithFailure);
                    return;
                }
            }
            DidCommandSucceed = true;
            SendReport("Successfully deleted contents of requested directories", ReportType.DoneTaskWithSuccess);
        }

        public override void Cleanup() {
            bool allOk = true;
            DeleteCommands.ForEach(c => {
                try {
                    c.Cleanup();
                    SendReport($"Cleanup succeeded for directory {c.SourceDirectory}", ReportType.Progress);
                }
                catch (Exception exc) {
                    allOk = false;
                    SendReport($"Cleanup FAILED for directory {c.SourceDirectory}: {exc.Message}", ReportType.Progress);
                }
            });
            SendReport($"Cleanup completed", allOk ? ReportType.DoneCleanupWithSuccess : ReportType.DoneCleanupWithFailure);
        }

        public override void Undo() {
            bool allOk = true;
            DeleteCommands.ForEach(c => {
                try {
                    c.Undo();
                    SendReport($"Undo succeeded for directory {c.SourceDirectory}", ReportType.Progress);
                }
                catch (Exception exc) {
                    allOk = false;
                    SendReport($"Undo FAILED for directory {c.SourceDirectory}: {exc.Message}", ReportType.Progress);
                }
            });
            SendReport($"Undo completed", allOk ? ReportType.UndoneTaskWithSuccess : ReportType.UndoneTaskWithFailure);
        }
        public override bool PreFlightCheck() {
            foreach (var cmd in DeleteCommands) {
                var result = cmd.PreFlightCheck();
                if (!result) {
                    return false;
                }
            }
            return DefaultPreFlightCheckSuccess();
        }
        private void forwardReports(BaseCommand command, CommandReportArgs args) {
            SendReport(this, args);
        }
    }
}
