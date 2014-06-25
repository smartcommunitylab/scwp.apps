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

        byte a = System.Convert.ToByte(Int32.Parse("FF", System.Globalization.NumberStyles.AllowHexSpecifier));
        byte r = 0;
        byte g = 0;
        byte b = 0;

        if (hexString.Length == 4)
        {
          string newHexString = hexString.Substring(1, 1) + hexString.Substring(1, 1);
          newHexString += hexString.Substring(2, 1) + hexString.Substring(2, 1);
          newHexString += hexString.Substring(3, 1) + hexString.Substring(3, 1);
          hexString = "#" + newHexString;
        }

        if (hexString.StartsWith("#"))
        {
          if (hexString.Length == 9)
          {
            a = System.Convert.ToByte(Int32.Parse(hexString.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            hexString = hexString.Substring(3, 6);
          }
          else
            hexString = hexString.Substring(1, 6);
        }

        r = System.Convert.ToByte(Int32.Parse(hexString.Substring(0, 2),
          System.Globalization.NumberStyles.AllowHexSpecifier));
        g = System.Convert.ToByte(Int32.Parse(hexString.Substring(2, 2),
            System.Globalization.NumberStyles.AllowHexSpecifier));
        b = System.Convert.ToByte(Int32.Parse(hexString.Substring(4, 2),
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
