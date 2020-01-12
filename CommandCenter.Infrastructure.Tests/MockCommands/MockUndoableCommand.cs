﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CommandCenter.Infrastructure.Tests.MockCommands {
    public class MockUndoableCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            SendReport(this, new CommandReportArgs("MockUndoableCommand done with success message", ReportType.DoneTaskWithSuccess));
        }
        public override void Undo() {
            SendReport(this, new CommandReportArgs("MockUndoableCommand UndoWithSuccess message", ReportType.UndoneTaskWithSuccess));
        }
    }
}
