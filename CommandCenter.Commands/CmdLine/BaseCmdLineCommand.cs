using CommandCenter.Commands.FileSystem;
using CommandCenter.Commands.FileSystem.BaseDefinitions;
using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System.Collections.Generic;

namespace CommandCenter.Commands.CmdLine {
    public abstract class BaseCmdLineCommand : BaseCommand {
        public override bool IsUndoable => false;

        public virtual bool ValidateExePath => true;

        protected virtual int SuccessExitCode => 0;

        protected IFileSystemCommandsStrategy FileSystemCommand = new FileSystemCommands();
        public List<string> CommandLineArguments { get; protected set; }
        public string Executable { get; protected set; }
        public int ExitCode { get; protected set; }

        public string ConstructedCommand {
            get {
                return $"{Executable} {string.Join(" ", CommandLineArguments)}";
            }
        }

        protected virtual void SetArguments() { }
        protected virtual void OnCommandWillRun() { }
        protected virtual void OnCommandDidRun() { }
        protected virtual void OnErrorStreamDataIn(string data) { }
        protected virtual void OnOutputStreamDataIn(string data) { }

        public BaseCmdLineCommand() {
            CommandLineArguments = new List<string>();
        }
        
        public sealed override void Do() {
            SetArguments();
            runCommand();
        }

        public override bool PreflightCheck() {
            if (ValidateExePath && !FileSystemCommand.FileExists(Executable)) {
                SendReport(this, $"{ShortName} will FAIL because executable {Executable} was not found", ReportType.DonePreFlightWithFailure);
                return false;
            }
            return true;
        }



        private void runCommand() {
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, ValidateExePath, string.Join(" ", CommandLineArguments))) {
                OnCommandWillRun();
                ExitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                DidCommandSucceed = ExitCode == SuccessExitCode;
                OnCommandDidRun();
            }
        }

        protected void errorStreamReceiver(string data) {
            OnErrorStreamDataIn(data);
        }

        protected void outputStreamReceiver(string data) {
            OnOutputStreamDataIn(data);
        }
    }
}

