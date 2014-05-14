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
  public class ParkingSlotsToForegoundColor : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Parking)
      {
        Parking p = value as Parking;
        if (p.Monitored)
        {
          if (p.SlotsAvailable == -1)
            return Colors.Red.ToString();
          else
          {
            if (p.SlotsTotal - p.SlotsAvailable < 10)
              return Colors.Orange.ToString();
            else
              return Colors.Green.ToString();
          }
        }
        else
          return "#FF5CA3FF";// Colors.Blue.ToString(); 
      }
      return Colors.White.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
