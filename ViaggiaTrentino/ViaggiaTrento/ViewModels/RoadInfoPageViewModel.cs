using Caliburn.Micro;
using MobilityServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class RoadInfoPageViewModel : Screen
  {
    private IEventAggregator eventAggregator;
    private INavigationService navigationService;
    private PublicTransportLibrary ptl;

    public RoadInfoPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.eventAggregator = eventAggregator;
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);

    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      var results = await ptl.GetRoadInfoByAgency(Settings.ParkingAgencyId, DateTimeToEpoch(DateTime.Now.AddMonths(-12)), DateTimeToEpoch(DateTime.Now));
    }

    private long DateTimeToEpoch(DateTime dt)
    {
      TimeSpan span = (dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
      return Convert.ToInt64(span.TotalSeconds);
    }
  }
}
