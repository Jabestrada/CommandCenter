using CommandCenter.Commands.FileSystem;
using CommandCenter.Infrastructure;
using System;
using System.Text;

namespace CommandCenter.Commands.FileZip {
    public class Zip7CompressCommand : BaseCommand, IFileCompressionStrategy {
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
            if (targetFileExists()) return;
            if (!allSourcesExist()) return;

            var result = FileCompressionStrategy.DoCompression(TargetZipfilename, SourcesToZip);
            reportResult(result);
        }

        public override void Undo() {
            if (DidCommandSucceed) FileSystemCommandsStrategy.FileDelete(TargetZipfilename);
        }

        public int DoCompression(string targetZipfilename, params string[] sourcesToZip) {
            var arguments = buildCommandLineArguments();
            var result = runCompression(arguments, outputStreamReceiver, errorStreamReceiver);
            return result;
        }

        private void outputStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                SendReport($"7zip info => {message}", ReportType.Progress);
            }
        }

        private void errorStreamReceiver(string message) {
            if (!string.IsNullOrWhiteSpace(message)) {
                SendReport($"7zip ERROR => {message}", ReportType.Progress);
            }
        }

        #region private methods
        private void reportResult(int result) {
            DidCommandSucceed = result == 0;
            SendReport($"{ExeLocation} exit code {result}",
                            DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }

        private bool allSourcesExist() {
            foreach (var source in SourcesToZip) {
                if (!FileSystemCommandsStrategy.FileExists(source) && !FileSystemCommandsStrategy.DirectoryExists(source)) {
                    SendReport($"Did not run {ExeLocation} because source file/folder {source} does not exist", ReportType.DoneTaskWithFailure);
                    DidCommandSucceed = false;
                    return false;
                }
            }
            return true;
        }

        private bool targetFileExists() {
            if (FileSystemCommandsStrategy.FileExists(TargetZipfilename)) {
                SendReport($"Did not run {ExeLocation} because output file {TargetZipfilename} already exists", ReportType.DoneTaskWithFailure);
                DidCommandSucceed = false;
                return true;
            }
            return false;
        }

        private int runCompression(string arguments, Action<string> outputStreamReceiver, Action<string> errorStreamReceiver) {
            using (var cmdLine = new CommandLineProcess(ExeLocation, arguments)) {
                SendReport($"Running command {ExeLocation} {arguments} ...", ReportType.Progress);
                return cmdLine.Run(outputStreamReceiver, errorStreamReceiver);
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
        #endregion
    }
}
