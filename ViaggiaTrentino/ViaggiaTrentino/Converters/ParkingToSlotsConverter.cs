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

namespace ViaggiaTrentino.Converters
{
  public class ParkingToSlotsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Parking)
      {
        Parking p = value as Parking;
        if (p.Monitored)
        {
          if (p.SlotsAvailable == -1)
            return Resources.AppResources.ParkingsSlotsFull;
          else
            return String.Format("{0}/{1}", p.SlotsAvailable, p.SlotsTotal);
        }
        else
          return p.SlotsTotal.ToString();
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
