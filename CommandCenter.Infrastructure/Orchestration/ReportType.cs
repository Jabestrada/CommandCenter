namespace CommandCenter.Infrastructure.Orchestration {
    public enum ReportType {
        RunningCommandStatistics,
        Progress,
        DoneTaskWithSuccess,
        DoneTaskWithFailure,
        UndoneTaskWithSuccess,
        UndoneTaskWithFailure,
        DoneCleanupWithSuccess,
        DoneCleanupWithFailure,
        DonePreflightWithSuccess,
        DonePreFlightWithFailure
    }
    public static class ReportTypeExtensions {
        public static bool IsUndoReport(this ReportType r) {
            return r == ReportType.UndoneTaskWithFailure || r == ReportType.UndoneTaskWithSuccess;
        }
        public static bool IsCleanupReport(this ReportType r) {
            return r == ReportType.DoneCleanupWithFailure || r == ReportType.DoneCleanupWithSuccess;
        }
        public static bool IsDoneTaskReport(this ReportType r) {
            return r == ReportType.DoneTaskWithFailure || r == ReportType.DoneTaskWithSuccess;
        }
        public static bool IsDonePreflightTaskReport(this ReportType r) {
            return r == ReportType.DonePreFlightWithFailure|| r == ReportType.DonePreFlightWithFailure;
        }
    }
}
