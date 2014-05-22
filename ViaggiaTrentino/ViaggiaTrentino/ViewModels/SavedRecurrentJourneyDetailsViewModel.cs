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
  public class SavedRecurrentJourneyDetailsViewModel : Screen
  {
    private readonly INavigationService navigationService;
    BasicRecurrentJourney basIti;
    UserRouteLibrary urLib;

    public BasicRecurrentJourney Journey
    {
      get { return basIti; }
      set
      {
        basIti = value;
        NotifyOfPropertyChange(() => Journey);
      }
    }

    public SavedRecurrentJourneyDetailsViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      Journey = PhoneApplicationService.Current.State["journey"] as BasicRecurrentJourney;      

      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

    }

    protected override void OnViewReady(object view)
    {
      base.OnViewReady(view);
      PhoneApplicationService.Current.State.Clear();
    }

    public async void BarMonitor()
    {
      Journey.Monitor = await urLib.SetMonitorRecurrentJourney(basIti.ClientId, !basIti.Monitor);
      NotifyOfPropertyChange(() => Journey);    
    }

    public async void BarDelete()
    {
      if (MessageBox.Show(AppResources.SureDelete, AppResources.Warn, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        if (await urLib.DeleteRecurrentJourney(basIti.ClientId))
          navigationService.UriFor<SavedJourneyPageViewModel>().Navigate();
    }

  }
}
