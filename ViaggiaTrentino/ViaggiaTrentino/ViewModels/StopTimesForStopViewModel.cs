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
using ViaggiaTrentino.Helpers;

namespace ViaggiaTrentino.ViewModels
{
  public class StopTimesForStopViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;

    private PublicTransportLibrary ptl;
    private AgencyType agencyID;
    private string stopID;

    public StopTimesForStopViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
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

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      var grouped =
                from list in await ptl.GetLimitedTimetable(AgencyID, stopID, 3)
                group list by list.RouteShortName into listByGroup
                select new KeyedList<string, TripData>(listByGroup);

      eventAggregator.Publish(grouped);
      
    }
  }
}