using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViaggiaTrentino.ViewModels
{
  public class PlanNewSingleJourneyListViewModel : Screen
  {
    private readonly INavigationService navigationService;
    RoutePlanningLibrary rpLib;
    string from, to;
    ObservableCollection<Itinerary> listIti;

    #region Properties

    public ObservableCollection<Itinerary> ListIti
    {
      get { return listIti; }
      set
      {
        listIti = value;
        NotifyOfPropertyChange(() => ListIti);
      }
    }

    public string From
    {
      get { return from; }
      set
      {
        from = value;
        NotifyOfPropertyChange(() => From);
      }
    }

    public string To
    {
      get { return to; }
      set
      {
        to = value;
        NotifyOfPropertyChange(() => To);
      }
    }

    #endregion

    public PlanNewSingleJourneyListViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      rpLib = new RoutePlanningLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      listIti = new ObservableCollection<Itinerary>();
    }

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      try
      {
        App.LoadingPopup.Show();
        SingleJourney sj = PhoneApplicationService.Current.State["singleJourney"] as SingleJourney;
        From = sj.From.Name;
        To = sj.To.Name;
        PhoneApplicationService.Current.State.Remove("singleJourney");
        await Settings.RefreshToken();
        List<Itinerary> li = await rpLib.PlanSingleJourney(sj);
        if (li != null)
          ListIti = new ObservableCollection<Itinerary>(li);
      }
      finally
      {
        App.LoadingPopup.Hide();
      }
      
    }

    public void OpenDetailView(object dataContext)
    {
      PhoneApplicationService.Current.State["singleJourney"] = dataContext;
      navigationService.UriFor<PlanNewSingleJourneySaveViewModel>().Navigate();
    }

  }
}
