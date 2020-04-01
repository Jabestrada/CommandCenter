using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Commands.NuGet {
    public class NuGetRestoreCommand : BaseCmdLineCommand {
        public readonly string Target;
        public NuGetRestoreCommand(string exe, string target) {
            Executable = exe;
            Target = target;
        }
        public override bool IsUndoable => false;
        public override bool HasPreFlightCheck => true;
        protected override void SetArguments() {
            CommandLineArguments.Add($"restore {Target}");
        }

        protected override void OnErrorStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"ERROR: {data}", ReportType.Progress);
            }
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"{data}", ReportType.Progress);
            }
        }
        protected override void OnCommandDidRun() {
            if (DidCommandSucceed) {
                SendReport($"Successfully restored NuGet packages of \"{Target}\"", ReportType.DoneTaskWithSuccess);
            }
            else {
                SendReport($"FAILED to restore NuGet packages of \"{Target}\" with exit code {ExitCode}", ReportType.DoneTaskWithFailure);
            }
        }
        public override bool PreFlightCheck() {
            if (!FileSystemCommand.FileExists(Target)) {
                SendReport($"Command likely to fail because {Target} does not exist", ReportType.DonePreFlightWithFailure);
                return false;
            }
            return DefaultPreFlightCheckSuccess();
        }
    }
}
