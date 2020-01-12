using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Commands.FileSystem {
    public class FileCopyCommand : BaseCommand {
        public override bool IsUndoable => true;

        public override void Do() {
            throw new NotImplementedException();
        }

        public override void Undo() {
            throw new NotImplementedException();
        }

        public override void Cleanup() {
            throw new NotImplementedException();
        }
    }
}
