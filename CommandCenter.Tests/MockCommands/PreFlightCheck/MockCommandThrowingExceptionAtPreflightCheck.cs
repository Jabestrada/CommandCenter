using System;

namespace CommandCenter.Tests.MockCommands.PreFlightCheck {
    public class MockCommandThrowingExceptionAtPreflightCheck : BaseMockPreflightCommand {
        public override bool HasPreFlightCheck => true;
        public override bool PreflightCheck() {
            PreflightCheckRan = true;
            throw new NotImplementedException();
        }
    }
}
