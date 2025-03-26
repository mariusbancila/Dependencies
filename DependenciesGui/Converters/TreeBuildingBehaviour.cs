using System;
using System.Windows.Data;

namespace Dependencies.Converters
{
    /// <summary>
    /// Dependency tree building behaviour.
    /// A full recursive dependency tree can be memory intensive, therefore the
    /// choice is left to the user to override the default behaviour.
    /// </summary>
    public class TreeBuildingBehaviour : IValueConverter
    {
        public enum DependencyTreeBehaviour
        {
            ChildOnly,
            RecursiveOnlyOnDirectImports,
            Recursive,

        }

        public static DependencyTreeBehaviour GetGlobalBehaviour()
        {
            return (DependencyTreeBehaviour)(new TreeBuildingBehaviour()).Convert(
                Dependencies.Properties.Settings.Default.TreeBuildBehaviour,
                null,// targetType
                null,// parameter
                null // System.Globalization.CultureInfo
            );
        }

        #region TreeBuildingBehaviour.IValueConverter_contract
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string StrBehaviour = (string)value;

            switch (StrBehaviour)
            {
                default:
                case "ChildOnly":
                    return DependencyTreeBehaviour.ChildOnly;
                case "RecursiveOnlyOnDirectImports":
                    return DependencyTreeBehaviour.RecursiveOnlyOnDirectImports;
                case "Recursive":
                    return DependencyTreeBehaviour.Recursive;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DependencyTreeBehaviour Behaviour = (DependencyTreeBehaviour)value;

            switch (Behaviour)
            {
                default:
                case DependencyTreeBehaviour.ChildOnly:
                    return "ChildOnly";
                case DependencyTreeBehaviour.RecursiveOnlyOnDirectImports:
                    return "RecursiveOnlyOnDirectImports";
                case DependencyTreeBehaviour.Recursive:
                    return "Recursive";
            }
        }
        #endregion TreeBuildingBehaviour.IValueConverter_contract
    }
}
