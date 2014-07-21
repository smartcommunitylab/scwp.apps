using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Text;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class PlanNewSingleJourneyViewModel: Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    LocationChooserHelper lch;
    FavouriteLocationHelper flh;
    private bool isSettingsShown;
    private SingleJourney journey;
    private DateTime departureDate;
    private bool isAppBarShown;
    private Position from;
    private Position to;
    private string locationResult;
    private PreferencesModel pm;

    public PlanNewSingleJourneyViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      departureDate = DateTime.Now+ new TimeSpan(0,5,0);
      journey = new SingleJourney();
      isAppBarShown = true;
      to = new Position() { Name = "" };
      from = new Position() { Name = "" };
      pm = Settings.AppPreferences.Clone();
      lch = new LocationChooserHelper();
      lch.PositionObtained += PositionObtained;
      flh = new FavouriteLocationHelper();
      flh.FavouriteSelectionCompleted += PositionObtained;
    }

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
    
    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

      if (PhoneApplicationService.Current.State.ContainsKey("navigationCoord"))
      {
        double[] dd = PhoneApplicationService.Current.State["navigationCoord"] as double[];
        PhoneApplicationService.Current.State.Remove("navigationCoord");
        locationResult = "to";
        ToPos = new Position() 
                 { 
                   Name = await lch.GetAddressFromGeoCoord(dd),
                   Latitude = dd[0].ToString(),
                   Longitude = dd[1].ToString() 
                 };
      }
    }

    #region Properties

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

    public bool IsAppBarShown
    {
      get { return isAppBarShown; }
      set
      {
        isAppBarShown = value;
        NotifyOfPropertyChange(() => IsAppBarShown);
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

    #endregion

    #region Appbar

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

    #endregion

    public void PlanNewJourney()
    {
      if (ValidateSingleJourney())
      {
        SingleJourney sj = new SingleJourney()
        {

          Date = departureDate.ToString("MM/dd/yyyy"),
          DepartureTime = departureDate.ToString("hh:mm tt").Replace(" ", ""),
          From = FromPos,
          To = ToPos,
          ResultsNumber = 3,
          RouteType = SelectedRouteType,
          TransportTypes = SelectedTransportTypes
        };

        sj.From.Latitude = sj.From.Latitude.Replace(',', '.');
        sj.From.Longitude = sj.From.Longitude.Replace(',', '.');
        sj.To.Latitude = sj.To.Latitude.Replace(',', '.');
        sj.To.Longitude = sj.To.Longitude.Replace(',', '.');

        PhoneApplicationService.Current.State["singleJourney"] = sj;
        navigationService.UriFor<PlanNewSingleJourneyListViewModel>().Navigate();
      }
    }

    private bool ValidateSingleJourney()
    {
      StringBuilder sb = new StringBuilder();

      if(from.Longitude == null)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationFrom));
      if (to.Longitude == null)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationTo));
      if (departureDate < DateTime.Now)
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationStartDate));
      if(SelectedTransportTypes.Length == 0)
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
