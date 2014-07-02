using System;
using System.Windows.Data;

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
          case "#FFF": 
          case "#FFFFFF": return "#FF000000";
          case "#000": 
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
