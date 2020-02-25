using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Commands.IIS {
    public class IisAppPoolStartCommand : BaseCmdLineCommand {
        public override bool IsUndoable => true;
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
        public override void Undo() {
            if (!DidCommandSucceed) {
                SendReport($"IisAppPoolStartCommand => Not stopping app pool {AppPoolName} on Undo because Do failed", ReportType.UndoneTaskWithSuccess);
                return;
            }


            SendReport($"IisAppPoolStartCommand => Attempting to stop app pool {AppPoolName} on Undo...", ReportType.Progress);
            var startArgs = CommandLineArguments;
            startArgs[0] = "stop";
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                var exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                if (exitCode == SuccessExitCode) {
                    SendReport($"IisAppPoolStartCommand => Successfully stopped app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithSuccess);
                }
                else {
                    SendReport($"IisAppPoolStartCommand => Failed to stop app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithFailure);

                }
            }
        }
    }
}
