using Models.MobilityService.PublicTransport;
using Models.TerritoryInformationService;
using System;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class DataContextToContentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is Parking)
        return (value as Parking).Name;
      else if (value is POIObject)
        return (value as POIObject).Title;

      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
