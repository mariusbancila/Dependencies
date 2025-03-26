using System;
using System.Windows.Data;

namespace Dependencies.Converters
{
    public class OverlayImageToHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ModuleFlag Flags = (ModuleFlag)value;

            // ext-ms api are considered optional
            if (((Flags & ModuleFlag.NotFound) != 0) && ((Flags & ModuleFlag.ApiSetExt) == 0))
            {
                return "Images/InvalidOverlay.png";
            }

            bool DelayLoadModule = (Flags & ModuleFlag.DelayLoad) != 0;
            if (DelayLoadModule)
            {
                return "Images/HourglassOverlay.png";
            }

            // How to handle delay-load + missing import?
            if ((Flags & ModuleFlag.MissingImports) != 0)
            {
                return "Images/InvalidOverlay.png";
            }

            bool ClrAssembly = (Flags & ModuleFlag.ClrReference) != 0;
            if (ClrAssembly)
            {
                return "Images/ReferenceOverlay.png";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
