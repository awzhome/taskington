using System.Collections.Generic;
using System.Linq;
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

        private readonly YamlConfigurationReader configurationReader;
        private readonly YamlConfigurationWriter configurationWriter;

        public ConfigurationManager(
            YamlConfigurationReader configurationReader,
            YamlConfigurationWriter configurationWriter)
        {
            this.configurationReader = configurationReader;
            this.configurationWriter = configurationWriter;

            GetPlansMessage.Subscribe(_ => plans);
        }

        private readonly List<Plan> plans = new();

        private readonly Dictionary<string, string?> configValues = new();

        [HandlesMessage]
        public void Initialize(InitializeConfigurationMessage _)
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

        [HandlesMessage]
        public void OnConfigurationChanged(ConfigurationChangedMessage _)
        {
            bool configReloaded = false;
            lock (configurationLock)
            {
                configReloaded = TryReloadConfiguration();
            }

            if (configReloaded)
            {
                new ConfigurationReloadedMessage().Publish();
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
                    new ConfigurationReloadDelayedMessage().Publish();
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

        [HandlesMessage]
        public void OnPlanIsRunningUpdated(PlanRunningUpdateMessage message)
        {
            if (message.IsRunning)
            {
                lock (configurationLock)
                {
                    runningPlans.Add(message.Plan);
                }
            }
            else
            {
                bool configReloaded = false;
                lock (configurationLock)
                {
                    runningPlans.Remove(message.Plan);
                    if (runningPlans.Count == 0 && reloadDelayed)
                    {
                        new ConfigurationReloadDelayedMessage().Publish();
                        reloadDelayed = false;
                        configReloaded = TryReloadConfiguration();
                    }
                }
                if (configReloaded)
                {
                    new ConfigurationReloadedMessage().Publish();
                }
            }
        }

        [HandlesMessage]
        public void SaveConfiguration(SaveConfigurationMessage _)
        {
            configurationWriter.Write(new Configuration(
                configValues.Select(entry => (entry.Key, entry.Value)),
                plans));
        }

        [HandlesMessage]
        public void InsertPlan(InsertPlanMessage message)
        {
            if (message.Index > -1)
            {
                plans.Insert(message.Index, message.Plan);
            }
            else
            {
                plans.Add(message.Plan);
            }
        }

        [HandlesMessage]
        public void RemovePlan(RemovePlanMessage message)
        {
            int index = plans.IndexOf(message.Plan);
            if (index > -1)
            {
                plans.RemoveAt(index);
            }
        }

        [HandlesMessage]
        public void ReplacePlan(ReplacePlanMessage message)
        {
            int index = plans.IndexOf(message.OldPlan);
            if (index > -1)
            {
                plans.RemoveAt(index);
                plans.Insert(index, message.NewPlan);
            }
            else
            {
                plans.Add(message.NewPlan);
            }
        }

        [HandlesMessage]
        public void SetValue(SetConfigValueMessage message)
        {
            configValues[message.Key] = message.Value;
        }

        [HandlesMessage]
        public string? GetValue(GetConfigValueMessage message)
        {
            if (configValues.TryGetValue(message.Key, out string? configValue))
            {
                return configValue;
            }

            return null;
        }
    }
}
