using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.Views
{
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
}
