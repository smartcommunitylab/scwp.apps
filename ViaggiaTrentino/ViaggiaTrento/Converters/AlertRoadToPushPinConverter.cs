using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class AlertRoadToPushPinConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is AlertRoad)
      {
        return new ImageSourceConverter().ConvertFromString(string.Format("/Assets/Alert/marker_{0}.png", (value as AlertRoad).ChangeTypes.First().ToString().ToLower()));
      }
      return new ImageSourceConverter().ConvertFromString("/Assets/Alert/marker_other.png");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
