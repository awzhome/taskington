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

        public void CardMenuClicked(object sender, RoutedEventArgs args)
        {
            this.Get<ContextMenu>("CardContextMenu").Open(sender as Control);
        }
    }
}
