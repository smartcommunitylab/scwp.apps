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

namespace ViaggiaTrentino
{
  public class GoogleAutoCompleteBox : AutoCompleteBox
  {
    string baseUrl = "http://maps.googleapis.com/maps/api/geocode/json?address=";

    WebClient webCli;

    public GoogleAutoCompleteBox()
    {
      webCli = new WebClient();
      webCli.DownloadStringCompleted += webCli_DownloadStringCompleted;
      base.TextChanged += GoogleAutoCompleteBox_TextChanged;
      base.SelectionChanged += GoogleAutoCompleteBox_SelectionChanged;
    }


    void GoogleAutoCompleteBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      /* 
       * SelectedItem changes after the user taps an item because the list of selectable items
       * gets destroyed. This causes the Event to be fired again while the selected item is still
       * being destroyed, resulting in the first if validating as true, and then associating tag to null
       */
      this.Text = this.SelectedItem != null ? (this.SelectedItem as Position).Name : this.Text;
      this.Tag = this.SelectedItem != null ? this.SelectedItem : this.Tag;
    }

    void GoogleAutoCompleteBox_TextChanged(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!webCli.IsBusy)
        UpdateData((sender as AutoCompleteBox).Text);
    }

    private void UpdateData(string text)
    {
      if (text.Length > 4)
      {
        webCli.DownloadStringAsync(new Uri(baseUrl + Uri.EscapeUriString(text)));

#if DEBUG
        Debug.WriteLine("i searched " + text);
#endif
      }

    }

    #region Google API interaction

    void webCli_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
      var gRes = JsonConvert.DeserializeObject<GoogleJSONObj>(e.Result);

      List<Position> poss = gRes.Results.Select(x => new Position()
      {
        Name = x.FormattedAddress,
        Latitude = x.Geometry.Location.Latitude.ToString(),
        Longitude = x.Geometry.Location.Longitude.ToString()
      }).ToList();

      this.ItemsSource = new ObservableCollection<Models.MobilityService.Journeys.Position>(poss);
    }

    #endregion


  }
}
