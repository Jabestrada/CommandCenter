using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Tests.MockCommands {
    public class MockSucceedingCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Do() {
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} progress message", ReportType.Progress, 50));
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} progress message", ReportType.Progress, 100));
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} success message", ReportType.DoneTaskWithSuccess));
            DidCommandSucceed = true;
        }

        public override void Undo() {
            SendReport(this, new CommandReportArgs($"MockSucceedingCommand {Id} undo success message", ReportType.UndoneTaskWithSuccess));
        }
    }
}
