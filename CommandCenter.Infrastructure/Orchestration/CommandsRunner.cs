using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommandCenter.Infrastructure.Orchestration {
    public class CommandsRunner : BaseCommand {
        public readonly List<CommandReport> Reports;

        protected Stack<BaseCommand> InvokedCommandsStackForUndo = new Stack<BaseCommand>();
        protected Stack<BaseCommand> InvokedCommandsStackForCleanup = new Stack<BaseCommand>();

        protected List<BaseCommand> Commands = new List<BaseCommand>();

        public TimeSpan LastRunElapsedTime { get; protected set; }
        public CommandsRunner(List<BaseCommand> commands) {
            Commands = commands;
            Reports = new List<CommandReport>();
            registerCommands();
        }

        public bool HasError => Commands.Any(c => c.WasCommandStarted && !c.DidCommandSucceed);

        public bool Run() {
            var timer = new Stopwatch();
            timer.Start();

            resetState();
            runCommands();
            undoCommandsIfNeeded();
            runCleanup();

            timer.Stop();
            LastRunElapsedTime = timer.Elapsed;

            return !HasError;
        }

        public bool RunPreflight() {
            var timer = new Stopwatch();
            timer.Start();

            resetState();
            var preFlightResult = runPreflightCheck();

            timer.Stop();
            LastRunElapsedTime = timer.Elapsed;

            return preFlightResult;
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
                if (!command.Enabled) {
                    SendReport($"Command [{command.ShortDescription}] was not run because it is disabled", ReportType.Progress);
                    continue;
                }

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

        private bool runPreflightCheck() {
            bool preFlightResult = true;
            foreach (var command in Commands) {
                try {
                    if (command.HasPreFlightCheck) {
                        var currentPreFlightResult = command.PreflightCheck();
                        if (!currentPreFlightResult) preFlightResult = false;
                    }
                    else {

                        reportCommand(command, ReportType.DonePreflightWithSuccess, $"{command.ShortName} has no preflight checks");
                    }
                }
                catch (Exception e) {
                    reportCommand(command, ReportType.DonePreFlightWithFailure, $"{command.ShortName}: Exception thrown while running PreflightCheck(): {e.Message}");
                    preFlightResult = false;

                }
            }
            return preFlightResult;
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
}
