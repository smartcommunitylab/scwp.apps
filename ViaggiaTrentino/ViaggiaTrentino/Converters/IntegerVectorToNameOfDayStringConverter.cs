using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class IntegerVectorToNameOfDayStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is int[])
      {
        StringBuilder sb = new StringBuilder();

        foreach (var day in value as int[])
        {
          sb.AppendFormat("{0}, ", ((DayOfWeek)day).ToString().Substring(0,3));
        }
        string tmp = sb.ToString().Remove(sb.ToString().Length - 2, 2);

        return tmp;
       
      }
      return "Hmm, not days";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
