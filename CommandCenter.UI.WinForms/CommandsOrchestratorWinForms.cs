using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CommandCenter.UI.WinForms {
    public class CommandsOrchestratorWinForms {
        private readonly Action<BaseCommand, CommandReportArgs> _reportReceiver;
        public CommandsOrchestratorWinForms(Action<BaseCommand, CommandReportArgs> reportReceiver) {
            _reportReceiver = reportReceiver;
        }

        public bool DidCommandsSucceed { get; protected set; }

        public bool Run(List<CommandConfiguration> commandsConfiguration) {
            var commandsBuilder = new CommandsBuilder(commandsConfiguration);
            var commands = commandsBuilder.BuildCommands();

            var commandsRunner = new CommandsRunner(commands);
            commandsRunner.OnReportSent += CommandsRunner_OnReportSent;
            DidCommandsSucceed = commandsRunner.Run();
            return DidCommandsSucceed;
        }

        public List<CommandConfiguration> GetCommands(string configFile) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);
            var commandsConfigurationSource = new CommandsConfigurationXmlSource(xmlDoc);
            return commandsConfigurationSource.GetCommandConfigurations().Where(c => c.Enabled).ToList();
        }

        private void CommandsRunner_OnReportSent(BaseCommand command, CommandReportArgs args) {
            _reportReceiver(command, args);
        }
    }
}
