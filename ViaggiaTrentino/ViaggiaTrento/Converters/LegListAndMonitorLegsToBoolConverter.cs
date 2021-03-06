﻿using Microsoft.Phone.Shell;
using Models.MobilityService.Journeys;
using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class LegListAndMonitorLegsToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is SimpleLeg)
      {
        BasicRecurrentJourney brj = PhoneApplicationService.Current.State["journey"] as BasicRecurrentJourney;
        
        SimpleLeg tmpLeg = value as SimpleLeg;
        string key = string.Format("{0}_{1}", tmpLeg.TransportInfo.AgencyId, tmpLeg.TransportInfo.RouteId);
          return brj.Data.MonitorLegs.ContainsKey(key) ? brj.Data.MonitorLegs[key]: false;

      }
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
