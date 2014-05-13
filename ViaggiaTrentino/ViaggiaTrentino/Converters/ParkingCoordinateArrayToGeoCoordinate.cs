using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class ParkingCoordinateArrayToGeoCoordinate : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is double[])
      {
        double[] coord = value as double[];
        return new GeoCoordinate(coord[0], coord[1]);
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
