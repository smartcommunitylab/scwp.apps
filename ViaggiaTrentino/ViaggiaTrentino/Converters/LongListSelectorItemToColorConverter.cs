using CommonHelpers;
using DBManager;
using Microsoft.Phone.Shell;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ViaggiaTrentino.Helpers;

namespace ViaggiaTrentino.Converters
{
  public class LongListSelectorItemToColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is KeyedList<string, TripData>)
      {
        KeyedList<string, TripData> a = value as KeyedList<string, TripData>;
        string s = EnumConverter.ToEnumString<AgencyType>(a.First().AgencyId);
        string color;

        if (!PhoneApplicationService.Current.State.ContainsKey("routeNamesForColor"))
        {
          using (DBHelper dbh = new DBHelper())
          {
            PhoneApplicationService.Current.State.Add("routeNamesForColor", dbh.GetRouteInfo(s));
          }
        }

        try
        {
          color = (PhoneApplicationService.Current.State["routeNamesForColor"] as List
            <DBManager.DBModels.RouteInfo>).Where(x => x.Name == a.First().RouteShortName).First().Color;
        }
        catch
        {
          color = ((Color)Application.Current.Resources["PhoneAccentBrush"]).ToString();
        }

        return color;
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
