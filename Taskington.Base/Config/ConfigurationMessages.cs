using System.Collections.Generic;
using Taskington.Base.Plans;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.Config
{
    public static class ConfigurationMessages
    {
        public static MessageEndPoint InitializeConfiguration { get; } = new();
        public static MessageEndPoint ConfigurationChanged { get; } = new();
        public static MessageEndPoint ConfigurationReloaded { get; } = new();
        public static MessageEndPoint<bool> ConfigurationReloadDelayed { get; } = new();

        public static RequestMessageEndPoint<string, string?> GetConfigValue { get; } = new();
        public static MessageEndPoint<string, string> SetConfigValue { get; } = new();
        public static MessageEndPoint SaveConfiguration { get; } = new();
        public static MessageEndPoint<int, Plan> InsertPlan { get; } = new();
        public static MessageEndPoint<Plan> RemovePlan { get; } = new();
        public static MessageEndPoint<Plan, Plan> ReplacePlan { get; } = new();
        public static RequestMessageEndPoint<IEnumerable<Plan>> GetPlans { get; } = new();
    }
}
