using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.Views;

class EditSyncStepView : ReactiveUserControl<EditSyncStepViewModel>
{
    public EditSyncStepView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}