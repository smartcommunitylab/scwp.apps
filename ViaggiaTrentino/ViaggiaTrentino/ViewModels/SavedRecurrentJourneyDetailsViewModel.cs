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
using System.Windows.Controls;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedRecurrentJourneyDetailsViewModel : Screen
  {
    private readonly INavigationService navigationService;
    BasicRecurrentJourney basIti;
    UserRouteLibrary urLib;
    bool isSomethingChanged;

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
      isSomethingChanged = false;
    }

    protected async override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (isSomethingChanged && MessageBox.Show(AppResources.SureChange, AppResources.Warn, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        await urLib.UpdateRecurrentJourney(Journey.ClientId, Journey);
     

    }

    public void CheckBoxPressed(CheckBox sender, SimpleLeg gambaSemplice)
    {
      if (gambaSemplice.TransportInfo.RouteId != null)
      {
        isSomethingChanged = true;
        string key = string.Format("{0}_{1}", gambaSemplice.TransportInfo.AgencyId, gambaSemplice.TransportInfo.RouteId);
        if (Journey.Data.MonitorLegs.ContainsKey(key))
          Journey.Data.MonitorLegs[key] = Convert.ToBoolean(sender.IsChecked);
        else Journey.Data.MonitorLegs.Add(key, Convert.ToBoolean(sender.IsChecked));
      }
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
