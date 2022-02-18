using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Taskington.Gui.Views
{
    class BoolToThicknessConverter : IValueConverter
    {
        public Thickness? IfTrue { get; set; }
        public Thickness? IfFalse { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch { true => IfTrue, _ => IfFalse } ?? new Thickness(0);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
