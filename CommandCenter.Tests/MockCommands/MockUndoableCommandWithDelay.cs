using CommandCenter.Infrastructure;
using System.Threading;

namespace CommandCenter.Tests.MockCommands {
    public class MockUndoableCommandWithUndoDelay : BaseCommand {
        public override bool IsUndoable => true;

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockUndoableCommand done with success message", ReportType.DoneTaskWithSuccess));
            DidCommandSucceed = true;
        }
        public override void Undo() {
            Thread.Sleep(1);
            SendReport(this, new CommandReportArgs("MockUndoableCommand UndoWithSuccess message", ReportType.UndoneTaskWithSuccess));
        }
    }
}
