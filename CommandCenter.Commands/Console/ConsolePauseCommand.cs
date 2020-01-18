using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.Commands.Console {
    public class ConsolePauseCommand : BaseCommand {
        public string Message { get; protected set; }
        public override bool IsUndoable => false;

        public ConsolePauseCommand() 
            : this("Press any key to continue...") { 
        }
        
        public ConsolePauseCommand(string message) {
            Message = message; 
        }

        public override void Do() {
            System.Console.WriteLine(Message);
            System.Console.ReadLine();
            DidCommandSucceed = true;
        }
    }
}
