using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Dispatch;

namespace CommandCenter.Tests.MockCommands {
    public class MockUndoableCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockUndoableCommand done with success message", ReportType.DoneTaskWithSuccess));
            DidCommandSucceed = true;
        }
        public override void Undo() {
            SendReport(this, new CommandReportArgs("MockUndoableCommand UndoWithSuccess message", ReportType.UndoneTaskWithSuccess));
        }
    }
}
