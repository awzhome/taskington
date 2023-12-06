using Taskington.Base;
using Taskington.Base.Extension;
using Taskington.Gui.UIProviders;

[assembly: TaskingtonExtension(typeof(Taskington.Gui.GuiExtension))]

namespace Taskington.Gui;

class GuiExtension : ITaskingtonExtension<IBaseEnvironment>
{
    public object? InitializeEnvironment(IBaseEnvironment baseEnvironment)
    {
        return new GuiEnvironment();
    }
}