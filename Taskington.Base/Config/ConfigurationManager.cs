using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Plans;

namespace Taskington.Base.Config
{
    public class ConfigurationManager
    {
        private static readonly object configurationLock = new();

        private bool isInitialized = false;
        private bool reloadDelayed = false;
        private readonly HashSet<Plan> runningPlans = new();

        private readonly YamlConfigurationReader configurationReader;
        private readonly YamlConfigurationWriter configurationWriter;

        public ConfigurationManager(
            YamlConfigurationReader configurationReader,
            YamlConfigurationWriter configurationWriter)
        {
            this.configurationReader = configurationReader;
            this.configurationWriter = configurationWriter;

            ConfigurationEvents.InitializeConfiguration.Subscribe(() => Initialize());
            ConfigurationEvents.ConfigurationChanged.Subscribe(OnConfigurationChanged);
            ConfigurationEvents.GetConfigValue.Subscribe(key => GetValue(key));
            ConfigurationEvents.SetConfigValue.Subscribe((key, value) => SetValue(key, value));
            ConfigurationEvents.InsertPlan.Subscribe((index, plan) => InsertPlan(index, plan));
            ConfigurationEvents.RemovePlan.Subscribe((plan) => RemovePlan(plan));
            ConfigurationEvents.ReplacePlan.Subscribe((oldPlan, newPlan) => ReplacePlan(oldPlan, newPlan));
            ConfigurationEvents.SaveConfiguration.Subscribe(SaveConfiguration);
            ConfigurationEvents.GetPlans.Subscribe(() => plans);
            PlanEvents.PlanIsRunningUpdated.Subscribe(OnPlanIsRunningUpdated);
        }

        private readonly List<Plan> plans = new();

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

        private void OnConfigurationChanged()
        {
            bool configReloaded = false;
            lock (configurationLock)
            {
                configReloaded = TryReloadConfiguration();
            }

            if (configReloaded)
            {
                ConfigurationEvents.ConfigurationReloaded.Push();
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
                    ConfigurationEvents.ConfigurationReloadDelayed.Push(true);
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

        private void OnPlanIsRunningUpdated(Plan plan, bool isRunning)
        {
            if (isRunning)
            {
                lock (configurationLock)
                {
                    runningPlans.Add(plan);
                }
            }
            else
            {
                bool configReloaded = false;
                lock (configurationLock)
                {
                    runningPlans.Remove(plan);
                    if (runningPlans.Count == 0 && reloadDelayed)
                    {
                        ConfigurationEvents.ConfigurationReloadDelayed.Push(true);
                        reloadDelayed = false;
                        configReloaded = TryReloadConfiguration();
                    }
                }
                if (configReloaded)
                {
                    ConfigurationEvents.ConfigurationReloaded.Push();
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
