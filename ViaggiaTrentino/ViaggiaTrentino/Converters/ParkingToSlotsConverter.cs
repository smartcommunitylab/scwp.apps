using Models.MobilityService.PublicTransport;
using System;
using System.Windows.Data;

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
