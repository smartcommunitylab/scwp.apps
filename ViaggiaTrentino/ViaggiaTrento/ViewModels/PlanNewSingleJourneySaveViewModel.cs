﻿using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System.Windows;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views;

namespace ViaggiaTrentino.ViewModels
{
  public class PlanNewSingleJourneySaveViewModel : Screen
  {
    private readonly INavigationService navigationService;
    GooglePolyline gplHelp;
    UserRouteLibrary urLib;
    Itinerary iti;

    public PlanNewSingleJourneySaveViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      gplHelp = new GooglePolyline();
      iti = new Itinerary();
    }

    public Itinerary PlannedIti
    {
      get { return iti; }
      set
      {
        iti = value;
        NotifyOfPropertyChange(() => PlannedIti);
      }
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      PlannedIti = PhoneApplicationService.Current.State["singleJourney"] as Itinerary;
      PhoneApplicationService.Current.State.Remove("singleJourney");
    }

    public void DisplayPolylineMap(PlanNewSingleJourneySaveView fullView)
    {
      gplHelp.ShowMapWithFullPath(fullView.listLegsBox.Items, fullView.listLegsBox.SelectedItem as Leg);
    }

    #region Appbar

    public void BarSave()
    {
      InputPrompt ip = new InputPrompt();
      ip.Message = AppResources.JourneyNameMsg;
      ip.Title = AppResources.JourneyNameTit;
      ip.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      ip.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

      ip.Completed += ip_Completed;
      ip.Show();
    }

    async void ip_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
    {
      if (e.PopUpResult == PopUpResult.Ok)
      {
        if (e.Result != "")
        {
          BasicItinerary respIti = null;
          BasicItinerary basIti = new BasicItinerary()
          {
            Data = iti,
            Monitor = true,
            Name = e.Result
          };

          try
          {
            App.LoadingPopup.Show();
            await Settings.RefreshToken();
            respIti = await urLib.SaveSingleJourney(basIti);
          }
          finally
          {
            App.LoadingPopup.Hide();
          }

          if (respIti is BasicItinerary)
            navigationService.UriFor<SavedJourneyPageViewModel>().WithParam(x => x.LastSavedJourney, 0).Navigate();
        }
        else
          MessageBox.Show(AppResources.ValidationJTitle, AppResources.ValidationCaption, MessageBoxButton.OK);
      }
    }

    #endregion
  }
}
