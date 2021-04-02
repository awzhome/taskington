using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PPBackup.Gui.Views
{
    public class EditSyncStepView : UserControl
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
