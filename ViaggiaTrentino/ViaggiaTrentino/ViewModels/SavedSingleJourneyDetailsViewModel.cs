using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
      PhoneApplicationService.Current.State.Remove("journey");
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      gplHelp = new GooglePolyline();
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

    }

    //public void DisplayPolylineMap(Leg dataContext)
    //{
    //  gplHelp.ShowMapWithPath(dataContext.LegGeometryInfo.Points);     
    //}

    public void DisplayPolylineMap(SavedSingleJourneyDetailsView fullView)
    {
      gplHelp.ShowMapWithFullPath(fullView.listLegsBox.Items, fullView.listLegsBox.SelectedItem as Leg);
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
