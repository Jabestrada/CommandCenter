using System.Collections.Generic;

namespace CommandCenter.Infrastructure.Configuration {
    public interface IConfigurationSource {
        List<CommandConfiguration> GetCommandConfigurations();
    }
}
