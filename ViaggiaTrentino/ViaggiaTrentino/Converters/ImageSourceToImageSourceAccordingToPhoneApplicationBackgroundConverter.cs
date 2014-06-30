using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class ImageSourceToImageSourceAccordingToPhoneApplicationBackgroundConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isDark = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;
      List<string> gg = new List<string>((parameter as string).Split('/'));
      gg.Insert(gg.Count - 1, isDark ? "Dark" : "Light");
      var a = string.Join("/", gg);
      return a;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
