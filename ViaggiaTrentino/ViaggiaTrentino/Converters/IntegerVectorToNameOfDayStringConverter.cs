using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ViaggiaTrentino.Converters
{
  public class IntegerVectorToNameOfDayStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is int[])
      {
        List<string> localizedDays = new List<string>();

        for (int i = 1; i <= 7; i++)
        {
          localizedDays.Add(new DateTime(1970, 2, i).ToString("ddd", DateTimeFormatInfo.CurrentInfo));
        }

        StringBuilder sb = new StringBuilder();

        foreach (var day in value as int[])
        {
          sb.AppendFormat("{0}, ", localizedDays[day-1]);
        }
        string tmp = sb.ToString().Remove(sb.ToString().Length - 2, 2);

        return tmp;
       
      }
      return "Hmm, not days";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
