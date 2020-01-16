using CommandCenter.Infrastructure;
using System;

namespace CommandCenter.UI.CmdLine {
    class Program {
        static void Main(string[] args) {
            var orchestrator = new CommandsOrchestrator("CommandCenter.config", onReportReceived);
            try {
                if (orchestrator.Run()) {
                    Console.WriteLine("Commands SUCCEEDED");
                }
                else {
                    Console.WriteLine("Commands FAILED");
                }
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
