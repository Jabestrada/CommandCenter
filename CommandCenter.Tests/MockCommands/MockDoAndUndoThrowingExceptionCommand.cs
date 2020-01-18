using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockDoAndUndoThrowingExceptionCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Do() {
            throw new NotImplementedException();
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
