using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class DebuggingConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var valueType = value.GetType();
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
