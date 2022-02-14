using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.Views
{
    class Convert
    {
        public object? If { get; set; }
        public object? Then { get; set; }
    }

    class DynamicConverter : IValueConverter
    {
        public object? Default { get; set; }

        public List<Convert> Mappings { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var mapping = Mappings.FirstOrDefault(m => m.If?.Equals(value) ?? false);
            if (mapping != null)
            {
                return mapping.Then;
            }
            return Default;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
