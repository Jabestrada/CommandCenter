using CommandCenter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandCenter.Commands.MsBuild {
    public class MsCleanRebuildCommand : BaseCommand {
        private const string FAILED_PROJECT_BUILD_PATTERN = @"Done Building Project ""(.*\.csproj)"".*FAILED";
        public string MsBuildExe { get; protected set; }
        public string Source { get; protected set; }
        public string Configuration { get; protected set; }

        public List<string> FailedProjectBuildResults { get; protected set; }

        //public string Platform { get; protected set; }
        public override bool IsUndoable => false;
        public MsCleanRebuildCommand(string msBuildExe, string sourceFile, string configuration) {
            MsBuildExe = msBuildExe;
            Source = sourceFile;        // can be .sln or .csproj
            //Platform = platform;
            Configuration = configuration;
            FailedProjectBuildResults = new List<string>();
        }

        public override void Do() {
            List<string> arguments = new List<string>() {
                $"/nologo",
                $"\"{Source}\"",
                $"/p:Configuration={Configuration}",
                //$"/p:Platform={Platform}",
                $"/t:Clean,Build"
            };
            using (CommandLineProcess cmd = new CommandLineProcess(MsBuildExe, string.Join(" ", arguments))) {
                SendReport($"Build started. Solution: '{Source}', Configuration: {Configuration}", ReportType.Progress);

                int exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);

                DidCommandSucceed = exitCode == 0;
                var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
                if (!DidCommandSucceed && FailedProjectBuildResults.Any()) {
                    foreach (string failedBuildProject in FailedProjectBuildResults) {
                        SendReport($"FAILED PROJECT BUILD: {failedBuildProject}", ReportType.Progress);
                    }
                }
                SendReport($"MsCleanRebuildCommand {result} with exit code {exitCode}",
                           DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
            }
        }

        private void outputStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                captureFailedProject(message);
                SendReport($"MsCleanRebuild info => {message}", ReportType.Progress);
            }
        }

        private void errorStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                SendReport($"MsCleanRebuild ERROR => {message}", ReportType.Progress);
            }
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
