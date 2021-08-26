using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Globalization;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.Views
{
    class MessageTypeToColorConverter : IValueConverter
    {
        public IBrush? AppInfo { get; set; }
        public IBrush? Info { get; set; }
        public IBrush? Warning { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AppMessageType type)
            {
                return type switch
                {
                    AppMessageType.Info => Info,
                    AppMessageType.Warning => Warning,
                    _ => AppInfo
                };
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class AppMessagePanel : UserControl
    {
        public AppMessagePanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
