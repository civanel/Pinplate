using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Pinplate.Converters;

public class PointsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var pc = new PointCollection();

        if (value is ObservableCollection<System.Windows.Point> points)
        {
            foreach (var p in points)
                pc.Add(p);
        }

        return pc;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}