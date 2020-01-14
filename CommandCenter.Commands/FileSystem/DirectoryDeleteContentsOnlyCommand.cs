namespace CommandCenter.Commands.FileSystem {
    public class DirectoryDeleteContentsOnlyCommand : BaseDirectoryDeleteCommand {
        public DirectoryDeleteContentsOnlyCommand(string dirToDelete, string backupDir, IFileSystemCommandsStrategy fileSysCommand)
        : base(dirToDelete, backupDir, fileSysCommand) {
        }

        public DirectoryDeleteContentsOnlyCommand(string dirToDelete, string backupDir)
            : base(dirToDelete, backupDir) {
        }

        protected override void Delete() {
            // TODO: Delete SourceDirectory contents only and not SourceDirectory itself
        }
    }
}
