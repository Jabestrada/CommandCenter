using System;
using System.Collections.Generic;
using System.Text;

namespace CommandCenter.Infrastructure.Tests.MockCommands {
    public class MockUndoThrowingExceptionCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockUndoThrowingExceptionCommand ReportType.DoneWithSuccess", ReportType.DoneTaskWithSuccess));
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
