#if DEBUG
using System.Diagnostics;
#endif

using Microsoft.Phone.Controls;
using Models.MobilityService.Journeys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Models.Geocoder;

namespace ViaggiaTrentino
{
  public class SmartCampusAutoCompleteBox : AutoCompleteBox
  {
    string baseUrl = "https://vas.smartcampuslab.it/core.geocoder/spring/address?address=";

    WebClient webCli;
    Position selPos;

    public SmartCampusAutoCompleteBox()
    {
      webCli = new WebClient();
      webCli.DownloadStringCompleted += webCli_DownloadStringCompleted;
      base.MinimumPopulateDelay = 1000;    
    }


    protected override void OnPopulating(PopulatingEventArgs e)
    {
      base.OnPopulating(e);      

      if (!webCli.IsBusy)
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
    }


    private void UpdateData(string text)
    {
      if (text.Length > 1)
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

    #region SmartCampus API interaction

    void webCli_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
      var gRes = JsonConvert.DeserializeObject<SCGeoJSONObj>(e.Result);

      if (gRes.Response.NumberOfResults > 0)
      {
        List<Position> poss = gRes.Response.Places.Where(x => x.Name != null).Select(x => new Position()
        {
          Name = x.ToString(),
          Latitude = x.Coordinate.Split(',')[0],
          Longitude = x.Coordinate.Split(',')[1]
        }).GroupBy(x => x.Name).Select(grp => grp.First()).ToList();

        this.ItemsSource = new ObservableCollection<Models.MobilityService.Journeys.Position>(poss);
        PopulateComplete();
      }
    }

    #endregion


  }
}
