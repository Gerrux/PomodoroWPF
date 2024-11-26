using System;
using System.Globalization;
using System.Windows.Data;

namespace PomodoroWPF.Converters
{
    public class BooleanToScaleXConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFlipped = (bool)value;
            return isFlipped ? -1 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
