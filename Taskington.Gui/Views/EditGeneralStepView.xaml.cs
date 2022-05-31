using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Taskington.Gui.Views
{
    class EditGeneralStepView : UserControl
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
