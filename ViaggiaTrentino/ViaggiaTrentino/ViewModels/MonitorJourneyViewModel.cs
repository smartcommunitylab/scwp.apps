using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class MonitorJourneyViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    ObservableCollection<object> selDays;
    LocationChooserHelper lch;
    FavouriteLocationHelper flh;
    bool isSettingsShown;
    bool isAlways;
    bool isAppBarShown;
    RecurrentJourney journey;
    DateTime beginDate;
    DateTime endDate;    
    private Position from;
    private Position to;
    private PreferencesModel pm;
    string locationResult;

    public MonitorJourneyViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      isAppBarShown = true;
      beginDate = DateTime.Now;
      endDate = DateTime.Now + new TimeSpan(2, 0, 0);
      journey = new RecurrentJourney();
      selDays = new ObservableCollection<object>();
      GiorniScelti.Add(DateTime.Now.ToString("dddd"));
      to = new Position() { Name = "" };
      from = new Position() { Name = "" };
      pm = Settings.AppPreferences.Clone();
      lch = new LocationChooserHelper();
      lch.PositionObtained += PositionObtained;
      flh = new FavouriteLocationHelper();
      flh.FavouriteSelectionCompleted += PositionObtained;
    }

    #region Properties

    void PositionObtained(object sender, Position results)
    {
      if (results != null)
      {
        if (locationResult == "from")
          FromPos = results;
        else if (locationResult == "to")
          ToPos = results;
      }
      IsAppBarShown = true;
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

    public Position FromPos
    {
      get { return from; }
      set
      {
        from = value;
        NotifyOfPropertyChange(() => FromPos);
        eventAggregator.Publish(new KeyValuePair<string, string>("from", from.Name));
      }
    }

    public Position ToPos
    {
      get { return to; }
      set
      {
        to = value;
        NotifyOfPropertyChange(() => ToPos);
        eventAggregator.Publish(new KeyValuePair<string, string>("to", to.Name));

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

    public bool IsAppBarShown
    {
      get { return isAppBarShown; }
      set
      {
        isAppBarShown = value;
        NotifyOfPropertyChange(() => IsAppBarShown);
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

    #endregion

    #region Buttons eventhandlers

    public void FavsFrom()
    {
      IsAppBarShown = false;
      locationResult = "from";
      flh.ShowFavouriteSelector();
    }

    public void FavsTo()
    {
      IsAppBarShown = false;
      locationResult = "to";
      flh.ShowFavouriteSelector();
    }
  
    public void GpsLocFrom()
    {
      IsAppBarShown = false;
      locationResult = "from";
      lch.ShowLocationSelectorHelper();
    }

    public void GpsLocTo()
    {
      IsAppBarShown = false;
      locationResult = "to";
      lch.ShowLocationSelectorHelper();
    }

    #region data format support

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
        usefulDays.Add(res + 1);
      }

      usefulDays.Sort();

      return usefulDays.ToArray();
    }

    private double DateTimeToEpoch(DateTime dt)
    {
      TimeSpan span = (dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
      return span.TotalMilliseconds;
    }

    #endregion

    public void PlanNewJourney()
    {
      if (ValidateRecurrentJourney())
      {
        long ToDateLong;
        if (isAlways)
          ToDateLong = 9999999999999L;
        else
          ToDateLong = Convert.ToInt64(DateTimeToEpoch((DateTime.Now + new TimeSpan(14, 0, 0, 0)).ToUniversalTime()));

        RecurrentJourneyParameters rjp = new RecurrentJourneyParameters()
        {
          Time = DateTime.Now.ToString("HH:mm"),
          FromDate = Convert.ToInt64(DateTimeToEpoch(beginDate)),
          ToDate = ToDateLong,
          Interval = Convert.ToInt64((endDate.TimeOfDay - beginDate.TimeOfDay).TotalMilliseconds),
          From = from,
          To = to,
          Recurrences = SelectedDaysToArray(),
          ResultsNumber = 3,
          RouteType = SelectedRouteType,
          TransportTypes = SelectedTransportTypes
        };

        rjp.From.Latitude = rjp.From.Latitude.Replace(',', '.');
        rjp.From.Longitude = rjp.From.Longitude.Replace(',', '.');
        rjp.To.Latitude = rjp.To.Latitude.Replace(',', '.');
        rjp.To.Longitude = rjp.To.Longitude.Replace(',', '.');

        PhoneApplicationService.Current.State["recurrentJourney"] = rjp;
        navigationService.UriFor<MonitorJourneyListViewModel>().Navigate();
      }
    }    

    private bool ValidateRecurrentJourney()
    {
      StringBuilder sb = new StringBuilder();

      if (from.Longitude == null)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationFrom));
      if (to.Longitude == null)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationTo));
      if (endDate.TimeOfDay - beginDate.TimeOfDay > new TimeSpan(2, 0, 0))
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationTimeSpan));
      if (SelectedDaysToArray().Length == 0)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationSelDays));
      if (SelectedTransportTypes.Length == 0)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationTType));
      
      string errors = sb.ToString();

      if (errors.Length > 0)
      {
        CustomMessageBox cmb = new CustomMessageBox()
        {
          Caption = AppResources.ValidationCaption,
          Message = AppResources.ValidationMessage,
          Content = sb.ToString(),
          LeftButtonContent = AppResources.ValidationBtnOk
        };
        cmb.Show();
        return false;
      }
      return true;
    }

    #endregion
  }
}
