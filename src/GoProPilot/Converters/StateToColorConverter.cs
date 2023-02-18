using System;
using System.Globalization;

#if AVALONIA
using Avalonia.Data.Converters;
using Avalonia.Media;
#endif

#if WPF
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace GoProPilot.Converters;

public class StateToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is State state)
            return new SolidColorBrush(state switch
            {
                State.Active => Colors.Green,
                State.Inactive => Colors.Silver,
                State.Error => Colors.Red,
                _ => Colors.Silver,
            });

        return new SolidColorBrush(Colors.Silver);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
