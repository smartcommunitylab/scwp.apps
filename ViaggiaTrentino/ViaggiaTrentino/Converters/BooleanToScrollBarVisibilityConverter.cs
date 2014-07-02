using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class BooleanToScrollBarVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is bool)
      {
        if ((bool)value)
          return ScrollBarVisibility.Hidden;
      }
      return ScrollBarVisibility.Disabled;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
