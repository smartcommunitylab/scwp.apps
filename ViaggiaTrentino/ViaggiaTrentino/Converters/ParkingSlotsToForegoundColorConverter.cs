using Models.MobilityService.PublicTransport;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class ParkingSlotsToForegoundColorConverter : IValueConverter
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
            if (p.SlotsTotal - p.SlotsAvailable < 50)
              return Colors.Orange.ToString();
            else
              return Colors.Green.ToString();
          }
        }
        else
          return "#FF5CA3FF";
      }
      return Colors.White.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
