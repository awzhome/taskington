using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PPBackup.Gui.ViewModels;

namespace PPBackup.Gui.Views
{
    public class EditSyncStepView : ReactiveUserControl<EditSyncStepViewModel>
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
