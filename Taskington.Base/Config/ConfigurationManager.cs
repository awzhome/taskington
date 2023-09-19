using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Taskington.Base.Plans;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Config;

public interface IConfigurationManager
{
    event EventHandler? ConfigurationReloaded;
    event EventHandler? ConfigurationReloadDelayed;

    void Initialize();
    void SetValue(string key, string? value);
    string? GetValue(string key);
    void SaveConfiguration();
    void ReloadConfiguration();

    void InsertPlan(int index, Plan plan);
    void RemovePlan(Plan plan);
    void ReplacePlan(Plan oldPlan, Plan newPlan);

    IEnumerable<Plan> Plans { get; }
}

public class ConfigurationManager : IConfigurationManager
{
    private static readonly object configurationLock = new();

    private bool isInitialized = false;
    private bool reloadDelayed = false;
    private readonly HashSet<Plan> runningPlans = new();
    private readonly YamlConfigurationReader configurationReader;
    private readonly YamlConfigurationWriter configurationWriter;

    public event EventHandler? ConfigurationReloaded;
    public event EventHandler? ConfigurationReloadDelayed;

    internal ConfigurationManager(
        YamlConfigurationReader configurationReader,
        YamlConfigurationWriter configurationWriter,
        IPlanExecution planExecution)
    {
        this.configurationReader = configurationReader;
        this.configurationWriter = configurationWriter;
        planExecution.PlanRunningUpdated += OnPlanIsRunningUpdated;
    }

    private readonly List<Plan> plans = new();

    private readonly Dictionary<string, string?> configValues = new();

    public IEnumerable<Plan> Plans => plans;

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

    public void ReloadConfiguration()
    {
        bool configReloaded = false;
        lock (configurationLock)
        {
            configReloaded = TryReloadConfiguration();
        }

        if (configReloaded)
        {
            ConfigurationReloadDelayed?.Invoke(this, new());
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
                ConfigurationReloadDelayed?.Invoke(this, new());
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

    private void OnPlanIsRunningUpdated(object? sender, PlanRunningUpdatedEventArgs e)
    {
        if (e.Running)
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
                    ConfigurationReloadDelayed?.Invoke(this, new());
                    reloadDelayed = false;
                    configReloaded = TryReloadConfiguration();
                }
            }
            if (configReloaded)
            {
                ConfigurationReloaded?.Invoke(this, new());
            }
        }
    }

    public void SaveConfiguration()
    {
        configurationWriter.Write(new Configuration(
            configValues.Select(entry => (entry.Key, entry.Value)),
            plans));
    }

    public void InsertPlan(int index, Plan plan)
    {
        if (index > -1)
        {
            plans.Insert(index, plan);
        }
        else
        {
            plans.Add(plan);
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

    public void SetValue(string key, string? value)
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
