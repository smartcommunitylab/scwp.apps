using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
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
    bool isSomethingChanged, isLoaded;

    public SavedRecurrentJourneyDetailsViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      Journey = PhoneApplicationService.Current.State["journey"] as BasicRecurrentJourney;
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      IsLoaded = true;
    }

    #region Properties

    public BasicRecurrentJourney Journey
    {
      get { return basIti; }
      set
      {
        basIti = value;
        NotifyOfPropertyChange(() => Journey);
      }
    }

    public bool IsLoaded
    {
      get { return isLoaded; }
      set
      {
        isLoaded = value;
        NotifyOfPropertyChange(() => IsLoaded);
      }
    }

    #endregion

    #region Page overrides

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
    }

    protected override void OnViewReady(object view)
    {
      base.OnViewReady(view);
      PhoneApplicationService.Current.State.Remove("journey");
      isSomethingChanged = false;
    }

    protected async override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (isSomethingChanged && MessageBox.Show(AppResources.SureChange, AppResources.Warn, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        try
        {
          IsLoaded = false; App.LoadingPopup.Show();
          await Settings.RefreshToken();
          await urLib.UpdateRecurrentJourney(Journey.ClientId, Journey);
        }
        finally
        {
          App.LoadingPopup.Hide(); IsLoaded = true;
        }        
      }
    }

    #endregion

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

    #region Appbar

    public async void BarMonitor()
    {
      try
      {
        IsLoaded = false; App.LoadingPopup.Show();
        await Settings.RefreshToken();
        Journey.Monitor = await urLib.SetMonitorRecurrentJourney(basIti.ClientId, !basIti.Monitor);
        NotifyOfPropertyChange(() => Journey);    
      }
      finally
      {
        App.LoadingPopup.Hide(); IsLoaded = true;
      }     
    }

    public async void BarDelete()
    {
      if (MessageBox.Show(AppResources.SureDelete, AppResources.Warn, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        bool delRes = false;
        try
        {
          IsLoaded = false; App.LoadingPopup.Show();
          await Settings.RefreshToken();
          delRes = await urLib.DeleteRecurrentJourney(basIti.ClientId);
        }        
        finally
        {
          App.LoadingPopup.Hide(); IsLoaded = true;
          if (delRes)
            navigationService.UriFor<SavedJourneyPageViewModel>().Navigate();
        }
       
      }
    }

    #endregion
  }
}
