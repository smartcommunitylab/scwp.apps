using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class RouteBackgroungColorToForegroundColor : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is string)
      {
        switch (value as string)
        {
          case "#FFF": return "#A5000000";
          case "#FFFFFF": return "#A5000000";
          default: return "#A5FFFFFF";
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
