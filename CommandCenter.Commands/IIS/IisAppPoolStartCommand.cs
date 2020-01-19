using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Commands.IIS {
    public class IisAppPoolStartCommand : BaseCmdLineCommand {
        public string AppPoolName { get; protected set; }
        public IisAppPoolStartCommand(string cmdPath, string appPoolName) {
            Executable = cmdPath;
            AppPoolName = appPoolName;
        }
        protected override void SetArguments() {
            CommandLineArguments.Add("start");
            CommandLineArguments.Add("apppool");
            CommandLineArguments.Add($"/apppool.name:\"{AppPoolName}\"");
        }
        protected override void OnCommandWillRun() {
            SendReport($"Starting IIS app pool \"{AppPoolName}\"...", ReportType.Progress);
        }
        protected override void OnCommandDidRun() {
            if (DidCommandSucceed) {
                SendReport($"Successfully STARTED IIS app pool \"{AppPoolName}\"", ReportType.DoneTaskWithSuccess);
            }
            else {
                SendReport($"FAILED to start app pool \"{AppPoolName}\" with exit code {ExitCode}", ReportType.DoneTaskWithFailure);
            }
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"IisAppPoolStartCommand => {data}", ReportType.Progress);
            }
        }
    }
}
