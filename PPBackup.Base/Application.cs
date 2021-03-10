using PPBackup.Base.Plans;
using PPBackup.Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base
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
            foreach (var execution in services.Get<List<ExecutableBackupPlan>>().Select(executablePlan => executablePlan.Execution))
            {
                execution.NotifyInitialStates();
            }
        }
    }
}
