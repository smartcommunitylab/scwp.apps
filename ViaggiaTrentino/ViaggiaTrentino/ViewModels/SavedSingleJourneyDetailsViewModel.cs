using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System.Windows;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedSingleJourneyDetailsViewModel : Screen
  {
    private readonly INavigationService navigationService;
    UserRouteLibrary urLib;
    BasicItinerary basIti;
    GooglePolyline gplHelp;
    bool isLoaded;

    #region Properties

    public BasicItinerary Journey
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

    public SavedSingleJourneyDetailsViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      Journey = PhoneApplicationService.Current.State["journey"] as BasicItinerary;
      PhoneApplicationService.Current.State.Remove("journey");
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      gplHelp = new GooglePolyline();
      IsLoaded = true;
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
    }
    
    public void DisplayPolylineMap(SavedSingleJourneyDetailsView fullView)
    {
      gplHelp.ShowMapWithFullPath(fullView.listLegsBox.Items, fullView.listLegsBox.SelectedItem as Leg);
    }

    #region Appbar

    public async void BarMonitor()
    {
      try
      {
        IsLoaded = false; App.LoadingPopup.Show();
        await Settings.RefreshToken();
        Journey.Monitor = await urLib.SetMonitorSingleJourney(basIti.ClientId, !basIti.Monitor);
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
          App.LoadingPopup.Show();
          await Settings.RefreshToken();
          delRes = await urLib.DeleteSingleJourney(basIti.ClientId);
        }
        finally
        {
          App.LoadingPopup.Hide();
          if (delRes)
            navigationService.UriFor<SavedJourneyPageViewModel>().Navigate();
        }        
      }
    }

    #endregion
  }
}
