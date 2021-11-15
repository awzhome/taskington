using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Events;
using Taskington.Base.Plans;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Config
{
    public class ConfigurationManager
    {
        private static readonly object configurationLock = new();

        private bool isInitialized = false;
        private bool reloadDelayed = false;
        private readonly HashSet<Plan> runningPlans = new();

        private readonly IEventBus eventBus;
        private readonly YamlConfigurationReader configurationReader;
        private readonly YamlConfigurationWriter configurationWriter;

        public ConfigurationManager(
            IEventBus eventBus,
            YamlConfigurationReader configurationReader,
            YamlConfigurationWriter configurationWriter)
        {
            this.eventBus = eventBus;
            this.configurationReader = configurationReader;
            this.configurationWriter = configurationWriter;

            eventBus
                .Subscribe<InitializeConfiguration>(e => Initialize())
                .Subscribe<ConfigurationChanged>(OnConfigurationChanged)
                .Subscribe<PlanIsRunningUpdated>(OnPlanIsRunningUpdated)
                .Subscribe<GetConfigValue, string?>(e => GetValue(e.Key))
                .Subscribe<SetConfigValue>(e => SetValue(e.Key, e.Value))
                .Subscribe<SaveConfiguration>(e => SaveConfiguration())
                .Subscribe<InsertPlan>(e => InsertPlan(e.Index, e.NewPlan))
                .Subscribe<RemovePlan>(e => RemovePlan(e.Plan))
                .Subscribe<ReplacePlan>(e => ReplacePlan(e.OldPlan, e.NewPlan));
        }

        private readonly List<Plan> plans = new();
        public IEnumerable<Plan> Plans => plans;

        private readonly Dictionary<string, string?> configValues = new();

        public void Initialize()
        {
            if (!isInitialized)
            {
                lock (configurationLock)
                {
                    ReadConfiguration();
                }
                isInitialized = true;
            }
        }

        private void OnConfigurationChanged(ConfigurationChanged e)
        {
            bool configReloaded = false;
            lock (configurationLock)
            {
                configReloaded = TryReloadConfiguration();
            }

            if (configReloaded)
            {
                eventBus.Push(new ConfigurationReloaded());
            }
        }

        private bool TryReloadConfiguration()
        {
            if (runningPlans.Count == 0)
            {
                plans.Clear();
                ReadConfiguration();
                return true;
            }
            else
            {
                if (!reloadDelayed)
                {
                    eventBus.Push(new ConfigurationReloadDelayed(true));
                }
                reloadDelayed = true;
                return false;
            }
        }

        private void ReadConfiguration()
        {
            var configuration = configurationReader.Read();
            foreach (var (key, value) in configuration.ConfigValues)
            {
                configValues.Add(key, value);
            }
            plans.AddRange(configuration.Plans);
        }

        private void OnPlanIsRunningUpdated(PlanIsRunningUpdated e)
        {
            if (e.IsRunning)
            {
                lock (configurationLock)
                {
                    runningPlans.Add(e.Plan);
                }
            }
            else
            {
                bool configReloaded = false;
                lock (configurationLock)
                {
                    runningPlans.Remove(e.Plan);
                    if (runningPlans.Count == 0 && reloadDelayed)
                    {
                        eventBus.Push(new ConfigurationReloadDelayed(false));
                        reloadDelayed = false;
                        configReloaded = TryReloadConfiguration();
                    }
                }
                if (configReloaded)
                {
                    eventBus.Push(new ConfigurationReloaded());
                }
            }
        }

        public void SaveConfiguration()
        {
            configurationWriter.Write(new Configuration(
                configValues.Select(entry => (entry.Key, entry.Value)),
                plans));
        }

        public void InsertPlan(int index, Plan newPlan)
        {
            if (index > -1)
            {
                plans.Insert(index, newPlan);
            }
            else
            {
                plans.Add(newPlan);
            }
        }

        public void RemovePlan(Plan plan)
        {
            int index = plans.IndexOf(plan);
            if (index > -1)
            {
                plans.RemoveAt(index);
            }
        }

        public void ReplacePlan(Plan oldPlan, Plan newPlan)
        {
            int index = plans.IndexOf(oldPlan);
            if (index > -1)
            {
                plans.RemoveAt(index);
                plans.Insert(index, newPlan);
            }
            else
            {
                plans.Add(newPlan);
            }
        }

        public void SetValue(string key, string value)
        {
            configValues[key] = value;
        }

        public string? GetValue(string key)
        {
            if (configValues.TryGetValue(key, out string? configValue))
            {
                return configValue;
            }

            return null;
        }
    }
}
