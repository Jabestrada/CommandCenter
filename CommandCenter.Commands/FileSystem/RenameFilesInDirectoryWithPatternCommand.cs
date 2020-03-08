using CommandCenter.Infrastructure.Orchestration;

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
        public override bool PreflightCheck() {
            foreach (var dir in SourceDirectories) {
                if (!FileSystemCommands.DirectoryExists(dir)) {
                    SendReport(this, $"{ShortName} is likely to fail because at least one of its source directories {dir} was not found, or application does not have sufficient permissions", ReportType.DonePreFlightWithFailure);
                    return false;
                }
                if (!PreflightCheckDirectoryReadWriteAccess(dir)) return false;
            }

            return DefaultPreflightCheckSuccess();
        }
    }
}
