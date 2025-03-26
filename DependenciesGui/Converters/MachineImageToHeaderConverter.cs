using System;
using System.Windows.Data;

namespace Dependencies.Converters
{
    public class MachineImageToHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MachineType? machine = (MachineType?)value;

            if (machine != null)
            {
                switch (machine.Value)
                {
                    case MachineType.i386:
                        return "Images/machine_x86.png";
                    case MachineType.AMD64:
                        return "Images/machine_x64.png";
                    case MachineType.IA64:
                        return "Images/machine_ia64.png";
                    case MachineType.ARM64:
                    case MachineType.ARMNT:
                        return "Images/machine_arm64.png";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
