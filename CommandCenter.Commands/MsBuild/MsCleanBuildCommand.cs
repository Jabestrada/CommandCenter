﻿using CommandCenter.Infrastructure;
using System.Collections.Generic;

namespace CommandCenter.Commands.MsBuild {
    public class MsCleanBuildCommand : BaseCommand {
        public string MsBuildExe { get; protected set; }
        public string Source { get; protected set; }
        public string Configuration { get; protected set; }
        //public string Platform { get; protected set; }
        public override bool IsUndoable => false;

        public MsCleanBuildCommand(string msBuildExe, string sourceFile, string configuration) {
            MsBuildExe = msBuildExe;
            Source = sourceFile;        // can be .sln or .csproj
            //Platform = platform;
            Configuration = configuration;
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

                int exitCode = cmd.Run(out string processOutput, out string processError);
                //if (!string.IsNullOrEmpty(processOutput)) {
                //    SendReport($"Dumping output stream of MsCleanBuildCommand: {processOutput}", ReportType.Progress);
                //}
                if (!string.IsNullOrEmpty(processError)) {
                    SendReport($"Dumping error stream of MsCleanBuildCommand: {processError}", ReportType.Progress);
                }
                DidCommandSucceed = exitCode == 0;
                var result = DidCommandSucceed ? "SUCCEEDED" : "FAILED";
                SendReport($"MsCleanBuildCommand {result} with exit code {exitCode}",
                           DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
            }
        }

        public override void Undo() {
            // Do nothing
        }

        public override void Cleanup() {
            // Do nothing
        }
    }
}
