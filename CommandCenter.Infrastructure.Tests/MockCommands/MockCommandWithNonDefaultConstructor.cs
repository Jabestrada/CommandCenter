using System;
using System.Collections.Generic;
using System.Text;

namespace CommandCenter.Infrastructure.Tests.MockCommands {
    public class MockCommandWithNonDefaultConstructor : BaseCommand {
        private readonly string _dummyArgs;
        public MockCommandWithNonDefaultConstructor(string dummyArgs) {
            _dummyArgs = dummyArgs;
        }

        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            SendReport(this, new CommandReportArgs($"Started work on ctor arg {_dummyArgs}", ReportType.Progress));
            SendReport(this, new CommandReportArgs($"Done work on ctor arg {_dummyArgs}", ReportType.DoneTaskWithSuccess));
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
