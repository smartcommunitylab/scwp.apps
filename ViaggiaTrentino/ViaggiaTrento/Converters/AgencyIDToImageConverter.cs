using CommonHelpers;
using DBManager.DBModels;
using Models.MobilityService;
using System;
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
        switch (EnumConverter.ToEnum<AgencyType>(route.AgencyID))
        {
          case AgencyType.BolzanoVeronaRailway: goto case AgencyType.TrentoBassanoDelGrappaRailway;
          case AgencyType.TrentoBassanoDelGrappaRailway: result = "/Assets/Vehicles/train.png"; break;
          case AgencyType.TrentoMaleRailway: result = "/Assets/Vehicles/hubtrain.png"; break;            
          case AgencyType.TrentoCityBus: goto case AgencyType.RoveretoCityBus;
          case AgencyType.RoveretoCityBus:  result = "/Assets/Vehicles/bus.png"; break;
          case AgencyType.ComuneDiTrento: goto case AgencyType.ComuneDiRovereto;
          case AgencyType.ComuneDiRovereto:  result = "/Assets/Vehicles/parking.png"; break;
          case AgencyType.BikeSharingTrento: goto case AgencyType.BikeSharingRovereto;
          case AgencyType.BikeSharingRovereto:  result = "/Assets/Vehicles/sharedbike.png"; break;
          case AgencyType.CarSharingService:  result = "/Assets/Vehicles/sharedcar.png"; break;
        }
      }

      ImageSourceToImageSourceAccordingToPhoneApplicationBackgroundConverter isisatpabc = new ImageSourceToImageSourceAccordingToPhoneApplicationBackgroundConverter();
      result = isisatpabc.Convert(null, targetType, result, culture) as string;


      return new ImageSourceConverter().ConvertFromString(result);;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
