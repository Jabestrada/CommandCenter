using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using CommandCenter.Infrastructure.Factory;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CommandCenter.UI.WinForms {
    public class CommandsControllerWinForms {
        public TimeSpan LastRunElapsedTime { get; protected set; }
        private CommandsRunner _commandsRunner;
        private readonly Action<BaseCommand, CommandReportArgs> _reportReceiver;
        public CommandsControllerWinForms(Action<BaseCommand, CommandReportArgs> reportReceiver) {
            _reportReceiver = reportReceiver;
        }

        public bool DidCommandsSucceed { get; protected set; }
        public List<CommandReport> Reports {
            get {
                if (_commandsRunner != null) {
                    return _commandsRunner.Reports;
                }
                return new List<CommandReport>();
            }
        }
        public bool Run(List<CommandConfiguration> commandsConfiguration) {
            var commandsBuilder = new CommandsBuilder(commandsConfiguration);
            var commands = commandsBuilder.BuildCommands();

            _commandsRunner = new CommandsRunner(commands);
            _commandsRunner.OnReportSent += CommandsRunner_OnReportSent;
            DidCommandsSucceed = _commandsRunner.Run();
            LastRunElapsedTime = _commandsRunner.LastRunElapsedTime;
            return DidCommandsSucceed;
        }

        public List<CommandConfiguration> GetCommands(string configFile) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            var commandsConfigurationSource = new CommandsConfigurationXmlSource(xmlDoc);
            var commandConfigs = commandsConfigurationSource.GetCommandConfigurations().Where(c => c.Enabled).ToList();
            var cmdBuilder = new CommandsBuilder(commandConfigs, commandsConfigurationSource.Tokens);
            return cmdBuilder.GetTokenizedConfiguration();
        }

        public List<Token> GetTokens(string configFile) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            var commandsConfigurationSource = new CommandsConfigurationXmlSource(xmlDoc);
            return commandsConfigurationSource.Tokens;
        }

        private void CommandsRunner_OnReportSent(BaseCommand command, CommandReportArgs args) {
            _reportReceiver(command, args);
        }
    }
}
