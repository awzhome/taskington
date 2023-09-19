using Taskington.Base.Config;
using Taskington.Base.Extension;
using Taskington.Base.Log;
using Taskington.Base.Plans;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;

namespace Taskington.Base;

public interface IBaseEnvironment
{
    ILog Log { get; }
    ILogConfiguration LogConfiguration { get; }
    IConfigurationManager ConfigurationManager { get; }
    IPlanExecution PlanExecution { get; }
    IKeyedRegistry<IStepExecution> StepExecutions { get; }
    ISystemOperations SystemOperations { get; }
}

internal class BaseEnvironment : IBaseEnvironment
{
    public BaseEnvironment()
    {
        Log = new FileLog();
        var configurationProvider = new YamlFileConfigurationProvider();
        var configurationReader = new YamlConfigurationReader(configurationProvider);
        var configurationWriter = new YamlConfigurationWriter(configurationProvider);
        SystemOperations = OsSpecificSystemOperations.Create(Log);
        StepExecutions = new KeyedRegistry<IStepExecution>();
        PlanExecution = new PlanExecution(SystemOperations, StepExecutions);
        ConfigurationManager = new ConfigurationManager(configurationReader, configurationWriter, PlanExecution);
        LogConfiguration = new LogConfiguration(Log, ConfigurationManager);
    }

    public ILog Log { get; }
    public ILogConfiguration LogConfiguration { get; }
    public IConfigurationManager ConfigurationManager { get; }
    public IPlanExecution PlanExecution { get; }
    public IKeyedRegistry<IStepExecution> StepExecutions { get; }
    public ISystemOperations SystemOperations { get; }
}