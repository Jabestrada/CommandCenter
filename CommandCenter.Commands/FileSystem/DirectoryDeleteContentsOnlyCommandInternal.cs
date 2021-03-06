﻿using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;

namespace CommandCenter.Commands.FileSystem {
    public class DirectoryDeleteContentsOnlyCommandInternal : BaseDirectoryDeleteCommand {
        public DirectoryDeleteContentsOnlyCommandInternal(string dirToDelete, string backupDir, IFileSystemCommandsStrategy fileSysCommand)
        : base(dirToDelete, backupDir, fileSysCommand) {
        }

        public DirectoryDeleteContentsOnlyCommandInternal(string dirToDelete, string backupDir)
            : base(dirToDelete, backupDir) {
        }

        public override bool HasPreFlightCheck => true;
        protected override void Delete() {
            try {
                SendReport($"Deleting contents of folder {SourceDirectory} ...", ReportType.Progress);
                FileSystemCommands.DirectoryDeleteContentsOnly(SourceDirectory, deleteProgress);
                DidCommandSucceed = true;
                SendReport($"Deleted contents of folder {SourceDirectory}", ReportType.DoneTaskWithSuccess);
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"Failed to delete contents of folder {SourceDirectory}. {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        public override bool PreFlightCheck() {
            if (FileSystemCommands.DirectoryExists(SourceDirectory) && !PreflightCheckWriteAccessToDirectory(SourceDirectory)) {
                return false;
            }

            return DefaultPreFlightCheckSuccess();
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
