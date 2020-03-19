﻿using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.IO;

namespace CommandCenter.Commands.FileSystem {
    public class FileCopyCommand : BaseFileCommand {
        public string TargetFilename { get; protected set; }
        public FileCopyCommand(string sourceFilename, string targetFilename, string backupDir, IFileSystemCommandsStrategy fileSystemCommandsStrategy)
                : this(sourceFilename, targetFilename, backupDir) {
            FileSystemCommands = fileSystemCommandsStrategy;
        }
        public FileCopyCommand(string sourceFilename, string targetFilename, string backupDir) {
            SourceFilename = sourceFilename;
            TargetFilename = targetFilename;
            BackupFolder = backupDir;
        }

        public override bool IsUndoable => true;
        public override bool HasPreFlightCheck => true;
        public override void Do() {
            if (!sourceFileExists() || destinationFileExists()) return;

            try {
                FileCopy(SourceFilename, TargetFilename);
                SendReport($"Copied file {SourceFilename} to {TargetFilename}", ReportType.DoneTaskWithSuccess);
                DidCommandSucceed = true;
            }
            catch (Exception exc) {
                SendReport($"Failed to copy file {SourceFilename} to {TargetFilename}. {exc.Message}", ReportType.DoneTaskWithFailure);
                DidCommandSucceed = false;
            }
        }

        private bool destinationFileExists() {
            if (FileExists(TargetFilename)) {
                SendReport($"{ShortName} failed because destination file {TargetFilename} already exists", ReportType.DoneTaskWithFailure);
                DidCommandSucceed = false;
                return true;
            }
            return false;
        }

        private bool sourceFileExists() {
            if (!FileExists(SourceFilename)) {
                SendReport($"{ShortName} failed because source file {SourceFilename} does not exist", ReportType.DoneTaskWithFailure);
                DidCommandSucceed = false;
                return false;
            }
            return true;
        }

        public override void Undo() {
            if (DidCommandSucceed) {
                FileDelete(TargetFilename);
                SendReport($"Task undone by deleting file {TargetFilename}", ReportType.DoneCleanupWithSuccess);
                return;
            }
        }

        public override bool PreFlightCheck() {
            if (!FileExists(SourceFilename)) {
                SendReport($"{ShortName} will FAIL because source file {SourceFilename} was not found, or application does not have sufficient permissions", ReportType.DonePreFlightWithFailure);
                return false;
            }
            if (FileExists(TargetFilename)) {
                SendReport($"{ShortName} will FAIL because destination file {TargetFilename} already exists", ReportType.DonePreFlightWithFailure);
                return false;
            }

            if (!PreflightCheckReadAccessFromDirectory(Path.GetDirectoryName(SourceFilename))) return false;

            if (!PreflightCheckWriteAccessToDirectory(Path.GetDirectoryName(TargetFilename))) return false;

            return DefaultPreFlightCheckSuccess();
        }

        public override void Cleanup() {
            // Nothing to do for cleanup
        }
    }
}
