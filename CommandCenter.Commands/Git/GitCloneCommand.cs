using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandCenter.Commands.Git {
    public class GitCloneCommand : BaseCmdLineCommand {
        public override bool HasPreFlightCheck => true;
        public string RemoteRepo { get; protected set; }
        public string Branch { get; protected set; }
        public string LocalCloneDirectory { get; protected set; }
        public GitCloneCommand(string exePath, string remoteRepo, string branch, string localCloneDirectory) {
            Executable = exePath;
            RemoteRepo = remoteRepo;
            Branch = branch;
            LocalCloneDirectory = localCloneDirectory;
        }
        protected override void SetArguments() {
            CommandLineArguments.Add($"clone --branch {Branch}");
            CommandLineArguments.Add(RemoteRepo);
            CommandLineArguments.Add($"\"{LocalCloneDirectory}\"");
            CommandLineArguments.Add("--recurse-submodules");
        }
        protected override void OnCommandWillRun() {
            SendReport($"{ShortName} => Running \"{ConstructedCommand}\" ...", ReportType.Progress);
        }

        protected override void OnCommandDidRun() {
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"{ShortName} {result} with exit code {ExitCode} for remote repo {RemoteRepo} for local clone {LocalCloneDirectory}",
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
        public override bool PreFlightCheck() {
            if (FileSystemCommand.DirectoryGetFiles(LocalCloneDirectory).Any()) {
                SendReport($"{ShortName} will likely FAIL because target directory {LocalCloneDirectory} is not empty", ReportType.DonePreFlightWithFailure);
                return false;
            }
            return DefaultPreFlightCheckSuccess();
        }
    }
}
