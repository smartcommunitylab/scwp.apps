using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedJourneyPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    ObservableCollection<BasicRecurrentJourney> mySavedRecurrentJourneys;
    ObservableCollection<BasicItinerary> mySavedSingleJourneys;
    List<BasicRecurrentJourney> barList;
    List<BasicItinerary> basList;
    UserRouteLibrary urLib;
    private int lastSavedJourney;

    public SavedJourneyPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      mySavedSingleJourneys = new ObservableCollection<BasicItinerary>();
      mySavedRecurrentJourneys = new ObservableCollection<BasicRecurrentJourney>();
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    #region Properties

    public ObservableCollection<BasicItinerary> MySavedSingleJourneys
    {
      get { return mySavedSingleJourneys; }
      set
      {
        mySavedSingleJourneys = value;
        NotifyOfPropertyChange(() => MySavedSingleJourneys);
        NotifyOfPropertyChange(() => IsLonelyHere);
      }
    }

    public ObservableCollection<BasicRecurrentJourney> MySavedRecurrentJourneys
    {
      get { return mySavedRecurrentJourneys; }
      set
      {
        mySavedRecurrentJourneys = value;
        NotifyOfPropertyChange(() => MySavedRecurrentJourneys);
        NotifyOfPropertyChange(() => IsLonelyThere);
      }
    }

    public int LastSavedJourney
    {
      get { return lastSavedJourney; }
      set
      {
        lastSavedJourney = value;
        NotifyOfPropertyChange(() => LastSavedJourney);
      }
    }

    public bool IsLonelyHere
    {
      get { return mySavedSingleJourneys.Count == 0; }
    }

    public bool IsLonelyThere
    {
      get { return mySavedRecurrentJourneys.Count == 0; }
    }

    #endregion

    #region Page overrides

    protected override async void OnViewAttached(object view, object context)
    {
      base.OnViewAttached(view, context);

      try
      {
        MySavedRecurrentJourneys.Clear();
        MySavedSingleJourneys.Clear();
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        basList = await urLib.ReadAllSingleJourneys();
        barList = await urLib.ReadAllRecurrentJourneys();
        MySavedSingleJourneys = new ObservableCollection<BasicItinerary>(basList);
        MySavedRecurrentJourneys = new ObservableCollection<BasicRecurrentJourney>(barList);
      }
      finally
      {
        App.LoadingPopup.Hide();        
      }
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

      while (navigationService.BackStack.Count() > 1)
        navigationService.RemoveBackEntry();
    }

    #endregion

    #region Journey opening

    public void OpenRecurrentJourney(BasicRecurrentJourney journey)
    {
      PhoneApplicationService.Current.State["journey"] = journey;
      navigationService.UriFor<SavedRecurrentJourneyDetailsViewModel>().Navigate();
    }

    public void OpenSingleJourney(BasicItinerary journey)
    {
      PhoneApplicationService.Current.State["journey"] = journey;
      navigationService.UriFor<SavedSingleJourneyDetailsViewModel>().Navigate();
    }

    #endregion

  }
}
