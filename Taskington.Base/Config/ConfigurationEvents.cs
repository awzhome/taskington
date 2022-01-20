using System.Collections.Generic;
using Taskington.Base.Plans;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Config
{
    public static class ConfigurationEvents
    {
        public static Event InitializeConfiguration { get; } = new();
        public static Event ConfigurationChanged { get; } = new();
        public static Event ConfigurationReloaded { get; } = new();
        public static Event<bool> ConfigurationReloadDelayed { get; } = new();

        public static RequestEvent<string, string?> GetConfigValue { get; } = new();
        public static Event<string, string> SetConfigValue { get; } = new();
        public static Event SaveConfiguration { get; } = new();
        public static Event<int, Plan> InsertPlan { get; } = new();
        public static Event<Plan> RemovePlan { get; } = new();
        public static Event<Plan, Plan> ReplacePlan { get; } = new();
        public static RequestEvent<IEnumerable<Plan>> GetPlans { get; } = new();
    }
}
