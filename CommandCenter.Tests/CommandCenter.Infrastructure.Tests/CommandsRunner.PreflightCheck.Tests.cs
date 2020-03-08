using CommandCenter.Infrastructure.Orchestration;
using CommandCenter.Tests.MockCommands.PreFlightCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CommandCenter.Infrastructure.Tests.PreflightCheck {
    [TestClass]
    public class CommandsRunnerPreflightCheckTests {
        [TestMethod]
        public void itShouldReturnFalseIfAtLeastOneFailed() {
            var cmdSucceeding1 = new MockCommandPreflightSucceeding();
            var cmdFailing = new MockCommandPreflightFailing();
            var cmdSucceeding2 = new MockCommandPreflightSucceeding();

            var commands = new List<BaseCommand>();
            commands.Add(cmdSucceeding1);
            commands.Add(cmdFailing);
            commands.Add(cmdSucceeding2);

			var runner = new CommandsRunner(commands);

            var result = runner.RunPreflight();

            Assert.IsFalse(result);
        }

		[TestMethod]
        public void itShouldRunAllChecksEvenIfOneFailedMidway() {
            var cmdSucceeding1 = new MockCommandPreflightSucceeding();
            var cmdFailing = new MockCommandPreflightFailing();
            var cmdSucceeding2 = new MockCommandPreflightSucceeding();

            var commands = new List<BaseCommand>();
            commands.Add(cmdSucceeding1);
            commands.Add(cmdFailing);
            commands.Add(cmdSucceeding2);

			var runner = new CommandsRunner(commands);

            var result = runner.RunPreflight();

            Assert.IsFalse(result);
            Assert.IsTrue(cmdSucceeding1.PreflightCheckRan);
            Assert.IsTrue(cmdFailing.PreflightCheckRan);
            Assert.IsTrue(cmdSucceeding2.PreflightCheckRan);
        }

		[TestMethod]
		public void itShouldReturnFalseOnUnhandledExceptions() {
            var cmdSucceeding1 = new MockCommandPreflightSucceeding();
            var cmdFailing = new MockCommandThrowingExceptionAtPreflightCheck();
            var cmdSucceeding2 = new MockCommandPreflightSucceeding();

            var commands = new List<BaseCommand>();
            commands.Add(cmdSucceeding1);
            commands.Add(cmdFailing);
            commands.Add(cmdSucceeding2);

			var runner = new CommandsRunner(commands);

            var result = runner.RunPreflight();

            Assert.IsFalse(result);
            Assert.IsTrue(cmdSucceeding1.PreflightCheckRan);
            Assert.IsTrue(cmdFailing.PreflightCheckRan);
            Assert.IsTrue(cmdSucceeding2.PreflightCheckRan);
        }


		[TestMethod]
		public void itShouldRunOnlyCommandsWithHasPreflightCheckSet() {
            var cmdSucceeding1 = new MockCommandPreflightSucceeding();
            var cmdPreflightCheckFlagNotSet = new MockCommandHasPreflightCheckButFlagNotSet();
            var cmdSucceeding2 = new MockCommandPreflightSucceeding();

            var commands = new List<BaseCommand>();
            commands.Add(cmdSucceeding1);
            commands.Add(cmdPreflightCheckFlagNotSet);
            commands.Add(cmdSucceeding2);

			var runner = new CommandsRunner(commands);

            var result = runner.RunPreflight();

            Assert.IsTrue(result);
            Assert.IsTrue(cmdSucceeding1.PreflightCheckRan);
            Assert.IsFalse(cmdPreflightCheckFlagNotSet.PreflightCheckRan);
            Assert.IsTrue(cmdSucceeding2.PreflightCheckRan);
        }
    }
}
