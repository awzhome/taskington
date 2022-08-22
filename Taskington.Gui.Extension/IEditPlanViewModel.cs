using ReactiveUI;
using System.Reactive;

namespace Taskington.Gui.Extension
{
    public interface IEditPlanViewModel
    {
        Interaction<string?, string?> OpenFolderDialog { get; }
        Interaction<string?, string?> OpenFileDialog { get; }
    }
}
