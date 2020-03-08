namespace CommandCenter.Tests.MockCommands.PreFlightCheck {
    public class MockCommandHasPreflightCheckButFlagNotSet : BaseMockPreflightCommand {
        public override bool HasPreFlightCheck => false;
        public override bool PreflightCheck() {
            PreflightCheckRan = false;
            return false;
        }
    }
}
