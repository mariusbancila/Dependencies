using System;
using System.Windows.Data;
using System.Windows;

namespace Dependencies.Converters
{
    /// <summary>
    /// Converter to transform a boolean into a Visibility settings. 
    /// Why is this not part of the WPF standard lib ? Everybody ends up coding one in every project.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean SettingValue = (Boolean)value;

            if (SettingValue)
                return Visibility.Visible;

            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
