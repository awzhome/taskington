using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Avalonia.ReactiveUI;
using Taskington.Gui.ViewModels;
using System;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using System.Reactive;
using System.Linq;

namespace Taskington.Gui.Views
{
    class EditPlanWindow : ReactiveWindow<EditPlanViewModel>
    {
        public EditPlanWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(d => d(ViewModel!.CloseCommand.Subscribe(save => Close(save))));
            this.WhenActivated(d => d(ViewModel!.OpenFolderDialog.RegisterHandler(OpenFolderDialogAsync)));
            this.WhenActivated(d => d(ViewModel!.OpenFileDialog.RegisterHandler(OpenFileDialogAsync)));
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

        private async Task OpenFileDialogAsync(InteractionContext<Unit, string?> interaction)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select file",
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(this);
            interaction.SetOutput(result?.FirstOrDefault());
        }
    }
}
