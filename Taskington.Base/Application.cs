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
        private readonly ILog log;
        private readonly EventBus eventBus;

        public Application(params Assembly[] extensionAssemblies)
        {
            log = new FileLog();

            eventBus = new EventBus(log);

            extensionContainer = new ExtensionContainer(log, eventBus);
            extensionContainer.LoadExtensionFrom(GetType().Assembly);

            foreach (var extensionAssembly in extensionAssemblies)
            {
                extensionContainer.LoadExtensionFrom(extensionAssembly);
            }
        }
    }
}
