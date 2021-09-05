using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        internal ApplicationServices()
        {
            services = new ServiceCollection();
            Bind<IAppServiceProvider>(this);
        }

        public void BindServicesFrom(Assembly assembly)
        {
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
                }
                catch (Exception)
                {
                    // TODO Log this
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
