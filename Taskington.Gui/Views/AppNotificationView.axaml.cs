using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Taskington.Gui.Views
{
    public partial class AppNotificationView : UserControl
    {
        public AppNotificationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
