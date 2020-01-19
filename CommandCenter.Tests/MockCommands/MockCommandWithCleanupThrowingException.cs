using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System;

namespace CommandCenter.Tests.MockCommands {
    public class MockCommandWithCleanupThrowingException : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
            throw new NotImplementedException();
        }

        public override void Do() {
            SendReport($"MockCommandWithCleanupThrowingException {Id} DoneCleanupWithSuccess", ReportType.DoneCleanupWithSuccess);
            DidCommandSucceed = true;
        }

        public override void Undo() {
            SendReport($"MockCommandWithCleanupThrowingException {Id} UndoneTaskWithSuccess", ReportType.UndoneTaskWithSuccess);

        }
    }
}
