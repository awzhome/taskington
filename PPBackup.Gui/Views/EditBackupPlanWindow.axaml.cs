using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Avalonia.ReactiveUI;
using PPBackup.Gui.ViewModels;
using System;

namespace PPBackup.Gui.Views
{
    public class EditBackupPlanWindow : ReactiveWindow<EditBackupPlanViewModel>
    {
        public EditBackupPlanWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(d => d(ViewModel.CloseCommand.Subscribe(save => Close(save))));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
