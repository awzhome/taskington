using ReactiveUI;
using System.Reactive;

namespace Taskington.Gui.Extension
{
    public interface IEditPlanViewModel
    {
        Interaction<Unit, string?> OpenFolderDialog { get; }
        Interaction<Unit, string?> OpenFileDialog { get; }
    }
}
