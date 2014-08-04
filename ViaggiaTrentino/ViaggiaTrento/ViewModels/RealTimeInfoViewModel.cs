using Caliburn.Micro;
using Models.MobilityService;

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
    
    public void TrainTile()
    {
      navigationService.UriFor<SelectTrainRouteViewModel>().Navigate();
    }

    public void RoadBlocksTile()
    {
      navigationService.UriFor<RoadInfoPageViewModel>().Navigate();
    }
  }
}
