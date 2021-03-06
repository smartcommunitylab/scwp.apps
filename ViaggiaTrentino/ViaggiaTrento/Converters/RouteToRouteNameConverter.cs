﻿using CommonHelpers;
using Microsoft.Phone.Shell;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class RouteToRouteNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      List<DBManager.DBModels.RouteName> routeNames = PhoneApplicationService.Current.State["routeNames"] as List<DBManager.DBModels.RouteName>;
      if (value is Route)
      {  
        Route localR = value as Route;
        DBManager.DBModels.RouteName routeName = routeNames.Find(x => x.AgencyID == EnumConverter.ToEnumString<AgencyType>(localR.RouteId.AgencyId) && x.RouteID == localR.RouteId.Id);

        return routeName != null ? string.Format("{0} - {1}", localR.RouteShortName, routeName.Name) : "";
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
