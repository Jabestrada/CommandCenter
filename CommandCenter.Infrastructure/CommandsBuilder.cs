using System.Collections.Generic;
using System.Linq;

namespace CommandCenter.Infrastructure {
    public class CommandsBuilder {
        public readonly List<CommandConfiguration> CommandConfigurations;
        public CommandsBuilder(List<CommandConfiguration> commandConfigurations) {
            CommandConfigurations = commandConfigurations;
        }
        public List<BaseCommand> BuildCommands() {
            var commands = new List<BaseCommand>();
            foreach (var cmdConfig in CommandConfigurations) {
                var args = cmdConfig.ConstructorArgs.Select(a => a.Value);
                var command = TypeFactory.CreateInstance<BaseCommand>(new FullTypeNameEntry(cmdConfig.TypeActivationName), args.ToArray<object>());
                commands.Add(command);
            }
            return commands;
        }
    }

    public class CommandConfiguration {
        public readonly Dictionary<string, string> ConstructorArgs;
        public CommandConfiguration() {
            ConstructorArgs = new Dictionary<string, string>();
        }
        public string TypeActivationName { get; set; }

    }
}
