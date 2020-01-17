using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.UI.CmdLine {
    class Program {
        static void Main(string[] args) {
            var orchestrator = new CommandsOrchestratorCmdLine("CommandCenter.config", onReportReceived);
            try {
                bool result = orchestrator.Run();
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Commands " + (result ? "SUCCEEDED" : "FAILED"));
            }
            catch (Exception exc) {
                Console.WriteLine($"Commands FAILED with unhandled exception: {exc.Message}");
            }
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
        }

        private static void onReportReceived(BaseCommand command, CommandReportArgs args) {
            Console.WriteLine($"{args.ReportType}: {args.Message}");
        }
    }
}
