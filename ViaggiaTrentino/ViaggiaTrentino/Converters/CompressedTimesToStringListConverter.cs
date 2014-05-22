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
  public class CompressedTimesToStringListConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is string)
      {
        int i = 0;
        string ct = value as string;
        List<string> results = new List<string>();
        
        while (i < ct.Length)
        {
          if (ct[i] == '|')
          {
            results.Add("");
            i++;
          }
          else
          {
            results.Add(ct.Substring(i, 4));
            i += 4;
          }
        }
        return results;
      }
      return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
