using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedSingleJourneyDetailsViewModel : Screen
  {
    private readonly INavigationService navigationService;
    UserRouteLibrary urLib;
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
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

    }

    public async void BarMonitor()
    {
      Journey.Monitor = await urLib.SetMonitorSingleJourney(basIti.ClientId, !basIti.Monitor);
      NotifyOfPropertyChange(() => Journey);    
    }

    public async void BarDelete()
    {
      if (MessageBox.Show(AppResources.SureDelete, AppResources.Warn, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        if (await urLib.DeleteSingleJourney(basIti.ClientId))
          navigationService.UriFor<SavedJourneyPageViewModel>().Navigate();
    }

  }
}
