using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base
{
    public class ApplicationServices
    {
        private readonly IServiceCollection services;
        private ServiceProvider? serviceProvider;

        internal ApplicationServices()
        {
            services = new ServiceCollection();
            With(this);
        }

        public ApplicationServices With<T>() where T : class
        {
            services.AddSingleton<T>();
            return this;
        }

        public ApplicationServices With<T>(T instance) where T : class
        {
            services.AddSingleton(instance);
            return this;
        }

        public ApplicationServices With<T>(Func<T> serviceFactory)
            where T : class
        {
            services.AddSingleton(provider => serviceFactory());
            return this;
        }

        public ApplicationServices With<T, S>()
            where T : class
            where S : class, T
        {
            services.AddSingleton<T, S>();
            return this;
        }

        public void Start()
        {
            serviceProvider = services.BuildServiceProvider();
        }

        public T Get<T>() => serviceProvider.GetRequiredService<T>();

        public T? Get<T>(Func<T, bool> predicate) => serviceProvider.GetServices<T>().FirstOrDefault(predicate);

        public IEnumerable<T> GetAll<T>() => serviceProvider.GetServices<T>();
    }
}
