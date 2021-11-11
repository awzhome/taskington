using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Taskington.Base.Log;
using Taskington.Base.Service;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Extension
{
    internal class ExtensionContainer
    {
        private readonly ILog log;
        private readonly ApplicationServices applicationServices;
        private readonly EventBus eventBus;
        private readonly List<ITaskingtonExtension> extensions = new();

        public ExtensionContainer(ILog log, ApplicationServices applicationServices, EventBus eventBus)
        {
            this.log = log;
            this.applicationServices = applicationServices;
            this.eventBus = eventBus;
        }

        public void LoadExtensionFrom(Assembly assembly)
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
                        if (typeof(ITaskingtonExtension).IsAssignableFrom(extensionType)
                            && extensionType.GetConstructors().Any(ctor => !ctor.GetParameters().Any()))
                        {
                            if (Activator.CreateInstance(extensionType) is ITaskingtonExtension extensionInstance)
                            {
                                extensions.Add(extensionInstance);
                                extensionInstance.Initialize(eventBus);
                                log.Info(this, "Loaded extension from {Assembly}", assembly.FullName ?? "");
                            }
                        }
                        else
                        {
                            log.Warning(this, "No valid extension implementation found in assembly {Assembly}", assembly.GetName());
                        }

                        var bindMethod = extensionType?.GetMethod("Bind", new[] { typeof(IAppServiceBinder) });
                        if ((bindMethod != null) && bindMethod.IsStatic)
                        {
                            bindMethod.Invoke(null, new[] { applicationServices });
                            log.Info(this, "Bound services from from {Assembly}", assembly.FullName ?? "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Error loading extension from from '{assembly.GetName()}'", ex);
                }
            }
        }
    }
}
