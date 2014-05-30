﻿using Caliburn.Micro;
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
      SingleJourney sj = PhoneApplicationService.Current.State["singleJorney"] as SingleJourney;
      PhoneApplicationService.Current.State.Remove("singleJorney");
      List<Itinerary> li = await rpLib.PlanSingleJourney(sj);
      ListIti = new ObservableCollection<Itinerary>(li);
    }

  }
}