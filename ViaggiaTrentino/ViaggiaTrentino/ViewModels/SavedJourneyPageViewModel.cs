using Caliburn.Micro;
using MobilityServiceLibrary;
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
    UserRouteLibrary urLib;

    public SavedJourneyPageViewModel(INavigationService navigationService)
    {
      mySavedJourneys = new ObservableCollection<BasicItinerary>();
      this.navigationService = navigationService;
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
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

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      List<BasicItinerary> basList = await urLib.ReadAllSingleJourneys();
      
      //foreach (var item in basList)
      //{
        
      //    deleted = await urLib.DeleteSingleJourney(item.ClientId);
       
      //}
      MySavedJourneys.Add(basList.FirstOrDefault());
    }

  }
}
