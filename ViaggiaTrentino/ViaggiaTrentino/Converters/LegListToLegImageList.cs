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
  public class LegListToLegImageList : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is List<Leg>)
      {
        List<Leg> tmpLegs = value as List<Leg>;
        var transp = tmpLegs.Select(x => x.TransportInfo.Type);

        List<object> bitmapImages = new List<object>();

        //TODO: add right icons once obtained

        foreach (TransportType s in transp)        
          bitmapImages.Add(new ImageSourceConverter().ConvertFromString(string.Format("/Assets/Vehicles/{0}.png", s.ToString().ToLower()))); 
          
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
