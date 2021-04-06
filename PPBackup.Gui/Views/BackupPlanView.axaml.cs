using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PPBackup.Gui.Views
{
    public class BackupPlanView : UserControl
    {
        public BackupPlanView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OpenButtonMenu(object sender, RoutedEventArgs args)
        {
            var senderControl = sender as Control;
            senderControl?.ContextMenu?.Open(senderControl);
        }
    }
}
