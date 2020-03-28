using CommandCenter.Infrastructure;
using System.Threading;

namespace CommandCenter.Tests.MockCommands {
    public class MockSleepingCommand : BaseCommand {
        public override bool IsUndoable => false;

        public int Delay { get; set; }
        public MockSleepingCommand(int delay = 1000) {
            Delay = delay;
        }
        public override void Do() {
            Thread.Sleep(Delay);
            DidCommandSucceed = true;
        }
    }
}
