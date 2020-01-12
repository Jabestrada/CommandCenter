using CommandCenter.Infrastructure;
using System.Threading;

namespace CommandCenter.Tests.MockCommands {
    public class MockCommandWithCleanup : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
            Thread.Sleep(1);
            SendReport($"MockCommandWithCleanup {Id} Done cleanup with success", ReportType.DoneCleanupWithSuccess);
        }

        public override void Do() {
            SendReport($"MockCommandWithCleanup {Id} Done task with success", ReportType.DoneTaskWithSuccess);
        }

        public override void Undo() {
            SendReport($"MockCommandWithCleanup {Id} Undone task with success", ReportType.DoneTaskWithSuccess);

        }
    }
}
