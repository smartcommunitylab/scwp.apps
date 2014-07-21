using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ViaggiaTrentino.Helpers
{
  class AssociationUriMapper : UriMapperBase
  {
    private string tempUri;

    public override Uri MapUri(Uri uri)
    {
        tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

        // URI association launch for contoso.
        if (tempUri.Contains("smartcampuslab:NavigateTowards?lat="))
        {                
            string querystring = tempUri.Substring(tempUri.IndexOf("smartcampuslab")).Split('?')[1];
             
            List<string> positionToNavigate = new List<string>(querystring.Split('&'));

          Dictionary<string, double> coords = new Dictionary<string,double>();

          foreach (var coordinate in positionToNavigate)
	        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(coordinate);
#endif 
            string[] data = coordinate.Split('=');
            coords.Add(data[0], Convert.ToDouble(data[1]));
	        }

          PhoneApplicationService.Current.State["navigationCoord"] = new double[] { coords["lat"], coords["lng"] };

            // Map the show products request to ShowProducts.xaml
          
            return new Uri("/Views/PlanNewSingleJourneyView.xaml", UriKind.Relative);
        }

        // Otherwise perform normal launch.
        return uri;  
    }
  }
}

