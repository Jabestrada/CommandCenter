namespace CommandCenter.Commands.FileSystem {
    public class RenameFilesInDirectoryWithPatternCommand : MultiFileRenameWithPatternCommand {
        public string[] SourceDirectories;

        public RenameFilesInDirectoryWithPatternCommand(string pattern, params string[] sourceDirectories) : base(pattern, sourceDirectories) {
            SourceDirectories = sourceDirectories;
        }

        public override void Do() {
            SourceFiles.Clear();
            foreach (var dir in SourceDirectories) {
                foreach (var file in FileSystemCommands.DirectoryGetFiles(dir)) {
                    SourceFiles.Add(file);
                }
            }

            base.Do();
        }
    }
}
