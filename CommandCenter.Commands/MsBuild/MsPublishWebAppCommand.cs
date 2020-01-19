using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Commands.MsBuild {
    public class MsPublishWebAppCommand : BaseCmdLineCommand {
        public string Source { get; protected set; }
        public string Configuration { get; protected set; }
        public string PublishProfile { get; protected set; }
        public override bool IsUndoable => false;

        public MsPublishWebAppCommand(string msBuildExe, string sourceFile, string configuration, string publishProfile) {
            Executable = msBuildExe;
            Source = sourceFile;        // must be .csproj
            Configuration = configuration;
            PublishProfile = publishProfile;
        }

        protected override void SetArguments() {
            CommandLineArguments.Add($"/nologo");
            CommandLineArguments.Add($"\"{Source}\"");
            CommandLineArguments.Add($"/p:DeployOnBuild=true");
            CommandLineArguments.Add($"/p:Configuration={Configuration}");
            CommandLineArguments.Add($"/p:PublishProfile={PublishProfile}");
        }
        protected override void OnCommandWillRun() {
            SendReport($"MsPublishWebAppCommand started. Project: '{Source}', Configuration: {Configuration}", ReportType.Progress);
        }
        protected override void OnCommandDidRun() {
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"MsPublishWebAppCommand {result} with exit code {ExitCode}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"WebPublish info => {data}", ReportType.Progress);
            }
        }
        protected override void OnErrorStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                SendReport($"WebPublish ERROR => {data}", ReportType.Progress);
            }
        }
    }
}
