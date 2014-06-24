using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;
using Windows.Phone.Devices.Notification;

namespace ViaggiaTrentino.Helpers
{
  /*
   * Quick flow of the inner workings of this helper:
   * 
   * 1. Page registers to PositionObtained event
   * 
   * 2. ShowLocationSelectorHelper() is called. A popup is shown asking how the user wants to obtain
   * his location, he can choose between GPS fix and point on map
   * 
   * 3a. GPS fix => GPS gets current location => PositionObtained event is fired and popup closed
   * 
   * 3b Point on map => map is shown, cnetered on last known location => user taps and hold a position =>
   * => pushpin appears on location + vibration => user taps pushpin => prompt for confirmation of position =>
   * => PositionObtained event is fired and popup closed. Result is Position instance if the user selected a position,
   * null otherwise
   * 
   * 
   */
  public class LocationChooserHelper
  {
    public delegate void BatchOperationCompletedHandler(object sender, Position results);
    public event BatchOperationCompletedHandler PositionObtained;

    MessagePrompt modeChooser;
    MessagePrompt hugeMap;
    
    #region Addresses management

    /// <summary>
    /// Converts a given GeoCoordinate position (i.e. the result of a GPS fix) into a street address
    /// </summary>
    /// <param name="position">a GeoCoordinate object with the desired position</param>
    /// <returns></returns>
    public Task<string> GetAddressFromGeoCoord(GeoCoordinate position)
    {
      
      return GetAddressFromGeoCoord(new double[] { position.Latitude, position.Longitude });
    }

    /// <summary>
    /// Converts a given position, contained in a double array (into a street address
    /// </summary>
    /// <param name="position">a two element double array, first element latitude, second longitude</param>
    /// <returns></returns>
    public async Task<string> GetAddressFromGeoCoord(double[] position)
    {      
      ReverseGeocodeQuery reverseGeocode = new ReverseGeocodeQuery();
      reverseGeocode.GeoCoordinate = new GeoCoordinate(position[0], position[1]);
      var t = await reverseGeocode.GetMapLocationsAsync();
      if (t.Count > 0)
        return MapAddressToString(t[0].Information.Address);
      else return "";
    }

    /// <summary>
    /// Formats a MapAddress object into a readable string
    /// </summary>
    /// <param name="mapa">the MapAddress object to convert</param>
    /// <returns>the formatted string</returns>
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
        default: PositionObtained(this, null); break;
      }     
    }

    private async void GeneratePushpinForAssegna()
    {
      string res = await GetAddressFromGeoCoord(Settings.GPSPosition);
      Assegna(new Pushpin() { GeoCoordinate = Settings.GPSPosition, Content = res });
    }

    #endregion

    #region Map popup

    private void ShowMappaGrande()
    {
      Map ggm = new Map();      
      ggm.ZoomLevel = 15;
      ggm.Center = Settings.GPSPosition;
      ggm.Height = Application.Current.Host.Content.ActualHeight;
      ggm.Width = Application.Current.Host.Content.ActualWidth;
      ggm.Hold += ggm_Hold;

      hugeMap = new MessagePrompt();
      hugeMap.Completed += hugeMap_Completed;
      hugeMap.Style = Application.Current.Resources["mpNoBorders"] as Style;
      hugeMap.Body = ggm;
      hugeMap.Show();
    
    }

    void hugeMap_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
    {
      if (hugeMap.Value != "yes")
        PositionObtained(this, null);
    }

    async void ggm_Hold(object sender, System.Windows.Input.GestureEventArgs e)
    {
      Map mappa = sender as Map;
      GeoCoordinate geocode = mappa.ConvertViewportPointToGeoCoordinate(e.GetPosition(hugeMap));
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
        hugeMap.Value = "yes";
        hugeMap.Hide();
        Assegna((sender as Pushpin));
      }
    }

    /// <summary>
    /// Creates a Position element from the selected position 
    /// and fires the Positionobtained event
    /// </summary>
    /// <param name="result">the pushpin containing the required position</param>
    private void Assegna(Pushpin result)
    {
      Position choosenPos = new Position()
      {
        Name = result.Content as string,
        Latitude = result.GeoCoordinate.Latitude.ToString(),
        Longitude = result.GeoCoordinate.Longitude.ToString()
      };

      PositionObtained(this, choosenPos);
    }

    #endregion

    /// <summary>
    /// Shows the dialog screen to select a position
    /// </summary>
    public void ShowLocationSelectorHelper()
    {
      modeChooser = new MessagePrompt();
      modeChooser.ActionPopUpButtons.Clear();
      modeChooser.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      modeChooser.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      modeChooser.Margin = new System.Windows.Thickness(10);
      modeChooser.Title = null;
      modeChooser.Body = new SelectLocationView(modeChooser);
      modeChooser.Completed += mp_Completed;
      modeChooser.IsAppBarVisible = false;
      modeChooser.Show();
    }

  }
}
