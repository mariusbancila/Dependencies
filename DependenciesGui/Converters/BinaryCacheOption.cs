using System;
using System.ComponentModel;
using System.Windows.Data;

namespace Dependencies.Converters
{
    /// <summary>
    /// Dependency tree building behaviour.
    /// A full recursive dependency tree can be memory intensive, therefore the
    /// choice is left to the user to override the default behaviour.
    /// </summary>
    public class BinaryCacheOption : IValueConverter
    {
        [TypeConverter(typeof(EnumToStringUsingDescription))]
        public enum BinaryCacheOptionValue
        {
            [Description("No (faster, but locks dll until Dependencies is closed)")]
            No = 0,

            [Description("Yes (prevents file locking issues)")]
            Yes = 1
        }

        public static BinaryCacheOptionValue GetGlobalBehaviour()
        {
            return (BinaryCacheOptionValue)(new BinaryCacheOption()).Convert(
                Dependencies.Properties.Settings.Default.BinaryCacheOptionValue,
                null,// targetType
                null,// parameter
                null // System.Globalization.CultureInfo
            );
        }

        #region BinaryCacheOption.IValueConverter_contract
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool StrOption = (bool)value;

            switch (StrOption)
            {
                default:
                case true:
                    return BinaryCacheOptionValue.Yes;
                case false:
                    return BinaryCacheOptionValue.No;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BinaryCacheOptionValue Behaviour = (BinaryCacheOptionValue)(int)value;

            switch (Behaviour)
            {
                default:
                case BinaryCacheOptionValue.Yes:
                    return true;
                case BinaryCacheOptionValue.No:
                    return false;
            }
        }
        #endregion BinaryCacheOption.IValueConverter_contract
    }
}
