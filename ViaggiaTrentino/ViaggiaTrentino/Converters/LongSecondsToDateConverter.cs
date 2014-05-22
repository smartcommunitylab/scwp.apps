using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class LongSecondsToDateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is long)
      {        
        DateTime time = new DateTime(1970, 1, 1).AddSeconds(System.Convert.ToDouble(value));

        return time.ToString("dd/MM/yyyy");
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    
  }
}
