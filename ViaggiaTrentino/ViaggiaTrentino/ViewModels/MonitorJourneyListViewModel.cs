using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class MonitorJourneyListViewModel : Screen
  {
    private readonly INavigationService navigationService;
    RoutePlanningLibrary rpLib;
    UserRouteLibrary urLib;
    RecurrentJourney recJ;
    RecurrentJourneyParameters recJP;

    public RecurrentJourney RecJourney
    {
      get { return recJ; }
      set
      {
        recJ = value;
        NotifyOfPropertyChange(null);
      }
    }

    public RecurrentJourneyParameters RecJourneyParams
    {
      get { return recJP; }
      set
      {
        recJP = value;
        NotifyOfPropertyChange(null);
      }
    }

    public MonitorJourneyListViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      rpLib = new RoutePlanningLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      try
      {
        App.LoadingPopup.Show();
        recJP = PhoneApplicationService.Current.State["recurrentJourney"] as RecurrentJourneyParameters;
        PhoneApplicationService.Current.State.Remove("recurrentJourney");
        await Settings.RefreshToken();
        RecurrentJourney rj = await rpLib.PlanRecurrentJourney(recJP);
        Dictionary<string, bool> monitoredLegs = new Dictionary<string, bool>();
        foreach (var gambaSemplice in rj.Legs)
        {
          string key = string.Format("{0}_{1}", gambaSemplice.TransportInfo.AgencyId, gambaSemplice.TransportInfo.RouteId);
          monitoredLegs[key] = true;
        }
        rj.MonitorLegs = monitoredLegs;
        if (rj != null)
          RecJourney = rj;
      }
      finally
      {
        App.LoadingPopup.Hide();
      }

    }

    public void CheckBoxPressed(CheckBox sender, SimpleLeg gambaSemplice)
    {
      if (gambaSemplice.TransportInfo.RouteId != null)
      {
        //isSomethingChanged = true;
        string key = string.Format("{0}_{1}", gambaSemplice.TransportInfo.AgencyId, gambaSemplice.TransportInfo.RouteId);
        if (recJ.MonitorLegs.ContainsKey(key))
          recJ.MonitorLegs[key] = Convert.ToBoolean(sender.IsChecked);
        else recJ.MonitorLegs.Add(key, Convert.ToBoolean(sender.IsChecked));
      }
    }

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
          BasicRecurrentJourney respJourney = null;

          try
          {
            BasicRecurrentJourney brj = new BasicRecurrentJourney()
            {

              Data = recJ,
              Monitor = true,
              Name = e.Result
            };
            await Settings.RefreshToken();
            respJourney = await urLib.SaveRecurrentJourney(brj);
          }
          finally
          {
            App.LoadingPopup.Hide();
            if (respJourney != null)
              navigationService.UriFor<SavedJourneyPageViewModel>().WithParam(x => x.LastSavedJourney, 1).Navigate();
          }


        }
        else
          MessageBox.Show(AppResources.ValidationJTitle, AppResources.ValidationCaption, MessageBoxButton.OK);
      }
    }

  }
}
