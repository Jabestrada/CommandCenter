using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.IO;
using System.Linq;

namespace CommandCenter.Commands.IIS {
    public class IisAppPoolStartCommand : BaseIisCommand {
        public override bool IsUndoable => true;
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
                SendReport($"{ShortName} => {data}", ReportType.Progress);
            }
        }
        public override void Undo() {
            if (!DidCommandSucceed) {
                SendReport($"{ShortName} => Not stopping app pool {AppPoolName} on Undo because Do failed", ReportType.UndoneTaskWithSuccess);
                return;
            }

            SendReport($"IisAppPoolStartCommand => Attempting to stop app pool {AppPoolName} on Undo...", ReportType.Progress);
            var startArgs = CommandLineArguments.ToList();
            startArgs[0] = "stop";
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                var exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                if (exitCode == SuccessExitCode) {
                    SendReport($"{ShortName} => Successfully stopped app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithSuccess);
                }
                else {
                    SendReport($"{ShortName} => Failed to stop app pool {AppPoolName} on Undo", ReportType.UndoneTaskWithFailure);
                }
            }
        }
        public override bool HasPreFlightCheck => true;
        public override bool PreflightCheck() {
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

            return DefaultPreflightCheckSuccess();
        }
    }
}
