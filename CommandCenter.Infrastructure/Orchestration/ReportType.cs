namespace CommandCenter.Infrastructure.Orchestration {
    public enum ReportType {
        Cancel,
        Progress,
        DoneTaskWithSuccess,
        DoneTaskWithFailure,
        UndoneTaskWithSuccess,
        UndoneTaskWithFailure,
        DoneCleanupWithSuccess,
        DoneCleanupWithFailure,
        DonePreflightWithSuccess,
        DonePreFlightWithFailure,
        Error,
        Warning,
        Info
    }
}
