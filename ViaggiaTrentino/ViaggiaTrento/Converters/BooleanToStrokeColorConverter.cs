using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class BooleanToStrokeColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is bool)
      {
        if ((bool)value)
          return "Green";
        return "Red";
      }
      return "Gray";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
