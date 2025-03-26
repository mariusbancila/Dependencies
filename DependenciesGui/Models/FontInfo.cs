using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Dependencies
{
    public class FontInfo : INotifyPropertyChanged
    {
        private FontFamily _fontFamily;
        private string _fontName;
        private double _fontSize;
        private bool _isBold;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Font
        {
            get
            {
                return _fontName;
            }
            set
            {
                _fontName = value;
                _fontFamily = new FontFamily(value);

                OnPropertyChanged(nameof(Font));
                OnPropertyChanged(nameof(FontFamily));
            }
        }
        public double FontSize 
        { 
            get
            {
                return _fontSize;
            }
            set
            { 
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public bool IsBold
        {
            get
            {
                return _isBold;
            }
            set
            {
                _isBold = value;
                OnPropertyChanged(nameof(IsBold));
                OnPropertyChanged(nameof(FontWeight));
            }
        }

        public FontFamily FontFamily => _fontFamily;
        public FontWeight FontWeight => IsBold ? FontWeights.Bold : FontWeights.Normal;

        #region INotifyPropertyChanged

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class FontInfoCollection : List<FontInfo>
    { }
}
