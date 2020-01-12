using CommandCenter.Infrastructure;

namespace CommandCenter.InfrastructureTests.MockCommands {
    public class MockCommandWithCleanup : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
            SendReport($"MockCommandWithCleanup {Id} done cleanup with success", ReportType.DoneCleanupWithSuccess);
        }

        public override void Do() {
            SendReport($"MockCommandWithCleanup {Id} done task with success", ReportType.DoneTaskWithSuccess);
        }

        public override void Undo() {
            SendReport($"MockCommandWithCleanup {Id} Undone task with success", ReportType.DoneTaskWithSuccess);

        }
    }
}
