using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Tests.MockCommands.PreFlightCheck {
    public class BaseMockPreflightCommand : BaseCommand {
        public bool PreflightCheckRan { get; protected set; }
        public override bool IsUndoable => throw new NotImplementedException();

        public override void Do() {
            throw new NotImplementedException();
        }

        public override bool HasPreFlightCheck => true;
        
    }
}
