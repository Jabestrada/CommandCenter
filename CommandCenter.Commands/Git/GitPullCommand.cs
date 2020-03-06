using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.IO;
using System.Text.RegularExpressions;

namespace CommandCenter.Commands.Git {
    public class GitPullCommand : BaseCmdLineCommand {
        public string GitDirectory { get; protected set; }
        public string RemoteName { get; protected set; }
        public string BranchName { get; set; }
        public GitPullCommand(string exePath, string gitDirectory, string remoteName, string branchName) {
            Executable = exePath;
            GitDirectory = gitDirectory;
            RemoteName = remoteName;
            BranchName = branchName;
        }
        protected override void SetArguments() {
            var gitDir = GitDirectory.EndsWith(".git") ? GitDirectory : Path.Combine(GitDirectory, ".git");
            CommandLineArguments.Add($"--git-dir={gitDir}");
            CommandLineArguments.Add("pull");
            CommandLineArguments.Add(RemoteName);
            CommandLineArguments.Add(BranchName);
        }

        protected override void OnCommandWillRun() {
            SendReport($"GitPullCommand => Running \"{ConstructedCommand}\" ...", ReportType.Progress);
        }
        protected override void OnCommandDidRun() {
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"GitPullCommand {result} with exit code {ExitCode} for directory {GitDirectory}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }

        protected override void OnErrorStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                var hadError = Regex.IsMatch(data, "error|fatal", RegexOptions.IgnoreCase);
                var reportType = hadError ? "ERROR" : "info";
                SendReport($"GitPullCommand {reportType} => {data}", ReportType.Progress);
            }
        }

        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"GitPullCommand info => {data}", ReportType.Progress);
            }
        }
    }
}
