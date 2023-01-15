using System;
using System.Globalization;
using System.Windows.Data;
using Starlex.Wpf.Effects;

namespace GoProPilot.Converters;

public class GrayscaleConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b)
        {
            return new GrayscaleEffect()
            {
                DesaturationFactor = 0,
            };
        }
        else
            return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
