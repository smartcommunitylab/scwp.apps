using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class MonitorJourneyListViewModel: Screen
  {
    private readonly INavigationService navigationService;
    RoutePlanningLibrary rpLib;
    RecurrentJourney recJ;

    public RecurrentJourney RecJourney
    {
      get { return recJ; }
      set
      {
        recJ = value;
        NotifyOfPropertyChange(() => RecJourney);
      }
    }

    public MonitorJourneyListViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      rpLib = new RoutePlanningLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);      
    }



    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      RecurrentJourneyParameters rjp = PhoneApplicationService.Current.State["recurrentJourney"] as RecurrentJourneyParameters;
      PhoneApplicationService.Current.State.Remove("recurrentJourney");
      RecurrentJourney rj = await rpLib.PlanRecurrentJourney(rjp);
      //rj.Parameters.From.Name
      if (rj != null)
        RecJourney = rj;
    }
  }
}
