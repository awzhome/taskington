using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Taskington.Gui.Views
{
    public class EditGeneralStepView : UserControl
    {
        public EditGeneralStepView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
