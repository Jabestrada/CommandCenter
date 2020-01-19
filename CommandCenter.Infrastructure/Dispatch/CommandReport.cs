using System;

namespace CommandCenter.Infrastructure.Dispatch {
    public class CommandReport {
        public readonly DateTime ReportedOn;
        public CommandReport() {
            ReportedOn = DateTime.Now;
        }
        public BaseCommand Reporter { get; set; }
        public string Message { get; set; }
        public ReportType ReportType { get; set; }

    }
}
