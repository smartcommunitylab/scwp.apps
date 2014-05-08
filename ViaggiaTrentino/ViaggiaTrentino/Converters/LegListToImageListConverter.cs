using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ViaggiaTrentino.Converters
{
  public class LegListToImageListConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is List<Leg>)
      {
        List<Leg> tmpIti = value as List<Leg>;
        var transp = tmpIti.Select(x => x.TransportInfo.Type).Distinct();

        List<object> g = new List<object>();

        //TODO: add right icons once obtained

        foreach (string s in transp)
          if (s == "CAR")
            g.Add(new ImageSourceConverter().ConvertFromString("/Assets/HubTiles/PlanJourney.png"));
          else
            g.Add(new ImageSourceConverter().ConvertFromString("/Assets/HubTiles/ReadNotification.png"));

        return g;

        
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
