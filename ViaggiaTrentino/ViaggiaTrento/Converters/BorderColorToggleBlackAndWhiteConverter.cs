using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class BorderColorToggleBlackAndWhiteConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string color = null;

      if (value is SolidColorBrush)
        color = (value as SolidColorBrush).Color.ToString();

      if (value is string)
        color = value as string;

      if (color != null)
      {
        switch (color as string)
        {
          case "#FFF":
          case "#FFFFFFFF":
          case "#FFFFFF": return "#FF000000";
          case "#000":
          case "#FF000000":
          case "#000000": return "#FFFFFFFF";
          default: return color;
        }
      }
      return color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
