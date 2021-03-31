using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PPBackup.Gui.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace PPBackup.Gui.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(d => d(ViewModel.ShowPlanEditDialog.RegisterHandler(ShowPlanEditDialogAsync)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task ShowPlanEditDialogAsync(InteractionContext<EditBackupPlanViewModel, bool> interaction)
        {
            var dialog = new EditBackupPlanWindow
            {
                DataContext = interaction.Input
            };

            var result = await dialog.ShowDialog<bool>(this);
            interaction.SetOutput(result);
        }
    }
}
