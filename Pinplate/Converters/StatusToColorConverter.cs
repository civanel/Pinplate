using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pinplate.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Открыта" => new SolidColorBrush(Color.FromRgb(0xE5, 0x73, 0x73)),
            "В работе" => new SolidColorBrush(Color.FromRgb(0xD6, 0x8C, 0x8C)),
            _ => new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}