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
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class StopTimesForStopViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private PublicTransportLibrary ptl;
    private AgencyType agencyID;
    private string stopID;
    private ObservableCollection<TripData> trips;


    public StopTimesForStopViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }

    public string StopID
    {
      get { return stopID; }
      set { stopID = value; }
    }

    public ObservableCollection<TripData> Trips
    {
      get { return trips; }
      set { trips = value; }
    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      var a = await ptl.GetLimitedTimetable(AgencyID, stopID, 3);
      Trips = new ObservableCollection<TripData>(a);
    }
  }
}