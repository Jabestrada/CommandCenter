using CommandCenter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Tests.MockCommands {
    public class MockCommandConsumingToken : BaseCommand {
        public override bool IsUndoable => throw new NotImplementedException();

        public string TokenizedArg { get; set; }
        public MockCommandConsumingToken(string arg) {
            TokenizedArg = arg;
        }
        public override void Do() {
            DidCommandSucceed = true;
        }
    }
}
