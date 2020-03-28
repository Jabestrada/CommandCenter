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
}
