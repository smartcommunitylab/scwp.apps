using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
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
  public class PlanNewSingleJourneySaveViewModel : Screen
  {
    Itinerary iti;
    UserRouteLibrary urLib;

    private readonly INavigationService navigationService;
    
    public PlanNewSingleJourneySaveViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      iti = new Itinerary();      
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      PlannedIti = PhoneApplicationService.Current.State["singleJourney"] as Itinerary;
      PhoneApplicationService.Current.State.Remove("singleJourney");
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
          BasicItinerary basIti = new BasicItinerary()
          {
            Data = iti,
            Monitor = true,
            Name = e.Result
          };
          var resp = await urLib.SaveSingleJourney(basIti);
          if (resp is BasicItinerary)
            navigationService.UriFor<MainPageViewModel>().Navigate();
        }
      }
      else
        MessageBox.Show(AppResources.ValidationJTitle, AppResources.ValidationCaption, MessageBoxButton.OK);
    }

   
  }
}
