using System.Collections;
using System.Collections.Generic;
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

    public void Load(params Assembly[] extensionAssemblies)
    {
        foreach (var extensionAssembly in extensionAssemblies)
        {
            extensionHost.LoadExtensionFrom(extensionAssembly);
        }
    }
}
