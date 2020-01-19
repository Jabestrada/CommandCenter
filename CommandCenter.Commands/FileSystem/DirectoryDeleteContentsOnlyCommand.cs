using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Dispatch;
using System;

namespace CommandCenter.Commands.FileSystem {
    public class DirectoryDeleteContentsOnlyCommand : BaseDirectoryDeleteCommand {
        public DirectoryDeleteContentsOnlyCommand(string dirToDelete, string backupDir, IFileSystemCommandsStrategy fileSysCommand)
        : base(dirToDelete, backupDir, fileSysCommand) {
        }

        public DirectoryDeleteContentsOnlyCommand(string dirToDelete, string backupDir)
            : base(dirToDelete, backupDir) {
        }

        protected override void Delete() {
            try {
                SendReport($"Deleting contents of folder {SourceDirectory} ...", ReportType.DoneTaskWithSuccess);
                FileSystemCommands.DirectoryDeleteContentsOnly(SourceDirectory, deleteProgress);
                DidCommandSucceed = true;
                SendReport($"Deleted contents of folder {SourceDirectory}", ReportType.DoneTaskWithSuccess);
            }
            catch (Exception exc) { 
                DidCommandSucceed = false;
                SendReport($"Failed to delete contents of folder {SourceDirectory}. {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        private void deleteProgress(string deletedItem, FileSystemItemType type) {
            string deletedWhatType = type == FileSystemItemType.File ? "file" : "directory";
            SendReport($"Deleted {deletedWhatType} {deletedItem}", ReportType.Progress); 
        }

        protected override void RunUndo() {
            FileSystemCommands.DirectoryDeleteContentsOnly(SourceDirectory, null);
            FileSystemCommands.DirectoryMoveContents(BackedUpDirectory, SourceDirectory);
        }
    }
}
