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
    IStreamReaderProvider StreamReaderProvider { get; }
    IStreamWriterProvider StreamWriterProvider { get; }
}

internal class BaseEnvironment : IBaseEnvironment
{
    public BaseEnvironment()
    {
        Log = new FileLog();
        var configurationProvider = new YamlFileConfigurationProvider();
        StreamReaderProvider = configurationProvider;
        StreamWriterProvider = configurationProvider;
        ConfigurationReader = new YamlConfigurationReader(configurationProvider);
        ConfigurationWriter = new YamlConfigurationWriter(configurationProvider);
        SystemOperations = OsSpecificSystemOperations.Create(Log);
        StepExecutions = new KeyedRegistry<IStepExecution>();
        PlanExecution = new PlanExecution(SystemOperations, StepExecutions);
        ConfigurationManager = new ConfigurationManager(ConfigurationReader, ConfigurationWriter, PlanExecution);
        LogConfiguration = new LogConfiguration(Log, ConfigurationManager);

        SyncStepExecution = new SyncStepExecution(PlanExecution, SystemOperations, StepExecutions);
    }

    public ILog Log { get; }
    public ILogConfiguration LogConfiguration { get; }
    public IConfigurationManager ConfigurationManager { get; }
    public IPlanExecution PlanExecution { get; }
    public IKeyedRegistry<IStepExecution> StepExecutions { get; }
    public ISystemOperations SystemOperations { get; }
    public IStreamReaderProvider StreamReaderProvider { get; }
    public IStreamWriterProvider StreamWriterProvider { get; }

    public SyncStepExecution SyncStepExecution { get; }
    public YamlConfigurationReader ConfigurationReader { get; }
    public YamlConfigurationWriter ConfigurationWriter { get; }
}