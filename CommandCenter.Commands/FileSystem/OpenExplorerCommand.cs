using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Commands.FileSystem {
    public class OpenExplorerCommand : BaseCmdLineCommand {
        public override bool IsUndoable => false;
        public override bool ValidateExePath => false;
        protected override int SuccessExitCode => 1;
        public string SelectArg { get; protected set; }
        public OpenExplorerCommand(string selectArg) {
            Executable = "explorer.exe";
            SelectArg = selectArg;
        }

        protected override void SetArguments() {
            CommandLineArguments.Add($"/open, {SelectArg}");
        }
        protected override void OnCommandDidRun() {
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"Explorer.exe {result} with exit code {ExitCode} for directory {SelectArg}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure); ;
        }
    }
}
