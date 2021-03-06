﻿using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockNonUndoableCommand : BaseCommand {
        public override bool IsUndoable => false;

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockNonUndoableCommand done with success message", ReportType.DoneTaskWithSuccess));
            DidCommandSucceed = true;
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
