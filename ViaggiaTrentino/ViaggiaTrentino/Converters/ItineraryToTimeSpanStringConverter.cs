using Models.MobilityService.Journeys;
using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class ItineraryToTimeSpanStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Itinerary)
      {
        Itinerary tmpIti = value as Itinerary;
        
        DateTime timeBegin = new DateTime(1970, 1, 1).AddMilliseconds(System.Convert.ToDouble(tmpIti.StartTime));
        DateTime timeEnding = new DateTime(1970, 1, 1).AddMilliseconds(System.Convert.ToDouble(tmpIti.EndTime));

        return string.Format("{0} - {1} ({2}m)", timeBegin.ToString("HH:mm"), timeEnding.ToString("HH:mm"), Math.Round((timeEnding-timeBegin).TotalMinutes, 0 ));
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
