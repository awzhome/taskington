using System.Collections.Generic;
using Taskington.Base.Plans;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Config
{
    public static class ConfigurationMessages
    {
        public static Message InitializeConfiguration { get; } = new();
        public static Message ConfigurationChanged { get; } = new();
        public static Message ConfigurationReloaded { get; } = new();
        public static Message<bool> ConfigurationReloadDelayed { get; } = new();

        public static RequestMessage<string, string?> GetConfigValue { get; } = new();
        public static Message<string, string> SetConfigValue { get; } = new();
        public static Message SaveConfiguration { get; } = new();
        public static Message<int, Plan> InsertPlan { get; } = new();
        public static Message<Plan> RemovePlan { get; } = new();
        public static Message<Plan, Plan> ReplacePlan { get; } = new();
        public static RequestMessage<IEnumerable<Plan>> GetPlans { get; } = new();
    }
}
