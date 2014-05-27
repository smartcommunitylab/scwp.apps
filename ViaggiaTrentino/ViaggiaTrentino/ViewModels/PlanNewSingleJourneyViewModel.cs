using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Devices;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;
using Windows.Phone.Devices.Notification;

namespace ViaggiaTrentino.ViewModels
{
  public class PlanNewSingleJourneyViewModel: Screen
  {
    private readonly INavigationService navigationService;
    bool isSettingsShown;
    SingleJourney journey;
    DateTime departureDate;
    Popup pu;
    string fromText;
    string toText;
    private string locationResult;
    
    public PlanNewSingleJourneyViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      departureDate = DateTime.Now;
      journey = new SingleJourney();
      pu = new Popup();
      pu.Closed += pu_Closed;
      
    }

    void pu_Closed(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => IsAppbarShown);
    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

      if (PhoneApplicationService.Current.State.ContainsKey("parkCoord"))
      {
        double[] dd = PhoneApplicationService.Current.State["parkCoord"] as double[];
        PhoneApplicationService.Current.State.Remove("parkCoord");
        locationResult = "to";
        ToText = await GetAddressFromGeoCoord(dd);
      }
    }

    #region Properties

    public string FromText
    {
      get { return fromText; }
      set
      {
        fromText = value;
        NotifyOfPropertyChange(() => FromText);
      }
    }

    public string ToText
    {
      get { return toText; }
      set
      {
        toText = value;
        NotifyOfPropertyChange(() => ToText);
      }
    }

    public SingleJourney Journey
    {
      get { return journey; }
      set
      {
        journey = value;
        NotifyOfPropertyChange(() => Journey);
      }
    }

    public DateTime DepartureDateTime
    {
      get { return departureDate; }
      set
      {
        departureDate = value;
        NotifyOfPropertyChange(() => DepartureDateTime);
      }
    }

    public bool IsSettingsShown
    {
      get { return isSettingsShown; }
      set 
      {
        isSettingsShown = value;
        NotifyOfPropertyChange(() => IsSettingsShown);

      }
    }

    public bool IsAppbarShown
    {
      get { return !pu.IsOpen; }
      
    }

    #endregion

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
      return result.StartsWith(",")? result.Substring(2): result;
    }

    #endregion

    #region LocationSelector

    private void ShowLocationMethodChooser()
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

    async void mp_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
    {
      MessagePrompt mp = sender as MessagePrompt;
      switch (mp.Value)
      {
        case "current": Assegna(await GetAddressFromGeoCoord(Settings.GPSPosition)); break;
        case "openMap": ShowMappaGrande(); break;
        default: break;
      }
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
      NotifyOfPropertyChange(() => IsAppbarShown);

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
      vibro.Vibrate(new TimeSpan(0, 0,1));
      
      //uncomment to enable hold to message box flow
      //p_Tap(p, e);
    }

    void p_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (MessageBox.Show((sender as Pushpin).Content as string, AppResources.ChooseConfirmTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        pu.IsOpen = false;
        Assegna((sender as Pushpin).Content as string);
      }
    }

    #endregion

    #region Buttons eventhandlers

    public void Assegna(string result)
    {
      if (locationResult == "from")
        FromText = result;
      else if (locationResult == "to")
        ToText = result;
    }

    public void GpsLocFrom()
    {
      locationResult = "from";
      ShowLocationMethodChooser();
    }

    public void GpsLocTo()
    {
      locationResult = "to";
      ShowLocationMethodChooser();
    }

    public void PlanNewJourney()
    {
      //finalize SingleJourneyObject and proceed to post
    }

    #endregion
  }
}
