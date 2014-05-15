using Caliburn.Micro;
using Models.MobilityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectAlertpageViewModel: Screen
  {
    private readonly INavigationService navigationService;

    public SelectAlertpageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    public void TrentoBus()
    {
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.Agency, AgencyType.TrentoCityBus).Navigate();
    }

    public void RoveretoBus()
    {
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.Agency, AgencyType.RoveretoCityBus).Navigate();

    }

    public void InterBus()
    {
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.Agency, AgencyType.TrentinoIntercityBus).Navigate();

    }

    public void Train()
    {
      //agency type of a railway, so that i can intercept it and then retrieve all railways
      navigationService.UriFor<SubmitAlertPageViewModel>().WithParam(x => x.Agency, AgencyType.TrentoMaleRailway).Navigate();
    }    
  }
}
