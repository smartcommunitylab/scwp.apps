using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.Converters
{
  public class FromToConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Leg)
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
