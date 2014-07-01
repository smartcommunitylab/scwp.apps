using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class BorderColorToggleBlackAndWhiteConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is string)
      {
        switch (value as string)
        {
          case "#FFF": return "#FF000000";
          case "#FFFFFF": return "#FF000000";
          case "#000": return "#FFFFFFFF";
          case "#000000": return "#FFFFFFFF";
          default: return value;
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
