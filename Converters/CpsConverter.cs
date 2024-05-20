using System.Globalization;
using System.Windows.Data;

namespace autoclicker.Converters;

public class CpsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Convert zero to an empty string
        if (value is int intValue && intValue == 0)
        {
            return string.Empty;
        }

        // If not zero, return the original value
        return value!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            value = "0";
        }
        
        return int.Parse((string)value) > 2000 ? 2000 : value!;
    }
    
}