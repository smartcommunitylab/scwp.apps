using Caliburn.Micro;
using Models.MobilityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class RealTimeInfoViewModel : Screen
  {
    private readonly INavigationService navigationService;
    public RealTimeInfoViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }
    public void ParkingTile()
    {
      navigationService.UriFor<ParkingsPageViewModel>().Navigate();
    }
    public void TrentoBusTile()
    {
      navigationService.UriFor<SelectBusRouteViewModel>().WithParam(x => x.AgencyID, AgencyType.TrentoCityBus).Navigate();
    }

    public void RoveretoBusTile()
    {
      navigationService.UriFor<SelectBusRouteViewModel>().WithParam(x => x.AgencyID, AgencyType.RoveretoCityBus).Navigate();
    }

  }
}
