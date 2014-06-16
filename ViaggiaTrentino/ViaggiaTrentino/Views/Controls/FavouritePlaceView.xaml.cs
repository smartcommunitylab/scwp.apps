using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using Models.MobilityService.Journeys;
using System.Threading.Tasks;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;
using Windows.Phone.Devices.Notification;
using ViaggiaTrentino.Resources;
using System.Windows.Controls.Primitives;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class FavouritePlaceView : UserControl
  {
    private Popup pu;

    public Position SelectedPosition
    {
      get { return smartCampusACB.Tag as Position; }
    }

    public FavouritePlaceView()
    {      
      InitializeComponent();
      pu = new Popup();
    }

    #region Addresses management

    public Task<string> GetAddressFromGeoCoord(GeoCoordinate position)
    {
      return GetAddressFromGeoCoord(new double[] { position.Latitude, position.Longitude });
    }

    public async Task<string> GetAddressFromGeoCoord(double[] position)
    {
      ReverseGeocodeQuery reverseGeocode = new ReverseGeocodeQuery();
      reverseGeocode.GeoCoordinate = new GeoCoordinate(position[0], position[1]);
      var t = await reverseGeocode.GetMapLocationsAsync();
      if (t.Count > 0)
        return MapAddressToString(t[0].Information.Address);
      else return "";
    }

    private string MapAddressToString(MapAddress mapa)
    {
      string result = string.Format("{0}, {1}, {2}, {4}, {5}", mapa.BuildingName, mapa.Street, mapa.HouseNumber, mapa.City, mapa.PostalCode, mapa.Country);
      return result.StartsWith(",") ? result.Substring(2) : result;
    }

    #endregion

    #region LocationSelector


    void mp_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
    {
      MessagePrompt mp = sender as MessagePrompt;
      switch (mp.Value)
      {
        case "current": GeneratePushpinForAssegna(); break;
        case "openMap": ShowMappaGrande(); break;
        default: break;
      }
    }

    private async void GeneratePushpinForAssegna()
    {
      string res = await GetAddressFromGeoCoord(Settings.GPSPosition);
      Assegna(new Pushpin() { GeoCoordinate = Settings.GPSPosition, Content = res });
    }

    #endregion

    #region Map popup

    public void ShowMappaGrande()
    {
      Map ggm = new Map();
      pu.Child = ggm;
      ggm.ZoomLevel = 15;
      ggm.Center = Settings.GPSPosition;
      ggm.Height = Application.Current.Host.Content.ActualHeight;
      ggm.Width = Application.Current.Host.Content.ActualWidth;
      ggm.Hold += ggm_Hold;
      pu.IsOpen = true;
      //   NotifyOfPropertyChange(() => IsAppbarShown);

    }

    async void ggm_Hold(object sender, System.Windows.Input.GestureEventArgs e)
    {
      Map mappa = sender as Map;
      GeoCoordinate geocode = mappa.ConvertViewportPointToGeoCoordinate(e.GetPosition(pu));
      string aa = await GetAddressFromGeoCoord(geocode);
      Pushpin p = new Pushpin()
      {
        GeoCoordinate = geocode,
        Content = aa
      };
      p.Tap += p_Tap;
      MapExtensions.GetChildren(mappa).Clear();
      MapExtensions.GetChildren(mappa).Add(p);

      VibrationDevice vibro = VibrationDevice.GetDefault();
      vibro.Vibrate(new TimeSpan(0, 0, 1));

      //uncomment to enable hold to message box flow
      //p_Tap(p, e);
    }

    void p_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (MessageBox.Show((sender as Pushpin).Content as string, AppResources.ChooseConfirmTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        pu.IsOpen = false;
        Assegna((sender as Pushpin));
      }
    }

    public void Assegna(Pushpin result)
    {
      Position choosenPos = new Position()
      {
        Name = result.Content as string,
        Latitude = result.GeoCoordinate.Latitude.ToString(),
        Longitude = result.GeoCoordinate.Longitude.ToString()
      };
      smartCampusACB.Tag = choosenPos;
      smartCampusACB.Text = choosenPos.Name;
    }

    #endregion

    private void GpsLocTo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      MessagePrompt mp = new MessagePrompt();
      mp.ActionPopUpButtons.Clear();
      mp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      mp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      mp.Margin = new System.Windows.Thickness(10);
      mp.Title = null;
      mp.Body = new SelectLocationView(mp);
      mp.Completed += mp_Completed;
      mp.Show();
    }
  }
}
