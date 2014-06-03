using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;
using Windows.Phone.Devices.Notification;

namespace ViaggiaTrentino.ViewModels
{
  public class MonitorJourneyViewModel : Screen
  {
    private readonly INavigationService navigationService;
    ObservableCollection<object> selDays;
    bool isSettingsShown;
    bool isAlways;
    RecurrentJourney journey;
    DateTime beginDate;
    DateTime endDate;
    Popup pu;
    private Position from;
    private Position to;
    private PreferencesModel pm;
    string locationResult;

    public MonitorJourneyViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      beginDate = DateTime.Now;
      endDate = DateTime.Now;
      journey = new RecurrentJourney();
      selDays = new ObservableCollection<object>();
      GiorniScelti.Add(DateTime.Now.ToString("dddd"));
      pu = new Popup();
      pu.Closed += pu_Closed;
      to = new Position() { Name = "" };
      from = new Position() { Name = "" };
      pm = Settings.AppPreferences.Clone();
    }

    void pu_Closed(object sender, EventArgs e)
    {
      NotifyOfPropertyChange(() => IsAppbarShown);
    }

    public PreferencesModel JourneySettings
    {
      get { return pm; }
      set
      {
        pm = value;
        NotifyOfPropertyChange(() => JourneySettings);
      }
    }

    public string FromText
    {
      get { return from.Name; }
    }

    public string ToText
    {
      get { return to.Name; }
    }

    public Position FromPos
    {
      get { return from; }
      set
      {
        from = value;
        NotifyOfPropertyChange(() => FromText);
      }
    }

    public Position ToPos
    {
      get { return to; }
      set
      {
        to = value;
        NotifyOfPropertyChange(() => ToText);
      }
    }

    public RecurrentJourney Journey
    {
      get { return journey; }
      set
      {
        journey = value;
        NotifyOfPropertyChange(() => Journey);
      }
    }

    public ObservableCollection<object> GiorniScelti
    {
      get { return selDays; }
      set
      {
        selDays = value;
        NotifyOfPropertyChange(() => GiorniScelti);
      }
    }

    public DateTime BeginDateTime
    {
      get { return beginDate; }
      set
      {
        beginDate = value;
        NotifyOfPropertyChange(() => BeginDateTime);
      }
    }

    public DateTime EndDateTime
    {
      get { return endDate; }
      set
      {
        endDate = value;
        NotifyOfPropertyChange(() => EndDateTime);
      }
    }

    //big cheat
    public bool CanChoose
    {
      get { return !IsAlways; }
    }

    public bool IsAlways
    {
      get { return isAlways; }
      set
      {
        isAlways = value;
        NotifyOfPropertyChange(() => IsAlways);
        NotifyOfPropertyChange(() => CanChoose);
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

    #endregion

    #region Buttons eventhandlers

    public void Assegna(Pushpin result)
    {
      if (locationResult == "from")
        FromPos = new Position()
        {
          Name = result.Content as string,
          Latitude = result.GeoCoordinate.Latitude.ToString(),
          Longitude = result.GeoCoordinate.Longitude.ToString()
        };
      else if (locationResult == "to")
        ToPos = new Position()
        {
          Name = result.Content as string,
          Latitude = result.GeoCoordinate.Latitude.ToString(),
          Longitude = result.GeoCoordinate.Longitude.ToString()
        };
      //ToText = result.Content as string;
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

    public RouteType SelectedRouteType
    {
      get
      {
        if (JourneySettings.PreferredRoute.Fastest)
          return RouteType.Fastest;
        if (JourneySettings.PreferredRoute.FewestChanges)
          return RouteType.LeastChanges;
        if (JourneySettings.PreferredRoute.LeastWalking)
          return RouteType.LeastWalking;

        //default choiche
        return RouteType.Fastest;
      }
    }

    public TransportType[] SelectedTransportTypes
    {
      get
      {
        List<TransportType> ltt = new List<TransportType>();
        if (JourneySettings.Transportation.Bike)
          ltt.Add(TransportType.Bicycle);
        if (JourneySettings.Transportation.Car)
          ltt.Add(TransportType.Car);
        if (JourneySettings.Transportation.SharedBike)
          ltt.Add(TransportType.SharedBike);
        if (JourneySettings.Transportation.SharedCar)
          ltt.Add(TransportType.SharedCar);
        if (JourneySettings.Transportation.Transit)
          ltt.Add(TransportType.Transit);
        if (JourneySettings.Transportation.Walking)
          ltt.Add(TransportType.Walk);
        return ltt.ToArray();
      }
    }

    public void PlanNewJourney()
    {
      RecurrentJourneyParameters rjp = new RecurrentJourneyParameters()
      {
        Time = "16:30",
        FromDate = Convert.ToInt64(DateTimeToEpoch(DateTime.Now)),
        ToDate = Convert.ToInt64(DateTimeToEpoch((DateTime.Now + new TimeSpan(14, 0, 0, 0)).ToUniversalTime())),
        Interval = Convert.ToInt64(1.5 * 60 * 60 * 1000),
        From = from,
        To = to,
        Recurrences = SelectedDaysToArray(),
        ResultsNumber = 3,
        RouteType = SelectedRouteType,
        TransportTypes = SelectedTransportTypes
      };

      PhoneApplicationService.Current.State["recurrentJorney"] = rjp;
      navigationService.UriFor<PlanNewSingleJourneyListViewModel>().Navigate();
    }

    private int[] SelectedDaysToArray()
    {
      List<int> usefulDays = new List<int>();
      var a = selDays.ToArray();

      List<string> localizedDays = new List<string>();
      
      for (int i = 1; i <= 7; i++)
      {
        localizedDays.Add(new DateTime(1970, 2, i).ToString("dddd", DateTimeFormatInfo.CurrentInfo));
      }
      
      foreach (var item in a)
      {
        int res = localizedDays.IndexOf(item as string);
        usefulDays.Add(res+1);
      }

      return usefulDays.ToArray();
    }

    private double DateTimeToEpoch(DateTime dt)
    {
      TimeSpan span = (dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
      return span.TotalMilliseconds;
    }

    #endregion
  }
}
