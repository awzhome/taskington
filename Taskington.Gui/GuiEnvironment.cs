using Taskington.Base.Extension;
using Taskington.Gui.Extension;
using Taskington.Gui.UIProviders;

namespace Taskington.Gui;

public interface IGuiEnvironment
{
    IKeyedRegistry<IStepUI> StepUIs { get; }
}

internal class GuiEnvironment : IGuiEnvironment
{
    public GuiEnvironment()
    {
        StepUIs = new KeyedRegistry<IStepUI>();
        var syncStepUI = new SyncStepUI(StepUIs);
    }

    public IKeyedRegistry<IStepUI> StepUIs { get; }
}