using AuthenticationLibrary;
using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class SubmitAlertPageViewModel : Screen
  {
    PublicTransportLibrary ptl;
    private Route selRoute;
    private Stop selStop;
    private StopTime selST;
    private AgencyType agencyID;
    private string delay;
    private string lineName;
    private readonly INavigationService navigationService;
    private ObservableCollection<Route> routes;
    private ObservableCollection<Stop> stops;
    private ObservableCollection<StopTime> stopTimes;

    public string LineName
    {
      get { return lineName; }
      set
      {
        lineName = value;
        NotifyOfPropertyChange(() => LineName);
      }
    }

    public ObservableCollection<Route> Routes
    {
      get
      {
        return routes;
      }
      set
      {
        routes = value;
        NotifyOfPropertyChange(() => Routes);
      }
    }

    public ObservableCollection<Stop> Stops
    {
      get
      {
        return stops;
      }
      set
      {
        stops = value;
        NotifyOfPropertyChange(() => Stops);
      }
    }

    public ObservableCollection<StopTime> StopTimes
    {
      get { return stopTimes; }
      set
      {
        stopTimes = value;
        NotifyOfPropertyChange(() => StopTimes);
      }
    }

    public Route SelectedRoute
    {
      get { return selRoute; }
      set
      {
        selRoute = value;
        NotifyOfPropertyChange(() => SelectedRoute);
        GetStopsForRoute(value);
      }
    }

    public Stop SelectedStop
    {
      get { return selStop; }
      set
      {
        selStop = value;
        NotifyOfPropertyChange(() => SelectedStop);

        GetStopTimesForStop(value);
      }
    }

    public StopTime SelectedStopTime
    {
      get { return selST; }
      set
      {
        selST = value;

        NotifyOfPropertyChange(() => SelectedStopTime);

        //GetStopTimesForStop(value);
      }
    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }

    public void DelayTimeChanged(TextBox obj)
    {
      delay = obj.Text;
    }


    public SubmitAlertPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      using (DBHelper dbh = new DBHelper())
      {
        PhoneApplicationService.Current.State["routeNames"] = dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(agencyID));
      }

      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        var results = await ptl.GetRoutes(agencyID);
        results.Sort();
        Routes = new ObservableCollection<Route>(results);
        SelectedRoute = Routes.FirstOrDefault();
      }
      finally
      {
        App.LoadingPopup.Hide();
      }

    }

    public async void GetStopsForRoute(Route r)
    {
      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        Stops = new ObservableCollection<Stop>(await ptl.GetStops(r.RouteId.AgencyId, r.RouteId.Id));
        SelectedStop = Stops.FirstOrDefault();
      }     
      finally
      {
        App.LoadingPopup.Hide();
      }


    }

    private async void GetStopTimesForStop(Stop value)
    {
      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        StopTimes = new ObservableCollection<StopTime>(await ptl.GetTimetable(agencyID, selRoute.RouteId.Id, value.StopId));
        SelectedStopTime = StopTimes.FirstOrDefault();
      }
      finally
      {
        App.LoadingPopup.Hide();
      }

    }

    public async void SubmitDelay()
    {
      RealTimeUpdateLibrary rtul = new RealTimeUpdateLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      long a = (DateTime.Now.Ticks - 621355968000000000) / 10000000;
      AlertDelay ad = new AlertDelay()
      {
        CreatorId = Settings.UserID,
        CreatorType = CreatorType.User,
        Note = "",
        PositionInfo = new Models.MobilityService.Journeys.Position()
        {
          Latitude = SelectedStop.Latitude.ToString(),
          Longitude = SelectedStop.Longitude.ToString(),
          Name = SelectedStop.Name,
          Stop = new StopId { Agency = agencyID, Id = SelectedStop.StopId },
          StopCode = SelectedStop.StopId
        },
        Delay = Convert.ToInt32(delay),
        Type = AlertType.Delay,
        ValidFrom = (DateTime.Now.Ticks - 621355968000000000) / 10000000

      };

      await Settings.RefreshToken();
      rtul.SignalAlert<AlertDelay>(ad);
      navigationService.UriFor<MainPageViewModel>().Navigate();
    }
  }
}
