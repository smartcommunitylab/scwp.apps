using Models.MobilityService;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class TransportTypeToImageListConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isDark = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;
      if (value is TransportType)      
        return new ImageSourceConverter().ConvertFromString(string.Format("/Assets/Vehicles/{0}/{1}.png",
          isDark ? "Dark" : "Light", value.ToString().ToLower()));         
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
