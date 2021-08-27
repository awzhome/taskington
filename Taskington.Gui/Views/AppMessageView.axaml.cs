using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Taskington.Gui.Views
{
    public partial class AppMessageView : UserControl
    {
        public AppMessageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
