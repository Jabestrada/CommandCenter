using CommandCenter.Infrastructure;
using System.Collections.Generic;

namespace CommandCenter.Commands.CmdLine {
    public abstract class BaseCmdLineCommand : BaseCommand {
        public override bool IsUndoable => false;
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
            using (CommandLineProcess cmd = new CommandLineProcess(Executable, string.Join(" ", CommandLineArguments))) {
                OnCommandWillRun();
                ExitCode = cmd.Run(outputStreamReceiver, errorStreamReceiver);
                DidCommandSucceed = ExitCode == 0;
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

