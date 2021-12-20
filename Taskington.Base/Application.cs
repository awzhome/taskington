using System.Reflection;
using Taskington.Base.Extension;
using Taskington.Base.Log;

namespace Taskington.Base
{
    public class Application
    {
        private readonly ExtensionContainer extensionContainer;
        private readonly ILog log;

        public Application()
        {
            log = new FileLog();

            extensionContainer = new ExtensionContainer(log);
        }

        public void Load(params Assembly[] extensionAssemblies)
        {
            extensionContainer.LoadExtensionFrom(GetType().Assembly);
            foreach (var extensionAssembly in extensionAssemblies)
            {
                extensionContainer.LoadExtensionFrom(extensionAssembly);
            }
        }
    }
}
