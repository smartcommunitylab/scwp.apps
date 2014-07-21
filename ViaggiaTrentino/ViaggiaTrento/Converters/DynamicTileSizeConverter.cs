using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class DynamicTileSizeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (System.Convert.ToDouble(value) - (System.Convert.ToDouble(parameter) * 6)) / 3;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
