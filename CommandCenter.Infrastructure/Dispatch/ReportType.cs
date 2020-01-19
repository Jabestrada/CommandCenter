namespace CommandCenter.Infrastructure.Dispatch {
    public enum ReportType {
        Cancel,
        Progress,
        DoneTaskWithSuccess,
        DoneTaskWithFailure,
        UndoneTaskWithSuccess,
        UndoneTaskWithFailure,
        DoneCleanupWithSuccess,
        DoneCleanupWithFailure,
        Error,
        Warning,
        Info
    }
}
