using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Avalonia.ReactiveUI;
using PPBackup.Gui.ViewModels;
using System;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using System.Reactive;

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
            this.WhenActivated(d => d(ViewModel.OpenFolderDialog.RegisterHandler(OpenFolderDialogAsync)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OpenButtonMenu(object sender, RoutedEventArgs args)
        {
            var senderControl = sender as Control;
            senderControl?.ContextMenu?.Open(senderControl);
        }

        private async Task OpenFolderDialogAsync(InteractionContext<Unit, string> interaction)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select directory",
            };

            var result = await dialog.ShowAsync(this);
            interaction.SetOutput(result);
        }
    }
}
