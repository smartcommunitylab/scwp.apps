using CommonHelpers;
using DBManager.DBModels;
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
  public class StringHexColorToColorsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is string)
      {
        string hexString = value as string;
        
        int n = 1;
        if (hexString.Length == 4)
          n = 2;

        if (hexString.StartsWith("#"))
          hexString = hexString.Substring(1, 6/n);



        byte a = System.Convert.ToByte(Int32.Parse("FF", System.Globalization.NumberStyles.AllowHexSpecifier));
        byte r = 0;
        byte g = 0;
        byte b = 0;

        r = System.Convert.ToByte(Int32.Parse(hexString.Substring(0 / n, 2 / n),
          System.Globalization.NumberStyles.AllowHexSpecifier));
        g = System.Convert.ToByte(Int32.Parse(hexString.Substring(2 / n, 2 / n),
            System.Globalization.NumberStyles.AllowHexSpecifier));
        b = System.Convert.ToByte(Int32.Parse(hexString.Substring(4 / n, 2 / n),
            System.Globalization.NumberStyles.AllowHexSpecifier));

        return Color.FromArgb(a, r, g, b);
      }
      return Colors.Transparent;
    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
