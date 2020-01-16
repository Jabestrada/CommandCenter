using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandCenter.Infrastructure {
    public class CommandsRunner : BaseCommand {
        public readonly List<CommandReport> Reports;

        protected Stack<BaseCommand> InvokedCommandsStackForUndo = new Stack<BaseCommand>();
        protected Stack<BaseCommand> InvokedCommandsStackForCleanup = new Stack<BaseCommand>();

        protected List<BaseCommand> Commands = new List<BaseCommand>();

        public CommandsRunner(List<BaseCommand> commands) {
            Commands = commands;
            Reports = new List<CommandReport>();
            registerCommands();
        }

        //public bool HasError => Reports.Any(r => r.ReportType == ReportType.DoneTaskWithFailure);

        public bool HasError => Commands.Any(c => !c.DidCommandSucceed);

        public bool Run() {
            resetState();
            runCommands();
            undoCommandsIfNeeded();
            runCleanup();

            return !HasError;
        }



        #region Non-public methods
        private void registerCommands() {
            foreach (var command in Commands) {
                command.OnReportSent += reportSentHandler;
            }
        }
        private void reportSentHandler(BaseCommand command, CommandReportArgs args) {
            Reports.Add(new CommandReport {
                Reporter = command,
                ReportType = args.ReportType,
                Message = args.Message
            });
            SendReport(command, new CommandReportArgs(args.Message, args.ReportType));
        }
        private void runCommands() {
            foreach (var command in Commands) {
                InvokedCommandsStackForUndo.Push(command);
                InvokedCommandsStackForCleanup.Push(command);
                command.WasCommandStarted = true;
                try {
                    command.Do();
                }
                catch (Exception e) {
                    reportCommand(command, ReportType.DoneTaskWithFailure, e.Message);
                }

                if (!command.DidCommandSucceed) break;
            }
        }
        private void undoCommandsIfNeeded() {
            if (!HasError) return;

            while (InvokedCommandsStackForUndo.Any()) {
                var invokedCommand = InvokedCommandsStackForUndo.Pop();
                if (!isCommandUndoable(invokedCommand)) continue;

                undoCommand(invokedCommand);
            }
        }

        private void runCleanup() {
            while (InvokedCommandsStackForCleanup.Any()) { 
                var invokedCommand = InvokedCommandsStackForCleanup.Pop();
                try {
                    invokedCommand.Cleanup();
                }
                catch (Exception exc) {
                    reportCommand(invokedCommand, ReportType.DoneCleanupWithFailure, exc.Message);
                }
            }
        }

        private void undoCommand(BaseCommand invokedCommand) {
            try {
                invokedCommand.Undo();
            }
            catch (Exception exc) {
                reportCommand(invokedCommand, ReportType.UndoneTaskWithFailure, exc.Message);
            }
        }

        private bool isCommandUndoable(BaseCommand invokedCommand) {
            try {
                return invokedCommand.IsUndoable;
            }
            catch (Exception e) {
                reportCommand(invokedCommand, ReportType.UndoneTaskWithFailure, e.Message);
            }
            return false;
        }

        private void reportCommand(BaseCommand command, ReportType reportType, string message) {
            Reports.Add(new CommandReport {
                Reporter = command,
                ReportType = reportType,
                Message = message
            });

            SendReport(command, new CommandReportArgs(message, reportType));
        }

        private void resetState() {
            Reports.Clear();
            InvokedCommandsStackForUndo.Clear();
            InvokedCommandsStackForCleanup.Clear();
        }

        #endregion

        #region BaseCommand implementation
        public override bool IsUndoable => Commands.Any(c => c.IsUndoable);

        public override void Do() {
            if (!Run()) {
                throw new ApplicationException("One or more commands failed. Inspect Reports property for details.");
            }
        }
        public override void Undo() {
            undoCommandsIfNeeded();
        }

        public override void Cleanup() {
            runCleanup();
        }
        #endregion
    }

    #region Ancillary types
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

    public class CommandReport {
        public readonly DateTime ReportedOn;
        public CommandReport() {
            ReportedOn = DateTime.Now;
        }
        public BaseCommand Reporter { get; set; }
        public string Message { get; set; }
        public ReportType ReportType { get; set; }

    }
    #endregion
}
