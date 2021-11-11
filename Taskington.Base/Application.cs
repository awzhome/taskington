using System.Linq;
using System.Reflection;
using Taskington.Base.Config;
using Taskington.Base.Extension;
using Taskington.Base.Log;
using Taskington.Base.Service;
using Taskington.Base.TinyBus;

namespace Taskington.Base
{
    public class Application
    {
        private readonly ExtensionContainer extensionContainer;
        private readonly ApplicationServices services;
        private readonly ILog log;
        private readonly EventBus eventBus;

        public Application(params Assembly[] extensionAssemblies)
        {
            log = new FileLog();

            eventBus = new EventBus(log);

            services = new ApplicationServices(log);

            extensionContainer = new ExtensionContainer(log, services, eventBus);
            extensionContainer.LoadExtensionFrom(GetType().Assembly);

            services.Bind(log);
            services.Bind(this);

            foreach (var extensionAssembly in extensionAssemblies)
            {
                extensionContainer.LoadExtensionFrom(extensionAssembly);
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
