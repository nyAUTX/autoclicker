using System.Globalization;
using System.Windows.Data;

namespace autoclicker.Converters;

public class ZeroToZeroConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // If not zero, return the original value
        return value!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            value = "0";
        }
        return value!;
    }
}