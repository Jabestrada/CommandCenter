using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Dispatch;
using System;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class DirectoryDeleteCommand : BaseDirectoryDeleteCommand {

        public DirectoryDeleteCommand(string dirToDelete, string backupDir, IFileSystemCommandsStrategy fileSysCommand)
           : base(dirToDelete, backupDir, fileSysCommand) {
        }

        public DirectoryDeleteCommand(string dirToDelete, string backupDir)
            : base(dirToDelete, backupDir) {
        }

        protected override void Delete() {
            try {
                SendReport($"Deleting directory {SourceDirectory} ...", ReportType.Progress);
                FileSystemCommands.DirectoryDelete(SourceDirectory);
                SendReport($"Directory {SourceDirectory} deleted", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"Failed to delete directory {SourceDirectory}. {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        protected override void RunUndo() {
            FileSystemCommands.DirectoryMove(BackedUpDirectory, SourceDirectory);
        }
    }
}
