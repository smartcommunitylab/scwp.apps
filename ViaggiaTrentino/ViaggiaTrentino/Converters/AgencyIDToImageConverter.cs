﻿using CommonHelpers;
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
        switch (EnumConverter.ToEnum<AgencyType>(route.AgencyID))
        {
          case AgencyType.BolzanoVeronaRailway: goto case AgencyType.TrentoBassanoDelGrappaRailway;
          case AgencyType.TrentoBassanoDelGrappaRailway: result = "/Assets/Vehicles/train.png"; break;
          case AgencyType.TrentoMaleRailway: result = "/Assets/HubTiles/Train.png"; break;            
          case AgencyType.TrentoCityBus: goto case AgencyType.RoveretoCityBus;
          case AgencyType.RoveretoCityBus:  result = "/Assets/Vehicles/bus.png"; break;
          case AgencyType.ComuneDiTrento: goto case AgencyType.ComuneDiRovereto;
          case AgencyType.ComuneDiRovereto:  result = "/Assets/HubTiles/Parking.png"; break;
          case AgencyType.BikeSharingTrento: goto case AgencyType.BikeSharingRovereto;
          case AgencyType.BikeSharingRovereto:  result = "/Assets/Vehicles/sharedbike.png"; break;
          case AgencyType.CarSharingService:  result = "/Assets/Vehicles/sharedcar.png"; break;
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
