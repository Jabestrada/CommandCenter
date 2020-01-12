﻿using CommandCenter.Infrastructure;

namespace CommandCenter.Tests.MockCommands {
    public class MockFailingCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockFailingCommand done with failure message", ReportType.DoneTaskWithFailure));
        }

        public override void Undo() {
            SendReport(this, new CommandReportArgs("MockFailingCommand undo success message", ReportType.UndoneTaskWithSuccess));
        }
    }
}