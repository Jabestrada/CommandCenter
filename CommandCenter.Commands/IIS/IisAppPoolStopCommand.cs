using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.IO;
using System.Linq;

namespace CommandCenter.Commands.IIS {
    public class IisAppPoolStopCommand : BaseIisCommand {
        public override bool IsUndoable => true;
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
                SendReport($"{ShortName} => {data}", ReportType.Progress);
            }
        }

        public override void Undo() {
            if (!DidCommandSucceed) {
                SendReport($"{ShortName} => Not restarting app pool {AppPoolName} on Undo because Do failed", ReportType.UndoneTaskWithSuccess);
                return;
            }

            if (AlreadyStoppedPrior) {
                SendReport($"{ShortName} => Not restarting app pool {AppPoolName} on Undo because it wasn't running before", ReportType.UndoneTaskWithSuccess);
            }
            else {
                SendReport($"{ShortName} => Attempting to restart app pool {AppPoolName} on Undo...", ReportType.Progress);
                var startArgs = CommandLineArguments.ToList();
                startArgs[0] = "start";
                using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                    var exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                    if (exitCode == SuccessExitCode) {
                        SendReport($"{ShortName} => Successfully restarted app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithSuccess);
                    }
                    else {
                        SendReport($"{ShortName} => Failed to restart app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithFailure);

                    }
                }
            }
        }
        public override bool HasPreFlightCheck => true;
        public override bool PreFlightCheck() {
            if (!IsCurrentUserAdmin) {
                SendReport(this, $"{ShortName} will FAIL because application is not running with Administrator privileges", ReportType.DonePreFlightWithFailure);
                return false;
            }

            if (ValidateExePath && !File.Exists(Executable)) {
                SendReport(this, $"{ShortName} will FAIL because file {Executable} was not found", ReportType.DonePreFlightWithFailure);
                return false;
            }

            SendReport(this, $"{ShortName}: Checking if app pool \"{AppPoolName}\" exists...", ReportType.Progress);
            if (!DoesAppPoolExist(AppPoolName)) {
                SendReport(this, $"{ShortName} will FAIL because app pool \"{AppPoolName}\" was not found", ReportType.DonePreFlightWithFailure);
                return false;
            }
            else {
                SendReport(this, $"{ShortName} => App pool \"{AppPoolName}\" exists", ReportType.Progress);
            }

            return DefaultPreFlightCheckSuccess();
        }
    }
}
