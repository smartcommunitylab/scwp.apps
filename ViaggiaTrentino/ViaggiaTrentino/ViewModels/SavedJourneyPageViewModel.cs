using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedJourneyPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    UserRouteLibrary urLib;

    ObservableCollection<BasicItinerary> mySavedSingleJourneys;
    ObservableCollection<BasicRecurrentJourney> mySavedRecurrentJourneys;


    public ObservableCollection<BasicItinerary> MySavedSingleJourneys
    {
      get { return mySavedSingleJourneys; }
      set
      {
        mySavedSingleJourneys = value;
        NotifyOfPropertyChange(() => MySavedSingleJourneys);
      }
    }
    
    public ObservableCollection<BasicRecurrentJourney> MySavedRecurrentJourneys
    {
      get { return mySavedRecurrentJourneys; }
      set
      {
        mySavedRecurrentJourneys = value;
        NotifyOfPropertyChange(() => MySavedRecurrentJourneys);
      }
    }

    public SavedJourneyPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      mySavedSingleJourneys = new ObservableCollection<BasicItinerary>();
      mySavedRecurrentJourneys = new ObservableCollection<BasicRecurrentJourney>();
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      
    }    

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      List<BasicItinerary> basList = await urLib.ReadAllSingleJourneys();
      List<BasicRecurrentJourney> barList = await urLib.ReadAllRecurrentJourneys();

      foreach (var item in basList)
      {
        MySavedSingleJourneys.Add(item);
      }

      foreach (var item in barList)
      {
        MySavedRecurrentJourneys.Add(item);
      }

    }

    public void OpenRecurrentJourney(BasicRecurrentJourney journey)
    {
      MessageBox.Show("no moar");
    }

    public void OpenSingleJourney(BasicItinerary journey)
    {
      PhoneApplicationService.Current.State["journey"] = journey;
      navigationService.UriFor<SavedSingleJourneyDetailsViewModel>().Navigate();
    }

  }
}
