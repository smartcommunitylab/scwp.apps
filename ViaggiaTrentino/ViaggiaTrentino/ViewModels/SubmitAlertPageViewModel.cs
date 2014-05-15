using Caliburn.Micro;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SubmitAlertPageViewModel: Screen
  {
    PublicTransportLibrary ptl;

    
    public Stop SelectedStop { get; set; }
    
    public Route selRoute;
    private AgencyType agencyID;
    private string lineName;
    private readonly INavigationService navigationService;
    private ObservableCollection<Route> routes;
    private ObservableCollection<Stop> stops;

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

    public Route SelectedRoute
    {
      get { return selRoute; } 
      set
      {
        selRoute = value;
        GetStopsForRoute(value);
      } 
    }

    public SubmitAlertPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    public async void GetStopsForRoute(Route r)
    {
      Stops = new ObservableCollection<Stop>(await ptl.GetStops(r.RouteId.AgencyId, r.RouteId.Id));
      SelectedStop = Stops.FirstOrDefault();
    }

    protected override async void OnViewLoaded(object view)
    {
       
      base.OnViewLoaded(view);
      Routes = new ObservableCollection<Route>(await ptl.GetRoutes(agencyID));
      SelectedRoute = Routes.FirstOrDefault();
      
    }

    public string LineName
    {
      get { return lineName; }
      set 
      { 
        lineName = value;
        NotifyOfPropertyChange(() => LineName);
      }
    }

    public AgencyType AgencyID 
    {
      get { return agencyID; }
      set { agencyID = value; } 
    }

    public void SubmitDelay()
    {
      
    }
  }
}
