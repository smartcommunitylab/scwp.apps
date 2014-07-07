using Caliburn.Micro;
using Models.MobilityService;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectAlertPageViewModel: Screen
  {
    private readonly INavigationService navigationService;

    public SelectAlertPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    public void TrentoBus()
    {
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.AgencyID, AgencyType.TrentoCityBus)
         .WithParam(x => x.LineName, Resources.AppResources.SelSendAlertTn)
         .Navigate();
    }

    public void RoveretoBus()
    {
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.AgencyID, AgencyType.RoveretoCityBus)
        .WithParam(x => x.LineName, Resources.AppResources.SelSendAlertRv)
        .Navigate();
    }

    //public void InterBus()
    //{
    //  navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.AgencyID, AgencyType.TrentinoIntercityBus)
    //    .WithParam(x => x.LineName, Resources.AppResources.SelSendAlertInter)
    //    .Navigate();
    //}

    public void Train()
    {
      //agency type of a railway, so that i can intercept it and then retrieve all railways
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.AgencyID, AgencyType.BolzanoVeronaRailway)
        .WithParam(x => x.LineName, Resources.AppResources.SelSendAlertTrain)
        .Navigate();
    }    
  }
}
