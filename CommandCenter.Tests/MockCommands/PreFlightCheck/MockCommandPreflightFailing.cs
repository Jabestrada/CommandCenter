using System;

namespace CommandCenter.Tests.MockCommands.PreFlightCheck {
    public class MockCommandPreflightFailing : BaseMockPreflightCommand {
        public override bool IsUndoable => throw new NotImplementedException();

        public override void Do() {
            throw new NotImplementedException();
        }

        public override bool HasPreFlightCheck => true;
        public override bool PreFlightCheck() {
            PreflightCheckRan = true;
            return false;
        }
    }
}
