using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandCenter.Commands.Git {
    public class GitPullCommand : BaseCmdLineCommand {
        public string GitTargetDirectory { get; protected set; }
        public string RemoteName { get; protected set; }
        public string BranchName { get; set; }
        public override bool HasPreFlightCheck => true;
        public GitPullCommand(string exePath, string gitDirectory, string remoteName, string branchName) {
            Executable = exePath;
            GitTargetDirectory = gitDirectory;
            RemoteName = remoteName;
            BranchName = branchName;
        }
        protected override void SetArguments() {
            var gitDir = getLocalRepoGitFile();
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
            SendReport($"GitPullCommand {result} with exit code {ExitCode} for directory {GitTargetDirectory}",
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
        public override bool PreFlightCheck() {
            var preFlightCheck = base.PreFlightCheck();
            if (!preFlightCheck) return false;

            if (!isValidGitRepo()) {
                SendReport($"{ShortName} will FAIL because target directory {GitTargetDirectory} does not seem to be a valid Git repo", ReportType.DonePreFlightWithFailure);
                return false;
            }

            return DefaultPreFlightCheckSuccess();
        }

        private StringBuilder _gitInfoStreamOutput;
        private bool isValidGitRepo() {
            _gitInfoStreamOutput = new StringBuilder();
            var localGitRepo = getLocalRepoGitFile();
            var startArgs = new List<string>();
            startArgs.Add($"--git-dir={localGitRepo}");
            startArgs.Add("status");
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                var exitCode = cmd.Run(gitInfoStreamReceiver, gitInfoStreamReceiver);
                var gitOutput = _gitInfoStreamOutput.ToString();
                return exitCode == SuccessExitCode && gitOutput.IndexOf("not a git repository", StringComparison.InvariantCultureIgnoreCase) < 0;
            }
        }

        private string getLocalRepoGitFile() {
            return GitTargetDirectory.EndsWith(".git") ? GitTargetDirectory : Path.Combine(GitTargetDirectory, ".git");
        }

        private void gitInfoStreamReceiver(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                _gitInfoStreamOutput.Append(data);
            }
        }
    }
}
