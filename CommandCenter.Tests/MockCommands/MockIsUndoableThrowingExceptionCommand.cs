using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockIsUndoableThrowingExceptionCommand : BaseCommand {
        public override bool IsUndoable => throw new NotImplementedException();
        public override void Do() {
            SendReport(this, new CommandReportArgs("Finished!", ReportType.DoneTaskWithSuccess));
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
