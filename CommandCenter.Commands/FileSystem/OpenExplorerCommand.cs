using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.Linq;

namespace CommandCenter.Commands.FileSystem {
    public class OpenExplorerCommand : BaseCmdLineCommand {
        public override bool IsUndoable => false;
        public override bool HasPreFlightCheck => true;
        public override bool ValidateExePath => false;
        protected override int SuccessExitCode => 1;
        public string[] FoldersToOpen { get; protected set; }

        public OpenExplorerCommand(params string[] foldersToOpen) {
            Executable = "explorer.exe";
            FoldersToOpen = foldersToOpen;
        }

        public override void Do() {
            foreach (var folder in FoldersToOpen) {
                if (!FileSystemCommand.DirectoryExists(folder)) {
                    DidCommandSucceed = false;
                    SendReport($"Windows Explorer cannot open directory {folder} because it doesn't exist",
                           ReportType.DoneTaskWithFailure);
                    return;
                }

                if (CommandLineArguments.Any()) {
                    CommandLineArguments[0] = $"/open, {folder}";
                }
                else { 
                    CommandLineArguments.Add($"/open, {folder}");
                }
                runCommand();
                SendReport($"Windows Explorer exited with code {ExitCode} for directory {folder}",
                           ReportType.Progress);
            }
            DidCommandSucceed = true;
            SendReport($"Windows Explorer opened all requested folders successfully",
                           ReportType.DoneTaskWithSuccess);
        }

        public override bool PreFlightCheck() {
            foreach (var folder in FoldersToOpen) {
                if (!FileSystemCommand.DirectoryExists(folder)) {
                    SendReport($"Command will likely fail because folder \"{folder}\" doesn't exist", ReportType.DoneTaskWithFailure);
                    return false;
                }
            }
            return DefaultPreFlightCheckSuccess();
        }
    }
}
