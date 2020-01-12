﻿using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Configuration;
using System;
using System.Xml;

namespace CommandCenter.UI.CmdLine {
    public class CommandsOrchestrator {
        private readonly string _commandsConfigFile;
        private readonly  Action<BaseCommand, CommandReportArgs> _reportReceiver;
        public CommandsOrchestrator(string commandsConfigFile, Action<BaseCommand, CommandReportArgs> reportReceiver) {
            _commandsConfigFile = commandsConfigFile;
            _reportReceiver = reportReceiver;
        }

        public bool Run() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_commandsConfigFile);
            var commandsConfigurationSource = new CommandsConfigurationXmlSource(xmlDoc);
            var commandsConfiguration = commandsConfigurationSource.GetCommandConfigurations();

            var commandsBuilder = new CommandsBuilder(commandsConfiguration);
            var commands = commandsBuilder.BuildCommands();

            var commandsRunner = new CommandsRunner(commands);
            commandsRunner.OnReportSent += CommandsRunner_OnReportSent;
            return commandsRunner.Run();
        }

        private void CommandsRunner_OnReportSent(BaseCommand command, CommandReportArgs args) {
            _reportReceiver(command, args);
        }
    }
}
