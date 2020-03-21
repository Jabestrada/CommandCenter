using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandCenter.Commands.FileSystem {
    public class FileCopyCommand : BaseFileCommand {
        public Dictionary<string, string> FileCopyPairs = new Dictionary<string, string>();
        public string TargetFilename { get; protected set; }
        public List<string> CopiedFiles = new List<string>();

        public FileCopyCommand(string backupDir, IFileSystemCommandsStrategy fileSystemCommandsStrategy, params string[] fileCopyPairs)
                : this(backupDir, fileCopyPairs) {
            FileSystemCommands = fileSystemCommandsStrategy;
        }
        public FileCopyCommand(string backupDir, params string[] fileCopyPairArgs) {
            if (fileCopyPairArgs.Length % 2 != 0) {
                throw new ArgumentException($"Count for argument {nameof(fileCopyPairArgs)} should be even");
            }
            for (var j = 0; j < fileCopyPairArgs.Length; j++) {
                if (j % 2 == 0) {
                    FileCopyPairs.Add(fileCopyPairArgs[j], string.Empty);
                }
                else {
                    FileCopyPairs[fileCopyPairArgs[j - 1]] = fileCopyPairArgs[j];
                }
            }
            BackupFolder = backupDir;
        }

        public override bool IsUndoable => true;
        public override bool HasPreFlightCheck => true;
        public override void Do() {
            if (!allSourceFilesExist() || anyDestinationFileExists()) return;

            foreach (var fileCopyPair in FileCopyPairs) {
                try {
                    FileCopy(fileCopyPair.Key, fileCopyPair.Value);
                    SendReport($"Copied file {fileCopyPair.Key} to {fileCopyPair.Value}", ReportType.Progress);
                    CopiedFiles.Add(fileCopyPair.Value);
                }
                catch (Exception exc) {
                    SendReport($"FAILED to copy file {fileCopyPair.Key} to {fileCopyPair.Value}. {exc.Message}", ReportType.DoneTaskWithFailure);
                    DidCommandSucceed = false;
                    return;
                }
            }
            SendReport($"All files successfully copied", ReportType.DoneTaskWithSuccess);
            DidCommandSucceed = true;
        }

        private bool anyDestinationFileExists() {
            foreach (var targetFile in FileCopyPairs.Values) {
                if (FileExists(targetFile)) {
                    SendReport($"Command failed because destination file {targetFile} already exists", ReportType.DoneTaskWithFailure);
                    DidCommandSucceed = false;
                    return true;
                }
            }
            return false;
        }

        private bool allSourceFilesExist() {
            foreach (var sourceFile in FileCopyPairs.Keys) {
                if (!FileExists(sourceFile)) {
                    SendReport($"Command failed because source file {sourceFile} does not exist", ReportType.DoneTaskWithFailure);
                    DidCommandSucceed = false;
                    return false;
                }
            }
            return true;
        }

        public override void Undo() {
            if (!CopiedFiles.Any()) return;

            foreach (var copiedFile in CopiedFiles) {
                try {
                    FileDelete(copiedFile);
                    SendReport($"Undoing command by deleting file {copiedFile}", ReportType.Progress);
                }
                catch (Exception exc) {
                    SendReport($"Undo FAILED to delete copied file {copiedFile}: {exc.Message}", ReportType.Progress);
                }
            }
            SendReport($"Undo completed successfully; all copied files were deleted", ReportType.DoneCleanupWithSuccess);
        }

        public override bool PreFlightCheck() {
            foreach (var fileCopyPair in FileCopyPairs) {
                if (!FileExists(fileCopyPair.Key)) {
                    SendReport($"Command will likely FAIL because source file {fileCopyPair.Key} was not found, or application does not have sufficient permissions", ReportType.DonePreFlightWithFailure);
                    return false;
                }
                if (FileExists(fileCopyPair.Value)) {
                    SendReport($"Command will likely FAIL because destination file {fileCopyPair.Value} already exists", ReportType.DonePreFlightWithFailure);
                    return false;
                }
                if (!PreflightCheckReadAccessFromDirectory(Path.GetDirectoryName(fileCopyPair.Key))) return false;

                if (!PreflightCheckWriteAccessToDirectory(Path.GetDirectoryName(fileCopyPair.Value))) return false;
            }

            return DefaultPreFlightCheckSuccess();
        }

        public override void Cleanup() {
            // Nothing to do for cleanup
        }
    }
}
