using System.Collections.Generic;
using Taskington.Base.Plans;

namespace Taskington.Base.Config;

public class Configuration(IEnumerable<(string, string?)> configValues, IEnumerable<Plan> plans)
{
    public IEnumerable<(string Key, string? Value)> ConfigValues { get; } = configValues;
    public IEnumerable<Plan> Plans { get; } = plans;
}
