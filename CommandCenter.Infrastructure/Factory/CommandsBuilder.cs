using CommandCenter.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandCenter.Infrastructure.Factory {
    public class CommandsBuilder {
        public readonly List<CommandConfiguration> CommandConfigurations;
        public readonly List<Token> Tokens;
        public CommandsBuilder(List<CommandConfiguration> commandConfigurations) {
            CommandConfigurations = commandConfigurations;
        }
        public CommandsBuilder(List<CommandConfiguration> commandConfigurations, List<Token> tokens) : this(commandConfigurations) {
            Tokens = tokens;
        }

        public List<BaseCommand> BuildCommands() {
            var commands = new List<BaseCommand>();
            foreach (var cmdConfig in CommandConfigurations) {
                var args = cmdConfig.ConstructorArgs.Select(a => expandTokens(a.Value));
                var command = TypeFactory.CreateInstance<BaseCommand>(new FullTypeNameEntry(cmdConfig.TypeActivationName), args.ToArray<object>());
                commands.Add(command);
            }
            return commands;
        }

        private string expandTokens(string input) {
            if (Tokens == null || !Tokens.Any()) return input;

            var expandedResult = input;
            foreach (var token in Tokens) {
                expandedResult = expandedResult.Replace(token.Key, token.Value);
            }
            return expandedResult;
        }
    }


}
