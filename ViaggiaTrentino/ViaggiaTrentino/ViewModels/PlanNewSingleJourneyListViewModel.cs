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

namespace ViaggiaTrentino.ViewModels
{
  public class PlanNewSingleJourneyListViewModel : Screen
  {
    private readonly INavigationService navigationService;
    RoutePlanningLibrary rpLib;
    ObservableCollection<Itinerary> listIti;
    
    public ObservableCollection<Itinerary> ListIti
    {
      get { return listIti; }
      set
      {
        listIti = value;
        NotifyOfPropertyChange(() => ListIti);
      }
    }
    
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
      }
      catch (Exception e)
      {
#if DEBUG
        System.Windows.MessageBox.Show(e.Message);
#endif

      }
      finally
      {
        App.LoadingPopup.Hide();
      }
      App.LoadingPopup.Show();
      SingleJourney sj = PhoneApplicationService.Current.State["singleJourney"] as SingleJourney;
      PhoneApplicationService.Current.State.Remove("singleJourney");
      await Settings.RefreshToken();
      List<Itinerary> li = await rpLib.PlanSingleJourney(sj);
      if(li != null)
        ListIti = new ObservableCollection<Itinerary>(li);
      App.LoadingPopup.Hide();
    }

    public void OpenDetailView(object dataContext)
    {
      PhoneApplicationService.Current.State["singleJourney"] = dataContext;
      navigationService.UriFor<PlanNewSingleJourneySaveViewModel>().Navigate();
    }

  }
}
