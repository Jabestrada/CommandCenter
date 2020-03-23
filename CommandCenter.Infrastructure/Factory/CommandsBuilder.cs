using CommandCenter.Infrastructure.Configuration;
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
                var typeName = expandTokens(cmdConfig.TypeActivationName);
                var command = TypeFactory.CreateInstance<BaseCommand>(new FullTypeNameEntry(typeName), args.ToArray<object>());
                command.Enabled = cmdConfig.Enabled;
                command.ShortDescription = cmdConfig.ShortDescription;
                commands.Add(command);
            }
            return commands;
        }

        public List<CommandConfiguration> GetTokenizedConfiguration() {
            return CommandConfigurations.Select(c => new CommandConfiguration { 
                                                         TypeActivationName = expandTokens(c.TypeActivationName),
                                                         Enabled = c.Enabled,
                                                         ShortDescription = expandTokens(c.ShortDescription),
                                                         ConstructorArgs = c.ConstructorArgs
                                                                            .ToDictionary(p => p.Key,
                                                                                          p => expandTokens(p.Value))
                                                    }).ToList();
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
