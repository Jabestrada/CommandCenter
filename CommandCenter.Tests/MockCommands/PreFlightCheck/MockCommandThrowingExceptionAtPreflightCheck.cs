using System;

namespace CommandCenter.Tests.MockCommands.PreFlightCheck {
    public class MockCommandThrowingExceptionAtPreflightCheck : BaseMockPreflightCommand {
        public override bool HasPreFlightCheck => true;
        public override bool PreFlightCheck() {
            PreflightCheckRan = true;
            throw new NotImplementedException();
        }
    }
}
