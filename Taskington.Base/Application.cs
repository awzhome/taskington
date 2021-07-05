using System;
using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Config;
using Taskington.Base.Service;

namespace Taskington.Base
{
    public class Application
    {
        private readonly ApplicationServices services;

        public Application(Action<IAppServiceBinder>? binderFunction = null)
        {
            services = new ApplicationServices(binder =>
            {
                BaseServices.Bind(binder);
                binder.Bind(this);
                binderFunction?.Invoke(binder);
            });
        }

        public IAppServiceProvider ServiceProvider => services;

        public void Start()
        {
            services.Start();
        }

        public void NotifyInitialStates()
        {
            foreach (var execution in services.Get<ConfigurationManager>().ExecutablePlans.Select(executablePlan => executablePlan.Execution))
            {
                execution.NotifyInitialStates();
            }
        }
    }
}
