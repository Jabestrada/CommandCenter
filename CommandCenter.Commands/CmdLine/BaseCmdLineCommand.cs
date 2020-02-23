using CommandCenter.Infrastructure;
using System.Collections.Generic;

namespace CommandCenter.Commands.CmdLine {
    public abstract class BaseCmdLineCommand : BaseCommand {
        public override bool IsUndoable => false;

        public virtual bool ValidateExePath => true;

        protected virtual int SuccessExitCode => 0;

        public List<string> CommandLineArguments { get; protected set; }
        public string Executable { get; protected set; }
        public int ExitCode { get; protected set; }

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

        private void runCommand() {
            using (CommandLineProcessRunner cmd = new CommandLineProcessRunner(Executable, ValidateExePath, string.Join(" ", CommandLineArguments))) {
                OnCommandWillRun();
                ExitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                DidCommandSucceed = ExitCode == SuccessExitCode;
                OnCommandDidRun();
            }
        }

        private void errorStreamReceiver(string data) {
            OnErrorStreamDataIn(data);
        }

        private void outputStreamReceiver(string data) {
            OnOutputStreamDataIn(data);
        }
    }
}

