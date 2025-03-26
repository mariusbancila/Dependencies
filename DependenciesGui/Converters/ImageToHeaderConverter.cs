using Dependencies.ClrPh;
using System;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Dependencies.Converters
{
    public class ImageToHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Filepath = (string)value;

            if (NativeFile.Exists(Filepath))
            {
                Icon icon = ShellIcon.GetSmallIcon(Filepath);
                if (icon != null)
                {
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            new Int32Rect(0, 0, icon.Width, icon.Height),
                            BitmapSizeOptions.FromEmptyOptions());
                }
            }

            return "Images/Question.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
