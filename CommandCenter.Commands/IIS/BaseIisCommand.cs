using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.Collections.Generic;
using System.Text;

namespace CommandCenter.Commands.IIS {
    public abstract class BaseIisCommand : BaseCmdLineCommand {
        public string AppPoolName { get; protected set; }

        private StringBuilder _appPoolExistsOutput;

        public bool DoesAppPoolExist(string appPoolName = null) {
            _appPoolExistsOutput = new StringBuilder();
            if (string.IsNullOrWhiteSpace(appPoolName)) {
                appPoolName = AppPoolName;
            }

            //appcmd.exe list apppool /name:training
            var startArgs = new List<string>();
            startArgs.Add("list apppool");
            startArgs.Add($"/name:{appPoolName}");
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                var exitCode = cmd.Run(appPoolExistsStreamReceiver, appPoolExistsErrorStreamReceiver);
                // APPPOOL "training" (MgdVersion:v4.0,MgdMode:Integrated,state:Started)
                return exitCode == SuccessExitCode && _appPoolExistsOutput.ToString().IndexOf(appPoolName) > -1;
            }
        }

        private void appPoolExistsErrorStreamReceiver(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport(this, data, ReportType.Progress);
            }
        }

        private void appPoolExistsStreamReceiver(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport(this, data, ReportType.Progress);
                _appPoolExistsOutput.Append(data);
            }
        }
    }
}
