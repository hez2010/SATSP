using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SATSP
{
    public sealed class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            if (value is null) return true;
            throw new InvalidCastException("Source type is not boolean.");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            throw new InvalidCastException("Source type is not boolean.");
        }
    }
}
