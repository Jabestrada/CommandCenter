﻿using System;

namespace CommandCenter.Infrastructure.Tests.MockCommands {
    public class MockDoAndUndoThrowingExceptionCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            throw new NotImplementedException();
        }

        public override void Undo() {
            throw new NotImplementedException();
        }
    }
}
