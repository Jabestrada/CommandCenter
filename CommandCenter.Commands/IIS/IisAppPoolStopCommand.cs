using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.Linq;

namespace CommandCenter.Commands.IIS {
    public class IisAppPoolStopCommand : BaseCmdLineCommand {
        public override bool IsUndoable => true;
        public string AppPoolName { get; protected set; }
        public bool AlreadyStoppedPrior { get; protected set; }
        public IisAppPoolStopCommand(string cmdPath, string appPoolName) {
            Executable = cmdPath;
            AppPoolName = appPoolName;
        }
        protected override void SetArguments() {
            CommandLineArguments.Add("stop");
            CommandLineArguments.Add("apppool");
            CommandLineArguments.Add($"/apppool.name:\"{AppPoolName}\"");
        }
        protected override void OnCommandWillRun() {
            AlreadyStoppedPrior = false;
            SendReport($"Stopping IIS app pool \"{AppPoolName}\"...", ReportType.Progress);
        }
        protected override void OnCommandDidRun() {
            if (DidCommandSucceed) {
                SendReport($"Successfully STOPPED IIS app pool \"{AppPoolName}\"", ReportType.DoneTaskWithSuccess);
            }
            else {
                if (AlreadyStoppedPrior) {
                    DidCommandSucceed = true;
                    SendReport($"IIS app pool \"{AppPoolName}\" already STOPPED before command executed", ReportType.DoneTaskWithSuccess);
                }
                else {
                    SendReport($"FAILED to stop app pool \"{AppPoolName}\" with exit code {ExitCode}", ReportType.DoneTaskWithFailure);
                }
            }
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                if (data.Contains("already stopped")) {
                    AlreadyStoppedPrior = true;
                }
                SendReport($"IisAppPoolStopCommand => {data}", ReportType.Progress);
            }
        }

        public override void Undo() {
            if (AlreadyStoppedPrior) {
                SendReport($"IisAppPoolStopCommand => Not restarting app pool {AppPoolName} on Undo because it wasn't running before", ReportType.UndoneTaskWithSuccess);
            }
            else {
                SendReport($"IisAppPoolStopCommand => Attempting to restart app pool {AppPoolName} on Undo...", ReportType.Progress);
                var startArgs = CommandLineArguments;
                startArgs[0] = "start";
                using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                    var exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                    if (exitCode == SuccessExitCode) {
                        SendReport($"IisAppPoolStopCommand => Successfully restarted app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithSuccess);
                    }
                    else {
                        SendReport($"IisAppPoolStopCommand => Failed to restart app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithFailure);

                    }
                }
            }
        }
    }
}
