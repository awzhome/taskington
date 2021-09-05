using System.Linq;
using System.Reflection;
using Taskington.Base.Config;
using Taskington.Base.Service;

namespace Taskington.Base
{
    public class Application
    {
        private readonly ApplicationServices services;

        public Application(params Assembly[] extensionAssemblies)
        {
            services = new ApplicationServices();
            services.BindServicesFrom(GetType().Assembly);
            services.Bind(this);
            foreach (var extensionAssembly in extensionAssemblies)
            {
                services.BindServicesFrom(extensionAssembly);
            }
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
