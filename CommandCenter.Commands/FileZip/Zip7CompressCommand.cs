﻿using CommandCenter.Commands.FileSystem;
using CommandCenter.Infrastructure;
using System;
using System.Text;

namespace CommandCenter.Commands.FileZip {
    public class Zip7CompressCommand : BaseCommand, IFileCompressionStrategy {
        //public bool DidTargetZipfileExistBeforeCompression { get; protected set; }

        protected IFileSystemCommandsStrategy FileSystemCommandsStrategy { get; set; }
        protected IFileCompressionStrategy FileCompressionStrategy { get; set; }

        public void setFileSystemCommandsStrategy(IFileSystemCommandsStrategy fileSystemCommandsStrategy) {
            if (fileSystemCommandsStrategy == null) throw new ArgumentNullException(nameof(fileSystemCommandsStrategy));

            FileSystemCommandsStrategy = fileSystemCommandsStrategy;
        }
        public void setFileCompressionStrategy(IFileCompressionStrategy fileCompressionStrategy) {
            if (fileCompressionStrategy == null) throw new ArgumentNullException(nameof(fileCompressionStrategy));

            FileCompressionStrategy = fileCompressionStrategy;
        }

        public string ExeLocation { get; protected set; }
        public string TargetZipfilename { get; protected set; }

        // SourcesToZip can be files or folders
        public string[] SourcesToZip { get; protected set; }

        public Zip7CompressCommand(string exeLocation, string targetZipfilename, params string[] sourcesToZip) {
            ExeLocation = exeLocation;
            TargetZipfilename = targetZipfilename;
            SourcesToZip = sourcesToZip;
            FileSystemCommandsStrategy = new FileSystemCommands();
            FileCompressionStrategy = this;
        }

        public override bool IsUndoable => true;

        public override void Do() {
            var result = FileCompressionStrategy.DoCompression(TargetZipfilename, SourcesToZip);
            DidCommandSucceed = result == 0;
            SendReport($"{ExeLocation} exit code {result}",
                            DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }

        public int DoCompression(string targetZipfilename, params string[] sourcesToZip) {
            var arguments = buildCommandLineArguments();
            string output, error;
            var result = runCompression(arguments, out output, out error);
            sendReportIfNeeded(arguments, output, error);
            return result;
        }

        private void sendReportIfNeeded(string arguments, string output, string error) {
            if (output?.Length > 0) {
                SendReport($"Dumping output stream of {ExeLocation} {arguments} => {output}", ReportType.Progress);
            }
            if (error?.Length > 0) {
                SendReport($"Dumping error stream of {ExeLocation} {arguments} => {error}", ReportType.Progress);
            }
        }

        private int runCompression(string arguments, out string output, out string error) {
            using (var cmdLine = new CommandLineProcess(ExeLocation, arguments)) {
                SendReport($"Running command {ExeLocation} {arguments} ...", ReportType.Progress);
                return cmdLine.Run(out output, out error);
            }
        }

        private string buildCommandLineArguments() {
            var escapedSourceFolders = new StringBuilder();
            for (var k = 0; k < SourcesToZip.Length; k++) {
                escapedSourceFolders.AppendFormat("\"{0}\"", SourcesToZip[k]);
                if (k != SourcesToZip.Length - 1) {
                    escapedSourceFolders.Append(" ");
                }
            }
            return $"a \"{TargetZipfilename}\" {escapedSourceFolders.ToString()}";
        }

        public override void Undo() {

        }
        public override void Cleanup() {
            // No cleanup
        }


    }
}
