using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommandCenter.Infrastructure.Tests.MockCommands {
    public class MockUndoableCommandWithUndoDelay : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockUndoableCommand done with success message", ReportType.DoneTaskWithSuccess));
        }
        public override void Undo() {
            Thread.Sleep(1);
            SendReport(this, new CommandReportArgs("MockUndoableCommand UndoWithSuccess message", ReportType.UndoneTaskWithSuccess));
        }
    }
}
