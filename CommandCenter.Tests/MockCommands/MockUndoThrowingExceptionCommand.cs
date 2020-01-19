using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Dispatch;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockUndoThrowingExceptionCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockUndoThrowingExceptionCommand ReportType.DoneWithSuccess", ReportType.DoneTaskWithSuccess));
            DidCommandSucceed = true;
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
