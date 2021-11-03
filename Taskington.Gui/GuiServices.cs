using Taskington.Base.Service;
using Taskington.Gui.Extension;
using Taskington.Gui.UIProviders;
using Taskington.Gui.ViewModels;

[assembly: TaskingtonExtension(typeof(Taskington.Gui.GuiServices))]

namespace Taskington.Gui
{
    static class GuiServices
    {
        public static void Bind(IAppServiceBinder binder)
        {
            binder
                .Bind<MainWindowViewModel>()
                .Bind<ModelEventDispatcher>()
                .Bind<IStepTypeUI, SyncStepUI>();
        }
    }
}
