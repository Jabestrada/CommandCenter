using CommandCenter.Infrastructure;
using System;
using System.Collections.Generic;

namespace CommandCenter.Commands.Svn {
    public class SvnUpdateCommand : BaseCommand {

        public string SvnCommand { get; protected set; }
        public string DirectoryToUpdate { get; protected set; }
        public override bool IsUndoable => false;

        public SvnUpdateCommand(string svnCommand, string directoryToUpdate) {
            SvnCommand = svnCommand;
            DirectoryToUpdate = directoryToUpdate;
        }

        public override void Do() {
            var arguments = new List<string>() {
                $"update",
                $"\"{DirectoryToUpdate}\""
            };
            using (CommandLineProcess cmd = new CommandLineProcess(SvnCommand, string.Join(" ", arguments))) {
                SendReport($"SVN update started on directory {DirectoryToUpdate}", ReportType.Progress);

                int exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);

                DidCommandSucceed = exitCode == 0;
                var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
                SendReport($"SvnUpdateCommand {result} with exit code {exitCode} for directory {DirectoryToUpdate}",
                           DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
            }
        }

        private void errorStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                SendReport($"SvnUpdateCommand ERROR => {message}", ReportType.Progress);
            }
        }

        private void outputStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                SendReport($"SvnUpdateCommand info => {message}", ReportType.Progress);
            }
        }

    }
}
