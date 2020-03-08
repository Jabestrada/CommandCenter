using System;

namespace CommandCenter.Tests.MockCommands.PreFlightCheck {
    public class MockCommandPreflightSucceeding : BaseMockPreflightCommand {
        public override bool IsUndoable => throw new NotImplementedException();
        public override bool HasPreFlightCheck => true;
        public override void Do() {
            throw new NotImplementedException();
        }
        public override bool PreflightCheck() {
            PreflightCheckRan = true;
            return true;
        }
    }
}
