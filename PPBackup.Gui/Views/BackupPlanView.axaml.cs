using Avalonia;
using Avalonia.Controls;
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
    }
}
