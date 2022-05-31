using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Taskington.Gui.Views
{
    class PlanView : UserControl
    {
        public PlanView()
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
