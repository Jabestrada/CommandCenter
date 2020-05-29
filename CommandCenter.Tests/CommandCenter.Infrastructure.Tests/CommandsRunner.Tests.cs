using CommandCenter.Infrastructure.Orchestration;
using CommandCenter.Tests.MockCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
            Assert.IsTrue(!runner.Reports.Any(r => r.ReportType.IsUndoReport()));
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
            Assert.IsFalse(runner.WasCommandStarted(successfulCmd2));
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
            Assert.IsFalse(runner.Reports.Any(r => r.Reporter.Id == nonUndoableCommand.Id && r.ReportType.IsUndoReport()));
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == undoableCommand1.Id && r.ReportType.IsUndoReport())
                                .Count());
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == failingCmd.Id && r.ReportType.IsUndoReport())
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
                                .Where(r => r.Reporter.Id == undoableCommand1.Id && r.ReportType.IsUndoReport())
                                .Count());
            Assert.AreEqual(1, runner.Reports
                                .Where(r => r.Reporter.Id == failingCmd.Id && r.ReportType.IsUndoReport())
                                .Count());
            Assert.AreEqual(0, runner.Reports
                             .Where(r => r.Reporter.Id == undoableCommand2.Id && r.ReportType.IsUndoReport())
                             .Count());
        }

        [TestMethod]
        public void itShouldSetWasCommandStartedOnRanCommandsOnly() {
            var commands = new List<BaseCommand>();
            var command1 = new MockSucceedingCommand();
            commands.Add(command1);

            var failingCmd = new MockFailingCommand();
            commands.Add(failingCmd);

            var command2 = new MockUndoableCommand();
            commands.Add(command2);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsFalse(result);
            Assert.IsTrue(runner.WasCommandStarted(command1));
            Assert.IsTrue(runner.WasCommandStarted(failingCmd));
            Assert.IsFalse(runner.WasCommandStarted(command2));
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
            var firstCommandUndoReport = runner.Reports.First(r => r.ReportType.IsUndoReport() &&
                                                                   r.Reporter.Id == undoableCommand1.Id);
            var latestUndoReportTimestamp = runner.Reports.Max(r => r.ReportedOn);
            var earliestUndoReport = runner.Reports.Where(r => r.ReportType.IsUndoReport())
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

            Assert.AreEqual(3, runner.Reports.Where(r => r.ReportType.IsUndoReport()).Count());
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
            Assert.IsTrue(runner.Reports.Any(r => r.ReportType.IsCleanupReport() && r.Reporter.Id == command1.Id));
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
            Assert.IsTrue(runner.Reports.Any(r => r.ReportType.IsCleanupReport() && r.Reporter.Id == command1.Id));
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
            Assert.IsTrue(runner.WasCommandStarted(command1));
            Assert.IsTrue(runner.Reports.Any(r => r.ReportType.IsCleanupReport() && r.Reporter.Id == command1.Id));


            Assert.IsTrue(!runner.WasCommandStarted(command3));
            Assert.IsFalse(runner.Reports.Any(r => r.ReportType.IsCleanupReport() && r.Reporter.Id == command3.Id));


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
            var firstCommandCleanupReport = runner.Reports.First(r => r.ReportType.IsCleanupReport() &&
                                                                   r.Reporter.Id == command1.Id);
            var latestCleanupReportTimestamp = runner.Reports.Max(r => r.ReportedOn);
            var earliestCleanupReport = runner.Reports.Where(r => r.ReportType.IsCleanupReport())
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

        [TestMethod]
        public void itShouldRunEnabledCommandsOnly() {
            var commands = new List<BaseCommand>();
            var command1 = new MockSucceedingCommand();
            command1.Enabled = true;
            commands.Add(command1);

            var command2 = new MockSucceedingCommand();
            command2.Enabled = false;
            commands.Add(command2);

            var runner = new CommandsRunner(commands);
            var result = runner.Run();

            Assert.IsTrue(result);
            Assert.IsTrue(runner.WasCommandStarted(command1));
            Assert.IsFalse(runner.WasCommandStarted(command2));
        }

        [TestMethod]
        public void itShouldSupportCancellation() {
            var commands = new List<BaseCommand>();
            var command1 = new MockSucceedingCommand();
            commands.Add(command1);

            var command2 = new MockSleepingCommand(2000);
            commands.Add(command2);

            var command3 = new MockSucceedingCommand();
            commands.Add(command3);

            var runner = new CommandsRunner(commands);
            bool result = true;

            ThreadStart starter = new ThreadStart(() => {
                result = runner.Run();
            });

            var thread = new Thread(starter);
            thread.Start();
            // Sleep to give time for command1 to succeed.
            Thread.Sleep(500);
            // Cancel before command2 completes
            runner.Cancel();

            thread.Join();

            // Cancelled means failed result
            Assert.IsFalse(result);
            Assert.IsTrue(runner.WasCommandStarted(command1));
            // Cancellation should trigger Undo
            Assert.IsTrue(runner.Reports.Any(r => r.Reporter == command1 && r.ReportType.IsUndoReport()));
            Assert.IsTrue(runner.WasCommandStarted(command2));
            // Cancellation should not run next command in the chain.
            Assert.IsFalse(runner.WasCommandStarted(command3));
        }
    }
}
