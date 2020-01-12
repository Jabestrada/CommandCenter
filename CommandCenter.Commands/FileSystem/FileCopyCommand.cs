using CommandCenter.Infrastructure;
using System;

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

        public override void Do() {
            throw new NotImplementedException();
        }

        public override void Undo() {
            throw new NotImplementedException();
        }

        public override void Cleanup() {
            throw new NotImplementedException();
        }
    }
}
