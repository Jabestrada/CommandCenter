using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandCenter.Commands.Svn {
    public class SvnUpdateCommand : BaseCmdLineCommand {
        public string DirectoryToUpdate { get; protected set; }
        public override bool IsUndoable => false;
        public override bool HasPreFlightCheck => true;

        public SvnUpdateCommand(string svnCommand, string directoryToUpdate) : base() {
            Executable = svnCommand;
            DirectoryToUpdate = directoryToUpdate;
        }

        protected override void SetArguments() {
            CommandLineArguments.Add("update");
            CommandLineArguments.Add($"\"{DirectoryToUpdate}\"");
        }
        protected override void OnCommandWillRun() {
            SendReport($"SVN update started on directory {DirectoryToUpdate}", ReportType.Progress);
        }
        protected override void OnCommandDidRun() {
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"{ShortName} {result} with exit code {ExitCode} for directory {DirectoryToUpdate}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }
        protected override void OnErrorStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"{ShortName} ERROR => {data}", ReportType.Progress);
            }
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"{ShortName} info => {data}", ReportType.Progress);
            }
        }
        public override bool PreflightCheck() {
            var preFlightCheck = base.PreflightCheck();
            if (!preFlightCheck) return false;

            if (!FileSystemCommand.DirectoryExists(DirectoryToUpdate)) {
                SendReport($"{ShortName} will FAIL because target directory {DirectoryToUpdate} was not found", ReportType.DonePreFlightWithFailure);
                return false;
            }

            if (!isValidSvnRepo(DirectoryToUpdate)) {
                SendReport($"{ShortName} will FAIL because target directory {DirectoryToUpdate} does not seem to be a valid SVN repo",
                            ReportType.DonePreFlightWithFailure);
                return false;
            }

            return DefaultPreflightCheckSuccess();
        }

        private StringBuilder _svnInfoStreamOutput;

        private bool isValidSvnRepo(string targetDirectory) {
            _svnInfoStreamOutput = new StringBuilder();
            var startArgs = new List<string>();
            startArgs.Add("info");
            startArgs.Add($"{targetDirectory}");
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, true, string.Join(" ", startArgs))) {
                var exitCode = cmd.Run(svnInfoStreamReceiver, svnInfoStreamReceiver);
                var svnOutput = _svnInfoStreamOutput.ToString();
                return exitCode == SuccessExitCode && svnOutput.IndexOf("not a working copy", StringComparison.InvariantCultureIgnoreCase) < 0;
            }
        }

        private void svnInfoStreamReceiver(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                _svnInfoStreamOutput.Append(data);
            }
        }
    }
}
