using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PomodoroWPF.Converters
{
    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = value as string;
            if (!string.IsNullOrEmpty(hex))
            {
                try
                {
                    return (SolidColorBrush)new BrushConverter().ConvertFromString(hex);
                }
                catch
                {
                    return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
