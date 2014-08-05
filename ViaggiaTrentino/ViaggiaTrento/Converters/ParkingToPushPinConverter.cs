using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class ParkingToPushPinConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Parking)
      {
        Parking p = value as Parking;

        if (p.Monitored)
        {
          if (p.SlotsAvailable == -1)
            return new ImageSourceConverter().ConvertFromString("/Assets/Miscs/marker_parking_red.png");
          else
          {
            if (p.SlotsTotal - p.SlotsAvailable < 50)
              return new ImageSourceConverter().ConvertFromString("/Assets/Miscs/marker_parking_orange.png");
            else
              return new ImageSourceConverter().ConvertFromString("/Assets/Miscs/marker_parking_green.png");
          }
        }
      }
      return new ImageSourceConverter().ConvertFromString("/Assets/Miscs/marker_parking.png");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
