using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;

namespace CommandCenter.Commands.Console {
    public class ConditionalConsolePauseCommand : BaseCommand {
        public override bool IsUndoable => false;

        public string MessagePrompt { get; set; }
        public ConditionalConsolePauseCommand() : this("Press Esc to cancel or Enter to continue ..."){
            
        }
        public ConditionalConsolePauseCommand(string messagePrompt) {
            MessagePrompt = messagePrompt;
        }

        public override void Do() {
            System.Console.WriteLine(MessagePrompt);
            var keyInfo = System.Console.ReadKey(true);
            while (keyInfo.Key != System.ConsoleKey.Enter && keyInfo.Key != System.ConsoleKey.Escape) { 
                System.Console.WriteLine(MessagePrompt);
                keyInfo = System.Console.ReadKey(true);
            }
            DidCommandSucceed = keyInfo.Key == System.ConsoleKey.Enter;
            if (DidCommandSucceed) {
                SendReport("Continuing execution of other commands", ReportType.DoneTaskWithSuccess);
            }
            else { 
                SendReport("User aborted execution of other commands", ReportType.DoneTaskWithFailure);
            }
        }
    }
}
