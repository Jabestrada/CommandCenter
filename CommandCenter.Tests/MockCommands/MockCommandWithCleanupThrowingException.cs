using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockCommandWithCleanupThrowingException : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
            throw new NotImplementedException();
        }

        public override void Do() {
            SendReport($"MockCommandWithCleanupThrowingException {Id} DoneCleanupWithSuccess", ReportType.DoneCleanupWithSuccess);
        }

        public override void Undo() {
            SendReport($"MockCommandWithCleanupThrowingException {Id} UndoneTaskWithSuccess", ReportType.UndoneTaskWithSuccess);

        }
    }
}
