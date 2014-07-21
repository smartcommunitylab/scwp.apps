using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace ViaggiaTrentino.Helpers
{
  class AssociationUriMapper : UriMapperBase
  {
    private string tempUri;

    private Uri DirectionsServiceLoader(string tempUri)
    {
      string querystring = tempUri.Substring(tempUri.IndexOf("smartcampuslab")).Split('?')[1];
      List<string> positionToNavigate = new List<string>(querystring.Split('&'));
      Dictionary<string, double> coords = new Dictionary<string, double>();

      foreach (var coordinate in positionToNavigate)
      {
#if DEBUG
          System.Diagnostics.Debug.WriteLine(coordinate);
#endif
        string[] data = coordinate.Split('=');
        coords.Add(data[0], Convert.ToDouble(data[1]));
      }

      PhoneApplicationService.Current.State["navigationCoord"] = new double[] { coords["lat"], coords["lng"] };

      return new Uri("/Views/PlanNewSingleJourneyView.xaml", UriKind.Relative);
    }

    public override Uri MapUri(Uri uri)
    {
      tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

      if (tempUri.Contains("smartcampuslab:"))
      {
        // to expose any additional component of the application to external apps.
        // just add the required action in an additional IF, as the following one:
        if(tempUri.Contains("NavigateTowards?lat="))
          return DirectionsServiceLoader(tempUri);
      }

      // Otherwise perform normal launch.
      return uri;
    }
  }
}

