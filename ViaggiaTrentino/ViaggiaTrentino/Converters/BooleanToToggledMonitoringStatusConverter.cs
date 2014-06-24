using CommonHelpers;
using DBManager;
using Microsoft.Phone.Shell;
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
using ViaggiaTrentino.Helpers;
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
