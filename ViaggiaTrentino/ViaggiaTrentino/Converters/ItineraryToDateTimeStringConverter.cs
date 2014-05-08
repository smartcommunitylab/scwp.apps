﻿using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class ItineraryToDateTimeStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Itinerary)
      {
        Itinerary tmpIti = value as Itinerary;
        DateTime timeBegin = new DateTime(1970, 1, 1).AddSeconds((tmpIti.StartTime));
        DateTime timeEnding = new DateTime(1970, 1, 1).AddSeconds((tmpIti.EndTime));

        return string.Format("{0} {1} - {2}", timeBegin.ToShortDateString(), timeBegin.ToShortTimeString(), timeEnding.ToShortTimeString());
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}