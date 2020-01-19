using CommandCenter.Infrastructure.Orchestration;
using CommandCenter.Tests.MockCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CommandCenter.Infrastructure.Tests {
    [TestClass]
    public class CommandsRunnerTests {
        [TestMethod]
        public void itShouldReturnTrueWhenThereAreNoErrors() {
            var successfulCmd = new MockSucceedingCommand();
            var commands = new List<BaseCommand>();
            commands.Add(successfulCmd);
            var runner = new CommandsRunner(commands);

            var result = runner.Run();

            Assert.IsTrue(result);
            Assert.AreEqual(result, !runner.HasError);
        }

        [TestMethod]
        public void itShouldReturnFalseWhenThereAreErrors() {
            var failingCmd = new MockFailingCommand();
            var commands = new List<BaseCommand>();
            commands.Add(failingCmd);
            var runner = new CommandsRunner(commands);

            var result = runner.Run();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void itShouldBeIdempotent() {
            var successfulCmd = new MockSucceedingCommand();
            var commands = new List<BaseCommand>();
            commands.Add(successfulCmd);
            var runner = new CommandsRunner(commands);

            var result = runner.Run();

            Assert.IsTrue(result);
            Assert.AreEqual(result, !runner.HasError);
            var firstRunReportCount = runner.Reports.Count();

            result = runner.Run();

            Assert.IsTrue(result);
            Assert.AreEqual(result, !runner.HasError);
            var secondRunReportCount = runner.Reports.Count();
            Assert.AreEqual(firstRunReportCount, secondRunReportCount);
        }


        [TestMethod]
        public void itShouldNotCallUndoWhenThereAreNoErrors() {
            var successfulCmd = new MockSucceedingCommand();
            var commands = new List<BaseCommand>();
            commands.Add(successfulCmd);
            var runner = new CommandsRunner(commands);

            var result = runner.Run();

            Assert.IsTrue(result);
            Assert.IsTrue(!runner.Reports.Any(r => isUndoReport(r.ReportType)));
        }

        [TestMethod]
        public void itShouldDiscontinueWhenCommandFails() {
            var commands = new List<BaseCommand>();
            var successfulCmd1 = new MockSucceedingCommand();
            commands.Add(successfulCmd1);
            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);
            var successfulCmd2 = new MockSucceedingCommand();
            commands.Add(successfulCmd2);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);

            // There should be no report from successfulCmd2 whatsoever because of failingCmd.
            Assert.IsFalse(successfulCmd2.WasCommandStarted);
        }

        [TestMethod]
        public void itShouldCallUndoablesOnlyOnFailure() {

            var commands = new List<BaseCommand>();
            var undoableCommand1 = new MockUndoableCommand();
            commands.Add(undoableCommand1);

            var nonUndoableCommand = new MockNonUndoableCommand();
            commands.Add(nonUndoableCommand);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);


            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            Assert.IsFalse(runner.Reports.Any(r => r.Reporter.Id == nonUndoableCommand.Id && isUndoReport(r.ReportType)));
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == undoableCommand1.Id && isUndoReport(r.ReportType))
                                .Count());
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == failingCmd.Id && isUndoReport(r.ReportType))
                                .Count());
        }



        [TestMethod]
        public void itShouldCallUndoOnCommandsThatRanOnly() {
            var commands = new List<BaseCommand>();
            var undoableCommand1 = new MockUndoableCommand();
            commands.Add(undoableCommand1);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);

            var undoableCommand2 = new MockUndoableCommand();
            commands.Add(undoableCommand2);


            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == undoableCommand1.Id && isUndoReport(r.ReportType))
                                .Count());
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == failingCmd.Id && isUndoReport(r.ReportType))
                                .Count());
            Assert.AreEqual(0, runner.Reports
                             .Where(r => r.Reporter.Id == undoableCommand2.Id && isUndoReport(r.ReportType))
                             .Count());
        }

        [TestMethod]
        public void itShouldSetWasCommandStartedOnRanCommandsOnly() {
            var commands = new List<BaseCommand>();
            var command1= new MockSucceedingCommand();
            commands.Add(command1);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);

            var command2  = new MockUndoableCommand();
            commands.Add(command2);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            Assert.IsTrue(command1.WasCommandStarted);
            Assert.IsTrue(failingCmd.WasCommandStarted);
            Assert.IsFalse(command2.WasCommandStarted);
        }

        [TestMethod]
        public void itShouldCallUndoOnFILOBasis() {
            var commands = new List<BaseCommand>();
            var undoableCommand1 = new MockUndoableCommand();
            commands.Add(undoableCommand1);

            var undoableCommand2 = new MockUndoableCommandWithUndoDelay();
            commands.Add(undoableCommand2);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            var firstCommandUndoReport = runner.Reports.First(r => isUndoReport(r.ReportType) &&
                                                                   r.Reporter.Id == undoableCommand1.Id);
            var latestUndoReportTimestamp = runner.Reports.Max(r => r.ReportedOn);
            var earliestUndoReport = runner.Reports.Where(r => isUndoReport(r.ReportType))
                                                 .OrderBy(r => r.ReportedOn)
                                                 .First();

            Assert.IsTrue(firstCommandUndoReport.ReportedOn > earliestUndoReport.ReportedOn);
            Assert.AreEqual(firstCommandUndoReport.ReportedOn, latestUndoReportTimestamp);
        }

        [TestMethod]
        public void itShouldBeResilientWhenDoOrUndoThrowsException() {
            var commands = new List<BaseCommand>();

            var doThrowingExceptionCommand = new MockDoAndUndoThrowingExceptionCommand();
            commands.Add(doThrowingExceptionCommand);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();
            Assert.IsFalse(result);

            // Implied assertion: No exceptions raised.
        }

        [TestMethod]
        public void itShouldContinueUndoEvenWhenOneUndoFails() {
            var commands = new List<BaseCommand>();
            var undoableCommand1 = new MockUndoableCommand();
            commands.Add(undoableCommand1);

            var undoableCommand2 = new MockUndoThrowingExceptionCommand();
            commands.Add(undoableCommand2);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);

            Assert.AreEqual(3, runner.Reports.Where(r => isUndoReport(r.ReportType)).Count());
        }

        [TestMethod]
        public void itShouldBeResilientWhenIsUndoableThrowException() {

            var commands = new List<BaseCommand>();
            commands.Add(new MockIsUndoableThrowingExceptionCommand());
            commands.Add(new MockFailingCommand());

            var runner = new CommandsRunner(commands);
            var result = runner.Run();
            Assert.IsFalse(result);

        }

        [TestMethod]
        public void itShouldCallCleanupOfInvokedCommandsOnSuccess() {
            var command1 = new MockCommandWithCleanup();
            var commands = new List<BaseCommand>();
            commands.Add(command1);
            var runner = new CommandsRunner(commands);

            var result = runner.Run();

            Assert.IsTrue(result);
            Assert.IsTrue(runner.Reports.Any(r => isCleanupReport(r.ReportType) && r.Reporter.Id == command1.Id));
        }

        [TestMethod]
        public void itShouldCallCleanupOfInvokedCommandsOnFailure() {
            var commands = new List<BaseCommand>();
            var command1 = new MockCommandWithCleanup();
            commands.Add(command1);
            var command2 = new MockFailingCommand();
            commands.Add(command2);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            Assert.IsTrue(runner.Reports.Any(r => isCleanupReport(r.ReportType) && r.Reporter.Id == command1.Id));
        }

        [TestMethod]
        public void itShouldCallCleanupOfInvokedCommandsOnly() {
            var commands = new List<BaseCommand>();
            var command1 = new MockCommandWithCleanup();
            commands.Add(command1);
            var command2 = new MockFailingCommand();
            commands.Add(command2);
            var command3 = new MockCommandWithCleanup();
            commands.Add(command3);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            Assert.IsTrue(command1.WasCommandStarted);
            Assert.IsTrue(runner.Reports.Any(r => isCleanupReport(r.ReportType) && r.Reporter.Id == command1.Id));


            Assert.IsTrue(!command3.WasCommandStarted);
            Assert.IsFalse(runner.Reports.Any(r => isCleanupReport(r.ReportType) && r.Reporter.Id == command3.Id));


        }

        [TestMethod]
        public void itShouldCallCleanupOnFILOBasis() {
            var commands = new List<BaseCommand>();
            var command1 = new MockCommandWithCleanup();
            commands.Add(command1);

            var command2 = new MockCommandWithCleanup();
            commands.Add(command2);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            var firstCommandCleanupReport = runner.Reports.First(r => isCleanupReport(r.ReportType) &&
                                                                   r.Reporter.Id == command1.Id);
            var latestCleanupReportTimestamp = runner.Reports.Max(r => r.ReportedOn);
            var earliestCleanupReport = runner.Reports.Where(r => isCleanupReport(r.ReportType))
                                                 .OrderBy(r => r.ReportedOn)
                                                 .First();

            Assert.IsTrue(firstCommandCleanupReport.ReportedOn > earliestCleanupReport.ReportedOn);
            Assert.AreEqual(firstCommandCleanupReport.ReportedOn, latestCleanupReportTimestamp);
        }

        [TestMethod]
        public void itShouldHandleExceptionsDuringCleanupButNotAffectOverallOutcome() {
            var commands = new List<BaseCommand>();
            var command1 = new MockCommandWithCleanup();
            commands.Add(command1);

            var command2 = new MockCommandWithCleanupThrowingException();
            commands.Add(command2);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsTrue(result);
        }

        #region Helper methods
        private bool isUndoReport(ReportType reportType) {
            return reportType == ReportType.UndoneTaskWithSuccess || reportType == ReportType.UndoneTaskWithFailure;
        }

        private bool isCleanupReport(ReportType reportType) {
            return reportType == ReportType.DoneCleanupWithFailure || reportType == ReportType.DoneCleanupWithSuccess;
        }
        #endregion
    }
}
