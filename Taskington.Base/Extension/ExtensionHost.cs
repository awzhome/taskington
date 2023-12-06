using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Taskington.Base.Log;

namespace Taskington.Base.Extension;

public class ExtensionHost<T>
{
    private readonly T baseEnvironment;
    private readonly ILog log;

    private readonly List<object> environments = new();


    public ExtensionHost(T baseEnvironment, ILog log)
    {
        this.baseEnvironment = baseEnvironment;
        this.log = log;
    }

    public object? LoadExtensionFrom(Assembly assembly)
    {
        log.Info(this, "Load extension from {Assembly}", assembly.FullName ?? "");

        foreach (var attribute in assembly.CustomAttributes.Where(
            a => a.AttributeType == typeof(TaskingtonExtensionAttribute) && a.ConstructorArguments.Any()))
        {
            try
            {
                var extensionType = attribute.ConstructorArguments.First().Value as Type;
                if (extensionType != null)
                {
                    if (typeof(ITaskingtonExtension<T>).IsAssignableFrom(extensionType)
                        && extensionType.GetConstructors().Any(ctor => !ctor.GetParameters().Any()))
                    {
                        if (Activator.CreateInstance(extensionType) is ITaskingtonExtension<T> extensionInstance)
                        {
                            var environment = extensionInstance.InitializeEnvironment(baseEnvironment);
                            if (environment is not null)
                            {
                                environments.Add(environment);
                            }
                            log.Info(this, "Loaded extension from {Assembly} (environment: {Environment})", assembly.FullName ?? "", typeof(T).Name);
                            return environment;
                        }
                    }
                    else
                    {
                        log.Warning(this, "No valid extension implementation found in assembly {Assembly}", assembly.GetName());
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error loading extension from from '{assembly.GetName()}'", ex);
            }
        }

        return null;
    }
}