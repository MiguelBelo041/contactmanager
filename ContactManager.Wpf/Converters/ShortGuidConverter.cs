using System.Globalization;
using System.Windows.Data;

namespace ContactManager.Wpf.Converters;

public class ShortGuidConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Guid guid)
            return guid.ToString("N")[..8];

        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
