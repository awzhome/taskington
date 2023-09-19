using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Taskington.Base.Extension;

internal class ExtensionHost
{
    private readonly IBaseEnvironment baseEnvironment;
    private readonly List<object> environments = new();


    public ExtensionHost(IBaseEnvironment baseEnvironment)
    {
        this.baseEnvironment = baseEnvironment;
    }

    public object? LoadExtensionFrom(Assembly assembly)
    {
        var log = baseEnvironment.Log;
        log.Info(this, "Load extension from {Assembly}", assembly.FullName ?? "");

        foreach (var attribute in assembly.CustomAttributes.Where(
            a => a.AttributeType == typeof(TaskingtonExtensionAttribute) && a.ConstructorArguments.Any()))
        {
            try
            {
                var extensionType = attribute.ConstructorArguments.First().Value as Type;
                if (extensionType != null)
                {
                    if (typeof(ITaskingtonExtension).IsAssignableFrom(extensionType)
                        && extensionType.GetConstructors().Any(ctor => !ctor.GetParameters().Any()))
                    {
                        if (Activator.CreateInstance(extensionType) is ITaskingtonExtension extensionInstance)
                        {
                            var environment = extensionInstance.InitializeEnvironment(baseEnvironment);
                            if (environment is not null)
                            {
                                environments.Add(environment);
                            }
                            log.Info(this, "Loaded extension from {Assembly}", assembly.FullName ?? "");
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