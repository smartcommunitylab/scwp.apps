using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class RouteBackgroungColorToForegroundColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is string)
      {
        switch (value as string)
        {
          case "#FFF": return "#FF000000";
          case "#FFFFFF": return "#FF000000";
          default: return "#FFFFFFFF";
        }
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
