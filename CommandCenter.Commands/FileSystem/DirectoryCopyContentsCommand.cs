using CommandCenter.Infrastructure;
using System;
using System.Collections.Generic;

namespace CommandCenter.Commands.FileSystem {
    public class DirectoryCopyContentsCommand : BaseDirectoryCommand {
        public override bool IsUndoable => true;
        public string TargetDirectory { get; protected set; }
        public List<string> CopiedFiles { get; protected set; }

        private bool _atLeastOneTargetFileExists;

        public DirectoryCopyContentsCommand(string sourceDirectory, string targetDirectory, IFileSystemCommandsStrategy fileSysCommand)
            : this(sourceDirectory, targetDirectory) {        
            FileSystemCommands = fileSysCommand;
        }
        public DirectoryCopyContentsCommand(string sourceDirectory, string targetDirectory) {
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            CopiedFiles = new List<string>();
        }

        public override void Do() {
            if (!FileSystemCommands.DirectoryExists(SourceDirectory)) {
                SendReport($"Cannot copy contents of source {SourceDirectory} to {TargetDirectory} because source does not exist", ReportType.DoneTaskWithFailure);
                return;
            }

            _atLeastOneTargetFileExists = false;
            CopiedFiles.Clear();
            try {
                SendReport($"Starting copy of files from {SourceDirectory} to {TargetDirectory} ...", ReportType.Progress);
                FileSystemCommands.DirectoryCopyContents(SourceDirectory, TargetDirectory, preFileCopyCallback, postCopyCallback);
                DidCommandSucceed = !_atLeastOneTargetFileExists;
                if (DidCommandSucceed) {
                    SendReport($"Finished copy of files from {SourceDirectory} to {TargetDirectory}. File(s) copied: {CopiedFiles.Count}", ReportType.DoneTaskWithSuccess);
                }
            }
            catch (Exception exc) {
                DidCommandSucceed = false;
                SendReport($"Failed to copy files from {SourceDirectory} to {TargetDirectory}. {exc.Message}", ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            int filesDeleted = 0;
            foreach (var copiedFile in CopiedFiles) {
                FileSystemCommands.FileDelete(copiedFile);
                SendReport($"Deleted file {copiedFile} during DirectoryCopyContentsCommand.Undo()", ReportType.Progress);
                filesDeleted++;
            }
            SendReport($"Deleted {filesDeleted} file(s) on undo", ReportType.UndoneTaskWithSuccess);
        }

          private bool preFileCopyCallback(string sourceFile, string targetFile) {
            if (FileSystemCommands.FileExists(targetFile)) {
                _atLeastOneTargetFileExists = true;
                SendReport($"Aborted copy of files from {SourceDirectory} to {TargetDirectory} because target file {targetFile} already exists", ReportType.DoneTaskWithFailure);
                return false;
            }
            return true;
        }

        private void postCopyCallback(string sourceFile, string targetFile) {
            CopiedFiles.Add(targetFile);
            SendReport($"File {sourceFile} copied to {targetFile}", ReportType.Progress);
        }
    }
}
