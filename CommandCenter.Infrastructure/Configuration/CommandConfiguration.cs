using System.Collections.Generic;

namespace CommandCenter.Infrastructure.Configuration {
    public class CommandConfiguration {
        public readonly Dictionary<string, string> ConstructorArgs;
        public CommandConfiguration() {
            ConstructorArgs = new Dictionary<string, string>();
        }
        public string TypeActivationName { get; set; }

        public string ShortDescription { get; set; }
        public bool Enabled { get; set; }
    }
}
