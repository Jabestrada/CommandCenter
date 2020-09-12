using CommandCenter.Infrastructure;
using System.Diagnostics;

namespace CommandCenter.Commands.Console {
    public class ShellPassThroughCommand : BaseCommand {
        public string Command { get; protected set; }
        public string Arguments { get; protected set; }
        public override bool IsUndoable => false;

        public ShellPassThroughCommand(string cmd, string args) {
            Command = cmd;
            Arguments = args;
        }

        public override void Do() {
            var process = new Process() {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo() {
                    FileName = Command,
                    Arguments = Arguments,
                    UseShellExecute = true
                },
            };

            process.Start();
            DidCommandSucceed = true;
        }
    }
}
