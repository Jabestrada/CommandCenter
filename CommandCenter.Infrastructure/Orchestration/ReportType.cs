namespace CommandCenter.Infrastructure.Orchestration {
    public enum ReportType {
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
