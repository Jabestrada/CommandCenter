using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.IO;
using System.Text.RegularExpressions;

namespace CommandCenter.Commands.Console {
    public class ConsolePassThroughCommand : BaseCmdLineCommand {
        public string CommandLineParameters { get; set; }

        public override bool ValidateExePath => Executable.IndexOf(':') > -1;
        public ConsolePassThroughCommand(string command, string workingDirectory, string cmdLineParams) {
            Executable = command;
            CommandLineParameters = cmdLineParams;
            WorkingDirectory = workingDirectory;
        }

        protected override void SetArguments() {
            CommandLineArguments.Add(CommandLineParameters);
        }

        protected override void OnCommandWillRun() {
            SendReport($"{ShortName} => Running \"{ConstructedCommand}\" ...", ReportType.Progress);
        }

        protected override void OnCommandDidRun() {
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"{ShortName} {result} with exit code {ExitCode}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }

        protected override void OnErrorStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                var hadError = Regex.IsMatch(data, "error|fatal", RegexOptions.IgnoreCase);
                var reportType = hadError ? "ERROR" : "info";
                SendReport($"{ShortName} {reportType} => {data}", ReportType.Progress);
            }
        }

        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"{ShortName} info => {data}", ReportType.Progress);
            }
        }
    }
}
