using System;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Globalization;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using Dependencies.Converters;

namespace Dependencies
{
    internal static class NameDictionaryHelper
    {
        public static string GetDisplayName(LanguageSpecificStringDictionary nameDictionary)
        {
            // Look up the display name based on the UI culture, which is the same culture
            // used for resource loading.
            XmlLanguage userLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            // Look for an exact match.
            string name;
            if (nameDictionary.TryGetValue(userLanguage, out name))
            {
                return name;
            }

            // No exact match; return the name for the most closely related language.
            int bestRelatedness = -1;
            string bestName = string.Empty;

            foreach (KeyValuePair<XmlLanguage, string> pair in nameDictionary)
            {
                int relatedness = GetRelatedness(pair.Key, userLanguage);
                if (relatedness > bestRelatedness)
                {
                    bestRelatedness = relatedness;
                    bestName = pair.Value;
                }
            }

            return bestName;
        }

        public static string GetDisplayName(IDictionary<CultureInfo, string> nameDictionary)
        {
            // Look for an exact match.
            string name;
            if (nameDictionary.TryGetValue(CultureInfo.CurrentUICulture, out name))
            {
                return name;
            }

            // No exact match; return the name for the most closely related language.
            int bestRelatedness = -1;
            string bestName = string.Empty;

            XmlLanguage userLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            foreach (KeyValuePair<CultureInfo, string> pair in nameDictionary)
            {
                int relatedness = GetRelatedness(XmlLanguage.GetLanguage(pair.Key.IetfLanguageTag), userLanguage);
                if (relatedness > bestRelatedness)
                {
                    bestRelatedness = relatedness;
                    bestName = pair.Value;
                }
            }

            return bestName;
        }

        private static int GetRelatedness(XmlLanguage keyLang, XmlLanguage userLang)
        {
            try
            {
                // Get equivalent cultures.
                CultureInfo keyCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(keyLang.IetfLanguageTag);
                CultureInfo userCulture = CultureInfo.GetCultureInfoByIetfLanguageTag(userLang.IetfLanguageTag);
                if (!userCulture.IsNeutralCulture)
                {
                    userCulture = userCulture.Parent;
                }

                // If the key is a prefix or parent of the user language it's a good match.
                if (IsPrefixOf(keyLang.IetfLanguageTag, userLang.IetfLanguageTag) || userCulture.Equals(keyCulture))
                {
                    return 2;
                }

                // If the key and user language share a common prefix or parent neutral culture, it's a reasonable match.
                if (IsPrefixOf(TrimSuffix(userLang.IetfLanguageTag), keyLang.IetfLanguageTag) || userCulture.Equals(keyCulture.Parent))
                {
                    return 1;
                }
            }
            catch (ArgumentException)
            {
                // Language tag with no corresponding CultureInfo.
            }

            // They're unrelated languages.
            return 0;
        }

        private static string TrimSuffix(string tag)
        {
            int i = tag.LastIndexOf('-');
            if (i > 0)
            {
                return tag.Substring(0, i);
            }
            else
            {
                return tag;
            }
        }

        private static bool IsPrefixOf(string prefix, string tag)
        {
            return prefix.Length < tag.Length &&
                tag[prefix.Length] == '-' &&
                string.CompareOrdinal(prefix, 0, tag, 0, prefix.Length) == 0;
        }
    }

    internal class FontFamilyListItem : TextBlock, IComparable
    {
        private string _displayName;

        public FontFamilyListItem(FontFamily fontFamily)
        {
            _displayName = GetDisplayName(fontFamily);

            this.FontFamily = fontFamily;
            this.Text = _displayName;
            this.ToolTip = _displayName;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (IsSymbolFont(fontFamily))
            {
                TextRange range = new TextRange(this.ContentStart, this.ContentEnd);
                range.ApplyPropertyValue(TextBlock.FontFamilyProperty, SystemFonts.MessageFontFamily);
            }
        }

        public override string ToString()
        {
            return _displayName;
        }

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }

        internal static bool IsSymbolFont(FontFamily fontFamily)
        {
            foreach (Typeface typeface in fontFamily.GetTypefaces())
            {
                GlyphTypeface face;
                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }
            return false;
        }

        internal static string GetDisplayName(FontFamily family)
        {
            return NameDictionaryHelper.GetDisplayName(family.FamilyNames);
        }
    }

    internal enum UIStyledElements
    {
        ModulesTree,
        ModulesList,
        ImportsList,
        ExportsList
    }

    public partial class UserSettings : Window
    {
        private string PeviewerPath;
        private ICollection<FontFamily> _familyCollection;          // see FamilyCollection property
        private ObservableCollection<double> _fontSizes = new ObservableCollection<double>()
        {
            8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72
        };

        public UserSettings()
        {
            InitializeComponent();

            TreeBuildCombo.ItemsSource = Enum.GetValues(typeof(TreeBuildingBehaviour.DependencyTreeBehaviour));
            BinaryCacheCombo.ItemsSource = Enum.GetValues(typeof(BinaryCacheOption.BinaryCacheOptionValue));
            PeviewerPath = Properties.Settings.Default.PeViewerPath;

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitializeTargetElements();
            InitializeFontFamilyList();
            InitializeFontSizeCombo();
            DisplaySettingsForElement((UIStyledElements)Properties.Settings.Default.FontsSelectionIndex);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {
            // Update defaults
            Properties.Settings.Default.PeViewerPath = PeviewerPath;

			int TreeDepth = Properties.Settings.Default.TreeDepth;
			if (Int32.TryParse(TreeDepthValue.Text, out TreeDepth))
			{
				Properties.Settings.Default.TreeDepth = TreeDepth;
			}
			

			if (TreeBuildCombo.SelectedItem != null)
            {
                Properties.Settings.Default.TreeBuildBehaviour = TreeBuildCombo.SelectedItem.ToString();
            }

            if (BinaryCacheCombo.SelectedItem != null)
            {
                bool newValue = (bool) (new BinaryCacheOption()).ConvertBack(BinaryCacheCombo.SelectedItem, null, null, null);

                if (Properties.Settings.Default.BinaryCacheOptionValue != newValue)
                {
                    System.Windows.MessageBox.Show("The binary caching preference has been modified, you need to restart Dependencies for the modifications to be actually reloaded.");
                }

                Properties.Settings.Default.BinaryCacheOptionValue = newValue;
            }


            this.Close();
        }

        private bool SelectListItem(System.Windows.Controls.ListBox list, object value)
        {
            ItemCollection itemList = list.Items;

            // Perform a binary search for the item.
            int first = 0;
            int limit = itemList.Count;

            while (first < limit)
            {
                int i = first + (limit - first) / 2;
                IComparable item = (IComparable)(itemList[i]);
                int comparison = item.CompareTo(value);
                if (comparison < 0)
                {
                    // Value must be after i
                    first = i + 1;
                }
                else if (comparison > 0)
                {
                    // Value must be before i
                    limit = i;
                }
                else
                {
                    // Exact match; select the item.
                    list.SelectedIndex = i;
                    itemList.MoveCurrentToPosition(i);
                    list.ScrollIntoView(itemList[i]);
                    return true;
                }
            }

            // Not an exact match; move current position to the nearest item but don't select it.
            if (itemList.Count > 0)
            {
                int i = Math.Min(first, itemList.Count - 1);
                itemList.MoveCurrentToPosition(i);
                list.ScrollIntoView(itemList[i]);
            }

            return false;
        }

        private bool SelectFontSizeComboItem(double value)
        {
            ItemCollection itemList = FontSizeCombo.Items;

            foreach(var item in itemList)
            {
                if (item.Equals(value))
                {
                    FontSizeCombo.SelectedItem = item;
                    return true;
                }
            }

            if(FontSizeCombo.IsEditable)
            {
                // insert the value in the _fontSizes collection in sorted order
                int i = 0;
                while (i < _fontSizes.Count && _fontSizes[i] < value)
                {
                    i++;
                }
                _fontSizes.Insert(i, value);

                FontSizeCombo.SelectedItem = value;
                return true;
            }

            return false;
        }

        public ICollection<FontFamily> FontFamilyCollection
        {
            get
            {
                return (_familyCollection == null) ? Fonts.SystemFontFamilies : _familyCollection;
            }

            set
            {
                if (value != _familyCollection)
                {
                    _familyCollection = value;
                }
            }
        }

        private void InitializeTargetElements()
        {
            var elements = new Dictionary<UIStyledElements, string>
            {
                { UIStyledElements.ModulesTree, "Modules tree" },
                { UIStyledElements.ModulesList, "Modules list" },
                { UIStyledElements.ImportsList, "Imports list" },
                { UIStyledElements.ExportsList, "Exports list" }
            };

            TargetElementCombo.ItemsSource = elements;
            TargetElementCombo.SelectedIndex = Properties.Settings.Default.FontsSelectionIndex;
        }

        private void InitializeFontFamilyList()
        {
            ICollection<FontFamily> familyCollection = FontFamilyCollection;
            if (familyCollection != null)
            {
                FontFamilyListItem[] items = new FontFamilyListItem[familyCollection.Count];

                int i = 0;

                foreach (FontFamily family in familyCollection)
                {
                    items[i++] = new FontFamilyListItem(family);
                }

                Array.Sort(items);

                FontFamilyList.ItemsSource = items;
            }
        }

        private void InitializeFontSizeCombo()
        {
            FontSizeCombo.ItemsSource = _fontSizes;
        }

        private void DisplaySettingsForElement(UIStyledElements element)
        {
            var fontInfo = FontInfoFor(element);

            FontBoldCheckbox.IsChecked = fontInfo.IsBold;

            if (FontFamilyList.Items.Count > 0)
                SelectListItem(FontFamilyList, FontFamilyListItem.GetDisplayName(fontInfo.FontFamily));

            if(FontSizeCombo.Items.Count > 0)
                SelectFontSizeComboItem(fontInfo.FontSize);
        }

        private void OnPeviewerPathSettingChange(object sender, RoutedEventArgs e)
        {
            string programPath = Dependencies.Properties.Settings.Default.PeViewerPath;

            OpenFileDialog InputFileNameDlg = new OpenFileDialog()
            {
                Filter = "exe files (*.exe, *.dll)| *.exe;*.dll; | All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                InitialDirectory = System.IO.Path.GetDirectoryName(programPath)
            };


            if (InputFileNameDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            PeviewerPath = InputFileNameDlg.FileName;
        }

		private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = IsTextNumeric(e.Text);

		}

		private static bool IsTextNumeric(string str)
		{
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]+");
			return reg.IsMatch(str);

		}

        private void TargetElementCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.FontsSelectionIndex = TargetElementCombo.SelectedIndex;
            if(TargetElementCombo.SelectedItem is KeyValuePair<UIStyledElements, string> kvp)
            {
                DisplaySettingsForElement(kvp.Key);
            }
        }

        private void FontSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.FontsSelectionIndex = TargetElementCombo.SelectedIndex;
            if (TargetElementCombo.SelectedItem is KeyValuePair<UIStyledElements, string> kvp)
            {
                var fontInfo = FontInfoFor(kvp.Key);
                if(fontInfo != null)
                    fontInfo.FontSize = (double)FontSizeCombo.SelectedValue;
            }
        }

        private void FontBoldCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FontsSelectionIndex = TargetElementCombo.SelectedIndex;
            if (TargetElementCombo.SelectedItem is KeyValuePair<UIStyledElements, string> kvp)
            {
                var fontInfo = FontInfoFor(kvp.Key);
                if (fontInfo != null)
                    fontInfo.IsBold = FontBoldCheckbox.IsChecked ?? false;
            }
        }
        private void fontFamilyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamilyListItem item = FontFamilyList.SelectedItem as FontFamilyListItem;
            if (item != null)
            {
                Properties.Settings.Default.FontsSelectionIndex = TargetElementCombo.SelectedIndex;
                if (TargetElementCombo.SelectedItem is KeyValuePair<UIStyledElements, string> kvp)
                {
                    var fontInfo = FontInfoFor(kvp.Key);
                    if (fontInfo != null)
                        fontInfo.Font = item.FontFamily.Source;
                }
            }
        }

        private void UseFontDefaults_Click(object sender, RoutedEventArgs e)
        {
            ResetToDefaults(Properties.Settings.Default.FontModulesTree);
            ResetToDefaults(Properties.Settings.Default.FontModulesList);
            ResetToDefaults(Properties.Settings.Default.FontImportsList);
            ResetToDefaults(Properties.Settings.Default.FontExportsList);

            DisplaySettingsForElement((UIStyledElements)Properties.Settings.Default.FontsSelectionIndex);
        }

        private void ResetToDefaults(FontInfo info)
        {
            info.Font = "Consolas";
            info.FontSize = 12;
            info.IsBold = false;
        }

        private FontInfo FontInfoFor(UIStyledElements element)
        {
            switch(element)
            {
                case UIStyledElements.ModulesTree:
                    return Properties.Settings.Default.FontModulesTree;
                case UIStyledElements.ModulesList:
                    return Properties.Settings.Default.FontModulesList;
                case UIStyledElements.ImportsList:
                    return Properties.Settings.Default.FontImportsList;
                case UIStyledElements.ExportsList:
                    return Properties.Settings.Default.FontExportsList;
                default:
                    return null;
            }
        }
    }
}
