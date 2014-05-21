using Caliburn.Micro;
using Microsoft.Phone.Shell;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedSingleJourneyDetailsViewModel : Screen
  {
    private readonly INavigationService navigationService;
    BasicItinerary basIti;


    public BasicItinerary Journey
    {
      get { return basIti; }
      set
      {
        basIti = value;
        NotifyOfPropertyChange(() => Journey);
      }
    }
    public SavedSingleJourneyDetailsViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      Journey = PhoneApplicationService.Current.State["journey"] as BasicItinerary;
      PhoneApplicationService.Current.State.Clear();
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

    }

  }
}
