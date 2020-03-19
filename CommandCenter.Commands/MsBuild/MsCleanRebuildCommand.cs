using CommandCenter.Commands.CmdLine;
using CommandCenter.Infrastructure.Orchestration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandCenter.Commands.MsBuild {
    public class MsCleanRebuildCommand : BaseCmdLineCommand {
        private const string FAILED_PROJECT_BUILD_PATTERN = @"Done Building Project ""(.*\.csproj)"".*FAILED";
        public string Source { get; protected set; }
        public string Configuration { get; protected set; }
        public List<string> FailedProjectBuildResults { get; protected set; }
        public override bool IsUndoable => false;
        public override bool HasPreFlightCheck => true;
        public MsCleanRebuildCommand(string msBuildExe, string sourceFile, string configuration) {
            Executable = msBuildExe;
            Source = sourceFile;        // can be .sln or .csproj
            Configuration = configuration;
            FailedProjectBuildResults = new List<string>();
        }
        protected override void SetArguments() {
            CommandLineArguments.Add($"/nologo");
            CommandLineArguments.Add($"\"{Source}\"");
            CommandLineArguments.Add($"/p:Configuration={Configuration}");
            CommandLineArguments.Add($"/t:Clean,Build");
        }
        protected override void OnCommandWillRun() {
            SendReport($"Build started. Solution: '{Source}', Configuration: {Configuration}", ReportType.Progress);
        }
        protected override void OnCommandDidRun() {
            if (!DidCommandSucceed && FailedProjectBuildResults.Any()) {
                foreach (string failedBuildProject in FailedProjectBuildResults) {
                    SendReport($"FAILED PROJECT BUILD: {failedBuildProject}", ReportType.Progress);
                }
            }
            var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
            SendReport($"MsCleanRebuildCommand {result} with exit code {ExitCode} for {Source}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }
        protected override void OnOutputStreamDataIn(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                captureFailedProject(data);
                SendReport($"MsCleanRebuildCommand info => {data}", ReportType.Progress);
            }
        }
        public override bool PreFlightCheck() {
            var preFlightCheck = base.PreFlightCheck();
            if (!preFlightCheck) return false;

            return DefaultPreFlightCheckSuccess();
        }

        private void captureFailedProject(string message) {
            var regEx = new Regex(FAILED_PROJECT_BUILD_PATTERN, RegexOptions.IgnoreCase);
            var matches = regEx.Matches(message);
            if (matches.Count > 0) {
                FailedProjectBuildResults.Add(matches[0].Groups[1].Value);
            }
        }
    }
}
