using System;
using System.Windows.Data;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.Converters
{
  public class BooleanToToggledMonitoringStatusConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is bool)
      {
        if ((bool)value)
          return AppResources.SavedJourneyMonitorOFF;
        return AppResources.SavedJourneyMonitorON;

      }
      return AppResources.SavedJourneyMonitorOFF;

    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
