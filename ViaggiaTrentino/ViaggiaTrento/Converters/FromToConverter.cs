using Models.MobilityService.Journeys;
using System;
using System.Windows.Data;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.Converters
{
  public class FromToConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Leg )
      {
        Leg tmpLeg = value as Leg;
        return string.Format("{0} {1} {2} {3}", AppResources.From, tmpLeg.From.Name,
                                                AppResources.To, tmpLeg.To.Name);
      }
     
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
