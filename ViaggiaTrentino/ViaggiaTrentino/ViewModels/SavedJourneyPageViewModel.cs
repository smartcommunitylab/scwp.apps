using Caliburn.Micro;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedJourneyPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    UserRouteLibrary urLib;

    ObservableCollection<BasicItinerary> mySavedSingleJourneys;
    ObservableCollection<BasicRecurrentJourney> mySavedRecurrentJourneys;


    List<BasicItinerary> basList;
    List<BasicRecurrentJourney> barList;

    public ObservableCollection<BasicItinerary> MySavedSingleJourneys
    {
      get { return mySavedSingleJourneys; }
      set
      {
        mySavedSingleJourneys = value;
        NotifyOfPropertyChange(() => MySavedSingleJourneys);
      }
    }
    
    public ObservableCollection<BasicRecurrentJourney> MySavedRecurrentJourneys
    {
      get { return mySavedRecurrentJourneys; }
      set
      {
        mySavedRecurrentJourneys = value;
        NotifyOfPropertyChange(() => MySavedRecurrentJourneys);
      }
    }

    public SavedJourneyPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      mySavedSingleJourneys = new ObservableCollection<BasicItinerary>();
      mySavedRecurrentJourneys = new ObservableCollection<BasicRecurrentJourney>();
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      
    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      App.LoadingPopup.Show();
      await Settings.RefreshToken();
      basList = await urLib.ReadAllSingleJourneys();
      barList = await urLib.ReadAllRecurrentJourneys();
      App.LoadingPopup.Hide();
      BackgroundWorker bw = new BackgroundWorker();
      bw.DoWork += bw_DoWork;
      bw.ProgressChanged += bw_ProgressChanged;
      bw.WorkerReportsProgress = true;
      bw.WorkerSupportsCancellation = true;
      bw.RunWorkerAsync(); 
    }

    void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if(e.UserState is BasicRecurrentJourney)
        MySavedRecurrentJourneys.Add(e.UserState as BasicRecurrentJourney);
      else MySavedSingleJourneys.Add(e.UserState as BasicItinerary);      
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      foreach (var item in basList)
      {
        (sender as BackgroundWorker).ReportProgress(0, item);
        Thread.Sleep(100);
      }

      foreach (var item in barList)
      {
        (sender as BackgroundWorker).ReportProgress(0, item);
        Thread.Sleep(100);
      }
    }

    public void OpenRecurrentJourney(BasicRecurrentJourney journey)
    {
      PhoneApplicationService.Current.State["journey"] = journey;
      navigationService.UriFor<SavedRecurrentJourneyDetailsViewModel>().Navigate();
    }

    public void OpenSingleJourney(BasicItinerary journey)
    {
      PhoneApplicationService.Current.State["journey"] = journey;
      navigationService.UriFor<SavedSingleJourneyDetailsViewModel>().Navigate();
    }

  }
}
