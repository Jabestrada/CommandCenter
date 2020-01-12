namespace CommandCenter.Infrastructure.Tests.MockCommands {
    public class MockSucceedingCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Cleanup() {
        }

        public override void Do() {
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} progress message", ReportType.Progress, 50));
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} progress message", ReportType.Progress, 100));
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} success message", ReportType.DoneTaskWithSuccess));
        }

        public override void Undo() {
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} undo success message", ReportType.UndoneTaskWithSuccess));
        }
    }
}
