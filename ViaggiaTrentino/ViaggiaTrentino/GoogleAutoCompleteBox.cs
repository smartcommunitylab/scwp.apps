#if DEBUG
using System.Diagnostics;
#endif

using Microsoft.Phone.Controls;
using MobilityServiceLibrary;
using Models.GoogleMapsAPI;
using Models.MobilityService.Journeys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Models.Geocoder;

namespace ViaggiaTrentino
{
  public class GoogleAutoCompleteBox : AutoCompleteBox
  {
    string baseUrl = "https://vas.smartcampuslab.it/core.geocoder/spring/address?address=";

    WebClient webCli;

    bool textChanged;
    Position selPos;

    public GoogleAutoCompleteBox()
    {
      webCli = new WebClient();
      webCli.DownloadStringCompleted += webCli_DownloadStringCompleted;
      base.MinimumPopulateDelay = 1000;    
      textChanged = false;
    }

    protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
    {
      base.OnKeyUp(e);
      textChanged = true;
    }

    protected override void OnPopulating(PopulatingEventArgs e)
    {
      if (!textChanged)
        e.Cancel = true;
      
      base.OnPopulating(e);      

      if (textChanged && !webCli.IsBusy)
        UpdateData((this as AutoCompleteBox).Text);
    }


    protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);
      /* 
       * SelectedItem changes after the user taps an item because the list of selectable items
       * gets destroyed. This causes the Event to be fired again while the selected item is still
       * being destroyed, resulting in the first if validating as true, and then associating tag to null
       */
      selPos = this.SelectedItem != null ? this.SelectedItem as Position : selPos;
      this.Tag = selPos;    
      textChanged = false;
    }

    protected override void OnDropDownClosed(System.Windows.RoutedPropertyChangedEventArgs<bool> e)
    {
      base.OnDropDownClosed(e);
      if (selPos != null)
        this.Text = selPos.Name;
    }

    private void UpdateData(string text)
    {
      if (text.Length > 4)
      {
        
        string completeUrl = baseUrl + Uri.EscapeUriString(text);
        if(Settings.LocationConsent && Settings.GPSPosition != null)
        {
          string location = string.Format("&latlng={0},{1}&distance=25", Settings.GPSPosition.Latitude.ToString().Replace(',', '.'),
                                                            Settings.GPSPosition.Longitude.ToString().Replace(',', '.'));
          completeUrl += Uri.EscapeUriString(location);
        }
        webCli.DownloadStringAsync(new Uri(completeUrl));

#if DEBUG
        Debug.WriteLine("i searched " + text);
#endif
      }

    }

    #region Google API interaction

    void webCli_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
      var gRes = JsonConvert.DeserializeObject<SCGeoJSONObj>(e.Result);

      List<Position> poss = gRes.Response.Places.Where(x=>x.Name != null).Select(x => new Position()
      {
        Name = x.Name, //FormattedAddress,
        Latitude = x.Coordinate.Split(',')[0],
        Longitude = x.Coordinate.Split(',')[1]
      }).ToList();

      this.ItemsSource = new ObservableCollection<Models.MobilityService.Journeys.Position>(poss);
      PopulateComplete();
    }

    #endregion


  }
}
