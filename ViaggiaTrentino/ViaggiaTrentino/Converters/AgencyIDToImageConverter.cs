using DBManager.DBModels;
using Models.MobilityService;
using Models.MobilityService.Journeys;
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
  public class AgencyIDToImageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string result = "/Assets/Vehicles/transit.png";

      if (value is RouteName)      
      {
        RouteName route = value as RouteName;
        switch (route.AgencyID)
        {
          case "5": goto case "6";
          case "6": result = "/Assets/Vehicles/train.png"; break;
          case "10": result = "/Assets/HubTiles/Train.png"; break;            
          case "12": goto case "16";
          case "16":  result = "/Assets/Vehicles/bus.png"; break;
          case "COMUNE_DI_TRENTO": goto case "COMUNE_DI_ROVERETO";
          case "COMUNE_DI_ROVERETO":  result = "/Assets/HubTiles/Parking.png"; break;
          case "BIKE_SHARING_TRENTO": goto case "BIKE_SHARING_ROVERETO";
          case "BIKE_SHARING_ROVERETO":  result = "/Assets/Vehicles/sharedbike.png"; break;
          case "CAR_SHARING_SERVICE":  result = "/Assets/Vehicles/sharedcar.png"; break;
        }
      }



      return new ImageSourceConverter().ConvertFromString(result);;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
