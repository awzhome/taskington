using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using Taskington.Base.Extension;

namespace Taskington.Base;

public class Application
{
    private readonly ExtensionHost<IBaseEnvironment> extensionHost;

    public Application()
    {
        BaseEnvironment = new BaseEnvironment();
        extensionHost = new ExtensionHost<IBaseEnvironment>(BaseEnvironment, BaseEnvironment.Log);
    }

    public IBaseEnvironment BaseEnvironment { get; }

    public IEnumerable<object> Load(params Assembly[] extensionAssemblies)
    {
        foreach (var extensionAssembly in extensionAssemblies)
        {
            var enviroment = extensionHost.LoadExtensionFrom(extensionAssembly);
            if (enviroment is not null)
            {
                yield return enviroment;
            }
        }
    }
}
