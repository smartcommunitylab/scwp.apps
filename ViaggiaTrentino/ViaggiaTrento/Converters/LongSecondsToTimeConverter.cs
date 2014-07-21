using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class LongSecondsToTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is long)
      {        
        DateTime time = new DateTime(1970, 1, 1).AddSeconds(System.Convert.ToDouble(value));

        return time.ToString("HH:mm");
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
