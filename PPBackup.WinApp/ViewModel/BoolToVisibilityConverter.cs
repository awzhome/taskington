using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PPBackup.WinApp.ViewModel
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsNegated { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!IsNegated == (value as bool?).GetValueOrDefault()) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !IsNegated == ((value as Visibility?).GetValueOrDefault() == Visibility.Visible);
        }
    }
}
