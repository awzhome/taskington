using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Taskington.Base.Extension;

namespace Taskington.Base;

public class Application
{
    private readonly ExtensionHost extensionHost;

    public Application()
    {
        BaseEnvironment = new BaseEnvironment();
        extensionHost = new ExtensionHost(BaseEnvironment);
    }

    public IBaseEnvironment BaseEnvironment { get; }

    public IEnumerable<object> Load(params Assembly[] extensionAssemblies)
    {
        var environments = new List<object>();

        foreach (var extensionAssembly in extensionAssemblies)
        {
            var environment = extensionHost.LoadExtensionFrom(extensionAssembly);
            if (environment is not null)
            {
                environments.Add(environment);
            }
        }

        return environments;
    }
}
