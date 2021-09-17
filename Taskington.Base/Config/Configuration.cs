using System.Collections.Generic;
using Taskington.Base.Plans;

namespace Taskington.Base.Config
{
    public class Configuration
    {
        public IEnumerable<(string Key, string? Value)> ConfigValues { get; }
        public IEnumerable<Plan> Plans { get; }

        public Configuration(IEnumerable<(string, string?)> configValues, IEnumerable<Plan> plans)
        {
            ConfigValues = configValues;
            Plans = plans;
        }
    }
}
