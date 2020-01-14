using CommandCenter.Infrastructure;

namespace CommandCenter.Commands.FileSystem {
    public abstract class BaseDirectoryCommand : BaseCommand {
        public string BackedUpDirectory { get; protected set; }
        public string BackupDirectory { get; protected set; }
        public string SourceDirectory { get; protected set; }

        protected IFileSystemCommandsStrategy FileSystemCommands = new FileSystemCommands();

    }
}
