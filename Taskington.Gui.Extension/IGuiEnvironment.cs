
using Taskington.Base.Extension;

namespace Taskington.Gui.Extension;

public interface IGuiEnvironment
{
    IKeyedRegistry<IStepUi> StepUIs { get; }
    IAppNotifications AppNotifications { get; }
}