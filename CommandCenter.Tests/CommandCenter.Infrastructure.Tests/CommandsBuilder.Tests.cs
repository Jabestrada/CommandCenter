using CommandCenter.Infrastructure.Configuration;
using CommandCenter.Infrastructure.Factory;
using CommandCenter.Tests.MockCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CommandCenter.Infrastructure.Tests {
    [TestClass]
    public class CommandsBuilderTests {
        [TestMethod]
        public void itShouldCreateCorrectlyConfiguredCommands() {
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = "CommandCenter.Tests, CommandCenter.Tests.MockCommands.MockSucceedingCommand"
            };
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var cmdBuilder = new CommandsBuilder(cmdConfigurations);

            var result = cmdBuilder.BuildCommands();

            Assert.IsNotNull(result.First());
        }

        [TestMethod]
        public void itShouldCreateCommandsWithDefaultConstructor() {
            itShouldCreateCorrectlyConfiguredCommands();
        }

        [TestMethod]
        public void itShouldCreateCommandsWithNonDefaultConstructor() {
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = "CommandCenter.Tests, CommandCenter.Tests.MockCommands.MockCommandWithNonDefaultConstructor"
            };
            cmdConfiguration.ConstructorArgs.Add("arg1", "dummy constructorArg value 1");

            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var cmdBuilder = new CommandsBuilder(cmdConfigurations);

            var result = cmdBuilder.BuildCommands();

            Assert.IsNotNull(result.First());
        }

        [TestMethod]
        [ExpectedException(typeof(TypeLoadFailedException))]
        public void itShouldRaiseExceptionWhenConstructorArgsAreWrong() {
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = "CommandCenter.Tests, CommandCenter.Tests.MockCommands.MockCommandWithNonDefaultConstructor"
            };
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var cmdBuilder = new CommandsBuilder(cmdConfigurations);

            cmdBuilder.BuildCommands();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidTypeNameFormatException))]
        public void itShouldRaiseExceptionWhenTypeNameConfigurationFormatIsIncorrect() {
            var incorrectTypeActivationNameFormat = "CommandCenter.Infrastructure.Tests.MockCommands.MockSucceedingCommand";
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = incorrectTypeActivationNameFormat
            };
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var cmdBuilder = new CommandsBuilder(cmdConfigurations);

            cmdBuilder.BuildCommands();
        }

        [TestMethod]
        [ExpectedException(typeof(TypeLoadFailedException))]
        public void itShouldRaiseExceptionWhenTypeLoadingFails() {
            var nonExistingTypeName = "CommandCenter.Tests, NoSuchType";
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = nonExistingTypeName
            };
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var cmdBuilder = new CommandsBuilder(cmdConfigurations);

            cmdBuilder.BuildCommands();
        }

        [TestMethod]
        [ExpectedException(typeof(AssemblyLoadFailedException))]
        public void itShouldRaiseExceptionWhenAssemblyLoadingFails() {
            var nonExistingAssemblyName = "NoSuchAssembly, CommandCenter.Infrastructure.Tests.MockCommands.MockSucceedingCommand";
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = nonExistingAssemblyName
            };
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var cmdBuilder = new CommandsBuilder(cmdConfigurations);

            cmdBuilder.BuildCommands();
        }

        [TestMethod]
        public void itShouldExpandCtorArgsWithConfiguredTokens() {
            var mockCommandConsumingToken = "CommandCenter.Tests, CommandCenter.Tests.MockCommands.MockCommandConsumingToken";
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = mockCommandConsumingToken
            };
            cmdConfiguration.ConstructorArgs.Add("ctorArg1", @"[BACKUPDIR]\FakeFile.txt");
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var tokens = new List<Token>();
            tokens.Add(new Token { 
                 Key = "[BACKUPDIR]",
                 Value = @"C:\SomeFakeDirectory"
            });

            var cmdBuilder = new CommandsBuilder(cmdConfigurations, tokens);
            var builtCommands = cmdBuilder.BuildCommands();

            var builtCommand = builtCommands.First() as MockCommandConsumingToken;
            Assert.IsNotNull(builtCommand);
            Assert.AreEqual(builtCommand.TokenizedArg, @"C:\SomeFakeDirectory\FakeFile.txt");
        }

         [TestMethod]
        public void itShouldExpandCtorArgsWithMultipleConfiguredTokens() {
            var mockCommandConsumingToken = "CommandCenter.Tests, CommandCenter.Tests.MockCommands.MockCommandConsumingToken";
            var cmdConfiguration = new CommandConfiguration {
                TypeActivationName = mockCommandConsumingToken
            };
            cmdConfiguration.ConstructorArgs.Add("ctorArg1", @"[BACKUPDIR]\FakeFile.[EXT]");
            var cmdConfigurations = new List<CommandConfiguration>();
            cmdConfigurations.Add(cmdConfiguration);
            var tokens = new List<Token>();
            tokens.Add(new Token { 
                 Key = "[BACKUPDIR]",
                 Value = @"C:\SomeFakeDirectory"
            });
            tokens.Add(new Token { 
                 Key = "[EXT]",
                 Value = "jpg"
            });

            var cmdBuilder = new CommandsBuilder(cmdConfigurations, tokens);
            var builtCommands = cmdBuilder.BuildCommands();

            var builtCommand = builtCommands.First() as MockCommandConsumingToken;
            Assert.IsNotNull(builtCommand);
            Assert.AreEqual(builtCommand.TokenizedArg, @"C:\SomeFakeDirectory\FakeFile.jpg");
        }
    }
}
