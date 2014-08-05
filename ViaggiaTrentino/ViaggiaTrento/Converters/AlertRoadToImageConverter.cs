using Models.MobilityService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class AlertRoadToImageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      List<object> images = new List<object>();
      if (value is List<ChangeType>)
      {
        bool isDark = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;

        foreach (ChangeType ct in value as List<ChangeType>)
          images.Add(new ImageSourceConverter().ConvertFromString(string.Format("/Assets/Alert/ic_menu_alert_{0}.png", ct.ToString().ToLower())));

        return images;
      }

      images.Add(new ImageSourceConverter().ConvertFromString(string.Format("/Assets/Alert/ic_menu_alert_other.png")));
      return images;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
