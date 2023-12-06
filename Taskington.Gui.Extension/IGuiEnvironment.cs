
using Taskington.Base.Extension;

namespace Taskington.Gui.Extension;

public interface IGuiEnvironment
{
    IKeyedRegistry<IStepUI> StepUIs { get; }
    IAppNotifications AppNotifications { get; }
}