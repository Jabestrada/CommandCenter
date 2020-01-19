using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Dispatch;

namespace CommandCenter.Commands.Svn {
    public class SvnUpdateCommand : BaseCmdLineCommand {
        public string DirectoryToUpdate { get; protected set; }
        public override bool IsUndoable => false;
        public SvnUpdateCommand(string svnCommand, string directoryToUpdate) : base(){
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
            SendReport($"SvnUpdateCommand {result} with exit code {ExitCode} for directory {DirectoryToUpdate}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }
        protected override void OnErrorStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"SvnUpdateCommand ERROR => {data}", ReportType.Progress);
            }
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"SvnUpdateCommand info => {data}", ReportType.Progress);
            }
        }
    }
}
