using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Taskington.Base.Log;

namespace Taskington.Base.Service
{
    public interface IAppServiceProvider
    {
        T Get<T>();
        T? Get<T>(Func<T, bool> predicate);
        IEnumerable<T> GetAll<T>();
    }

    public interface IAppServiceBinder
    {
        IAppServiceBinder Bind<T>() where T : class;
        IAppServiceBinder Bind<T>(T instance) where T : class;
        IAppServiceBinder Bind<T>(Func<T> serviceFactory) where T : class;
        IAppServiceBinder Bind<T, S>()
            where T : class
            where S : class, T;
    }

    class ApplicationServices : IAppServiceProvider, IAppServiceBinder
    {
        private readonly IServiceCollection services;
        private ServiceProvider? serviceProvider;

        private readonly List<Action> autoInitializers = new();
        private readonly ILog log;

        internal ApplicationServices(ILog log)
        {
            this.log = log;

            services = new ServiceCollection();
            Bind<IAppServiceProvider>(this);
        }

        public void BindServicesFrom(Assembly assembly)
        {
            log.Info(this, "Binding services from {Assembly}", assembly.FullName ?? "");

            foreach (var attribute in assembly.CustomAttributes.Where(
                a => a.AttributeType == typeof(TaskingtonExtensionAttribute) && a.ConstructorArguments.Any()))
            {
                try
                {
                    var bindMethod = (attribute.ConstructorArguments.First().Value as Type)
                        ?.GetMethod("Bind", new[] { typeof(IAppServiceBinder) });
                    if ((bindMethod != null) && bindMethod.IsStatic)
                    {
                        bindMethod.Invoke(null, new[] { this });
                    }
                    else
                    {
                        log.Error(this, "No valid bind method found in assembly {Assembly}", assembly.GetName());
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Error binding services from '{assembly.GetName()}'", ex);
                }
            }
        }

        public IAppServiceBinder Bind<T>() where T : class
        {
            services.AddSingleton<T>();
            CheckForAutoInitialization<T>();
            return this;
        }

        public IAppServiceBinder Bind<T>(T instance) where T : class
        {
            services.AddSingleton(instance);
            return this;
        }

        public IAppServiceBinder Bind<T>(Func<T> serviceFactory)
            where T : class
        {
            services.AddSingleton(provider => serviceFactory());
            return this;
        }

        public IAppServiceBinder Bind<T, S>()
            where T : class
            where S : class, T
        {
            services.AddSingleton<T, S>();
            return this;
        }

        public void Start()
        {
            serviceProvider = services.BuildServiceProvider();
            foreach (var autoInitializer in autoInitializers)
            {
                autoInitializer();
            }
        }

        private void CheckForAutoInitialization<T>()
        {
            if (typeof(IAutoInitializable).IsAssignableFrom(typeof(T)))
            {
                autoInitializers.Add(() => (Get<T>() as IAutoInitializable)?.Initialize());
            }
        }

        public T Get<T>() => serviceProvider.GetRequiredService<T>();

        public T? Get<T>(Func<T, bool> predicate) => serviceProvider.GetServices<T>().FirstOrDefault(predicate);

        public IEnumerable<T> GetAll<T>() => serviceProvider.GetServices<T>();
    }
}
