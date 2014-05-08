using Caliburn.Micro;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedJourneyPageViewModel : Screen
  {
    private readonly INavigationService navigationService;

    public SavedJourneyPageViewModel(INavigationService navigationService)
    {
      mySavedJourneys = new ObservableCollection<BasicItinerary>();
      this.navigationService = navigationService;

    }


    ObservableCollection<BasicItinerary> mySavedJourneys;

    public ObservableCollection<BasicItinerary> MySavedJourneys
    {
      get { return mySavedJourneys; }
      set
      {
        mySavedJourneys = value;
        NotifyOfPropertyChange(() => MySavedJourneys);
      }
    }
  }
}
