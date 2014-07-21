using Models.MobilityService;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class LegListToLegImageListConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is List<Leg>)
      {
        bool isDark = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;
        List<Leg> tmpLegs = value as List<Leg>;
        var transp = tmpLegs.Select(x => x.TransportInfo.Type);

        List<object> bitmapImages = new List<object>();

        //TODO: add right icons once obtained

        foreach (TransportType s in transp)        
          bitmapImages.Add(new ImageSourceConverter().ConvertFromString(string.Format("/Assets/Vehicles/{0}/{1}.png",
            isDark ? "Dark" : "Light", s.ToString().ToLower()))); 
          
        return bitmapImages;
        
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
