using CommandCenter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Commands.MsBuild {
    public class MsPublishWebAppCommand : BaseCommand {
        public string MsBuildExe { get; protected set; }
        public string Source { get; protected set; }
        public string Configuration { get; protected set; }
        public string PublishProfile { get; protected set; }
        public override bool IsUndoable => false;

        public MsPublishWebAppCommand(string msBuildExe, string sourceFile, string configuration, string publishProfile) {
            MsBuildExe = msBuildExe;
            Source = sourceFile;        // must be .csproj
            Configuration = configuration;
            PublishProfile = publishProfile;
        }

        public override void Do() {
            List<string> arguments = new List<string>() {
                $"/nologo",
                $"\"{Source}\"",
                $"/p:DeployOnBuild=true",
                $"/p:Configuration={Configuration}",
                $"/p:PublishProfile={PublishProfile}"
                //$"/p:Platform={Platform}",
                //$"/t:Clean,Build"
            };
            using (CommandLineProcess cmd = new CommandLineProcess(MsBuildExe, string.Join(" ", arguments))) {
                SendReport($"MsPublishWebAppCommand started. Project: '{Source}', Configuration: {Configuration}", ReportType.Progress);

                int exitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);

                DidCommandSucceed = exitCode == 0;
                var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
                SendReport($"MsPublishWebAppCommand {result} with exit code {exitCode}",
                           DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            // No-op
        }

        public override void Cleanup() {
            // No-op
        }

        private void outputStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                SendReport($"WebPublish info => {message}", ReportType.Progress);
            }
        }

        private void errorStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) { 
               SendReport($"WebPublish ERROR => {message}", ReportType.Progress);
            }
        }
    }
}
