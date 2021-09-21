using System.Linq;
using System.Reflection;
using Taskington.Base.Config;
using Taskington.Base.Log;
using Taskington.Base.Service;

namespace Taskington.Base
{
    public class Application
    {
        private readonly ApplicationServices services;
        private readonly ILog log;

        public Application(params Assembly[] extensionAssemblies)
        {
            log = new FileLog();

            services = new ApplicationServices(log);
            services.BindServicesFrom(GetType().Assembly);
            services.Bind(log);
            services.Bind(this);
            foreach (var extensionAssembly in extensionAssemblies)
            {
                services.BindServicesFrom(extensionAssembly);
            }
        }

        public IAppServiceProvider ServiceProvider => services;

        public void Start()
        {
            log.Info(this, "Starting Taskington base application");
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
