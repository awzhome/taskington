using Taskington.Base.Extension;
using Taskington.Gui.Extension;
using Taskington.Gui.UIProviders;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui;

internal interface IFullGuiEnvironment : IGuiEnvironment
{
    IAppNotificationViewModel AppNotificationViewModel { get; }
}

internal class GuiEnvironment : IFullGuiEnvironment
{
    public GuiEnvironment()
    {
        StepUIs = new KeyedRegistry<IStepUI>();
        AppNotificationViewModel = new AppNotificationViewModel();
        AppNotifications = AppNotificationViewModel;

        SyncStepUI = new SyncStepUI(StepUIs);
    }

    public IKeyedRegistry<IStepUI> StepUIs { get; }

    public IAppNotifications AppNotifications { get; }
    public IAppNotificationViewModel AppNotificationViewModel { get; }

    internal SyncStepUI SyncStepUI { get; }
}