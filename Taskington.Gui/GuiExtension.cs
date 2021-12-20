using Taskington.Base.Extension;
using Taskington.Gui.UIProviders;

[assembly: TaskingtonExtension(typeof(Taskington.Gui.GuiExtension))]

namespace Taskington.Gui
{
    class GuiExtension : ITaskingtonExtension
    {
        //public static void Bind(IAppServiceBinder binder)
        //{
        //    binder
        //        .Bind<MainWindowViewModel>()
        //        .Bind<ModelEventDispatcher>()
        //        .Bind<IStepTypeUI, SyncStepUI>();
        //}

        public void Initialize(IHandlerStore handlerStore)
        {
            handlerStore.Add(new SyncStepUI());
        }
    }
}
